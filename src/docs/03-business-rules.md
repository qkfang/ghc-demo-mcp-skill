# 3. Business Rules and Decision Points

This document captures the **business logic** encoded in the legacy
application: the rules a re-implementation must preserve to remain
behaviour-compatible. Every rule is traced back to a concrete source line
in `mulesoft/src/main/mule/implementation.xml`.

## 3.1 Movie listing rule

> A movie is returned by `GET /api/movies` if, and only if, its
> `m_available` column is **strictly greater than zero**.

Source — `implementation.xml:12`:

```sql
select * from movie_table where m_available > 0
```

Consequences:

- A movie with `m_available = 0` is hidden from listings, but it is **not**
  deleted. The legacy code never removes movies.
- No ordering, paging, or filtering by name/date is applied. The result is
  the natural row order returned by the database.
- Every column of `movie_table` is exposed verbatim in the response. There
  is no API-level whitelist.

## 3.2 Booking inputs

A booking is identified by:

| Input        | Source                              | Type at entry | Coerced to |
|--------------|-------------------------------------|---------------|------------|
| `m_id`       | URI path `/api/movies/{m_id}`       | String        | Number     |
| `no_tickets` | Query string `?no_tickets=N`        | String        | Number     |

Source — `implementation.xml:25` and the input-parameter expressions on
lines 28–30, 36–40, 45–48.

The request **body is ignored**. There is no user/customer identifier on a
booking, no payment token, no idempotency key, no booking timestamp from
the client.

## 3.3 Availability validation rule

> A booking is allowed only if there are at least `no_tickets` seats still
> available for the movie at the moment of the check.

Source — `implementation.xml:32`:

```dwl
(payload[0].m_available as Number) - (vars.no_tickets.no_tickets as Number) >= 0
```

Notes and edge cases:

- The check is on the value **read in the immediately preceding `SELECT`**
  (`select m_available from movie_table where m_id = :m_id`,
  `implementation.xml:27`). There is **no row lock**, no transaction, and no
  `SELECT ... FOR UPDATE`. Two concurrent bookings can both pass validation
  and oversell the movie.
- The comparison is `>= 0`, so booking exactly the remaining seat count is
  allowed and leaves `m_available = 0`.
- The validation processor's failure type is `VALIDATION:INVALID_BOOLEAN`;
  see `07-error-handling.md` for the response shape on failure.
- If `m_id` does not exist, `payload[0]` does not exist either and the
  validation expression throws a different error (not handled in the
  flow). This is a latent bug; flagged in `08-migration-notes.md`.

## 3.4 Pricing tiers

> Order `price` is calculated solely from `no_tickets` using a three-tier
> bulk-discount schedule:

| `no_tickets` (n) | Per-ticket price | Total price formula |
|------------------|------------------|---------------------|
| `1 ≤ n ≤ 5`      | 100              | `n * 100`           |
| `6 ≤ n ≤ 10`     | 90               | `n * 90`            |
| `n ≥ 11`         | 80               | `n * 80`            |

Source — `implementation.xml:38-40`:

```dwl
price:
  if (vars.no_tickets.no_tickets as Number <= 5)  vars.no_tickets.no_tickets as Number * 100
  else if (vars.no_tickets.no_tickets as Number <= 10) vars.no_tickets.no_tickets as Number * 90
  else vars.no_tickets.no_tickets as Number * 80
```

Worked examples:

- `1` ticket → `1 * 100 = 100`
- `5` tickets → `5 * 100 = 500`
- `6` tickets → `6 * 90 = 540`
- `10` tickets → `10 * 90 = 900`
- `11` tickets → `11 * 80 = 880`  ← **note** the boundary inversion below.

> ⚠️ **Tier boundary anomaly**: ordering 11 tickets is *cheaper* (880) than
> ordering 10 (900). This is a property of the bulk-discount tiers as
> coded; it has been preserved in this document because the legacy logic
> behaves this way. The migration team should decide whether to keep this
> behaviour or fix it (see `08-migration-notes.md`).

The currency is unspecified in the source. There is no `currency` column,
no tax handling, and no rounding logic. `price` is persisted as a raw
number.

## 3.5 Booking write sequence

The flow performs the following writes, **in order**, with **no enclosing
transaction**:

1. `INSERT INTO order_table (m_id, no_tickets, price) VALUES (...)`
   (`implementation.xml:34`).
2. `UPDATE movie_table SET m_available = m_available - :no_tickets WHERE
   m_id = :m_id` (`implementation.xml:44`).
3. `SELECT * FROM order_table WHERE o_id = (SELECT MAX(o_id) FROM
   order_table)` (`implementation.xml:52`).

Implications:

- A failure between step 1 and step 2 (e.g. database disconnect) leaves an
  order row without the corresponding availability decrement. There is no
  rollback or compensating action.
- Step 3 reads the globally newest order, not the order this flow just
  inserted. Under concurrent load it may return another caller's order
  row. Migration should use `LAST_INSERT_ID()` / `OUTPUT inserted.*` /
  `RETURNING` instead.

## 3.6 Confirmation response rule

> The booking response is the row inserted into `order_table`, JSON-encoded
> via `output application/json`.

Source — `implementation.xml:54-60` (the final transform).

Note: the auto-generated RAML type hint
(`application-types.xml:152-158`) suggests the response should be
`{ "message": "ticket(s) booked" }`. The implementation does **not** do
this. Where the contract and the implementation disagree, the
implementation is authoritative for migration.

## 3.7 Decision-point summary

| Decision           | Rule                                                                            | Source                          |
|--------------------|---------------------------------------------------------------------------------|---------------------------------|
| Show movie?        | `m_available > 0`                                                               | `implementation.xml:12`         |
| Accept booking?    | `m_available - no_tickets >= 0` for the selected `m_id`                         | `implementation.xml:32`         |
| Per-ticket price   | 100 (1–5), 90 (6–10), 80 (≥11)                                                  | `implementation.xml:38-40`      |
| Decrement seats    | `m_available -= no_tickets` on the booked movie                                 | `implementation.xml:44`         |
| Confirmation body  | The latest `order_table` row (by `MAX(o_id)`)                                   | `implementation.xml:52-60`      |
| Booking rejection  | Body `{ "error": "avaible tickets is only X but you have ordered Y" }`, 200 OK  | `implementation.xml:62-73`      |
