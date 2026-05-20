# 2. API Endpoints

The application exposes a single APIKit-managed HTTP API plus the APIKit
console. The HTTP listener binds to `0.0.0.0:${http.port}` (default `8081`)
and listens on two paths:

| Path        | Flow            | Purpose                                |
|-------------|-----------------|----------------------------------------|
| `/api/*`    | `movie-main`    | APIKit router for the business API     |
| `/console/*`| `movie-console` | APIKit console (RAML browser UI)       |

Source: `mulesoft/src/main/mule/interface.xml` (lines 4–122) and
`mulesoft/src/main/mule/global.xml`.

APIKit dispatches based on the RAML
`dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml:zip` (file
`movie.raml`). The flow names referenced in `interface.xml` reveal the two
operations:

```xml
<flow name="get:\movies:movie-config">          <!-- GET  /movies          -->
<flow name="post:\movies\(m_id):movie-config">  <!-- POST /movies/{m_id}   -->
```

## 2.1 `GET /api/movies` — List available movies

Implemented by the `GetMovies` flow in `implementation.xml` (lines 10–23).

### Request

- **Method**: `GET`
- **Path**: `/api/movies`
- **Headers**: none required (no auth in source).
- **Query / URI params**: none.
- **Body**: none.

### Behaviour

1. Run SQL:
   ```sql
   select * from movie_table where m_available > 0
   ```
   against `Database_Config` (MySQL — see `05-integrations-and-config.md`).
2. Transform the JDBC result set into JSON via DataWeave 2.0
   (`output application/json; --- payload`). This is an identity transform
   of the rows.
3. Log the resulting payload at `INFO`.

### Response — `200 OK`

`application/json` array of every column of `movie_table` for movies still
available. The auto-generated type inference (`application-types.xml`, lines
4–13) hints that downstream consumers should expect at least:

```json
[
  { "id": "1", "name": "abc", "no_of_tickets": 100 }
]
```

In reality the result is whatever `SELECT *` returns from the table (see
`04-data-model.md` for the inferred columns: `m_id`, `m_available`, plus
likely `m_name` and any other columns the DBA has added). The transform does
**not** rename, filter, or project columns.

### Failure modes

- APIKit-level errors (`BAD_REQUEST`, `NOT_FOUND`, `METHOD_NOT_ALLOWED`,
  `NOT_ACCEPTABLE`, `UNSUPPORTED_MEDIA_TYPE`, `NOT_IMPLEMENTED`) — see
  `07-error-handling.md`.
- Database connection / query errors — **no flow-level error handler is
  defined for `GetMovies`**. Errors propagate to the `movie-main` global
  handler, which only covers APIKit error types, so any other error
  ultimately surfaces as HTTP `500` with the raw error payload (the
  `http:error-response` defaults `httpStatus` to `500`).

## 2.2 `POST /api/movies/{m_id}` — Book tickets

Implemented by the `BookTickets` flow in `implementation.xml` (lines 24–75).

### Request

- **Method**: `POST`
- **Path**: `/api/movies/{m_id}` — `m_id` is the movie identifier (string in
  the URI; coerced to a number when used in DB params, see below).
- **Query param**: `no_tickets` — the number of tickets to book.
- **Headers**: none required.
- **Body**: ignored. The request body is **not read**; ticket count is
  taken from the query string.

The auto-generated `Input-Attributes` type confirms this shape
(`application-types.xml`, lines 115–140):

```dwl
queryParams: {| no_tickets: Number |}
uriParams:   {| m_id: String |}
```

### Behaviour

1. Capture inputs into a Mule variable `no_tickets`:
   ```dwl
   { no_tickets: attributes.queryParams.no_tickets,
     m_id:      attributes.uriParams.m_id }
   ```
   (Both are strings at this point — they come from the HTTP layer.)

2. SQL — read current availability for the movie:
   ```sql
   select m_available from movie_table where m_id = :m_id
   ```
   with `:m_id = vars.no_tickets.m_id`.

3. Validation (`validation:is-true`):
   ```dwl
   (payload[0].m_available as Number) - (vars.no_tickets.no_tickets as Number) >= 0
   ```
   - **Pass** → continue. Seats remain after this booking.
   - **Fail** → raises `VALIDATION:INVALID_BOOLEAN`, handled by the flow's
     own `on-error-continue` (see step 7 and `07-error-handling.md`).

4. SQL — insert the order:
   ```sql
   insert into order_table (m_id, no_tickets, price)
   values (:m_id, :no_tickets, :price)
   ```
   The `:price` is computed in DataWeave from the booking quantity using
   the pricing tiers documented in `03-business-rules.md`.

5. SQL — decrement availability:
   ```sql
   update movie_table
   set    m_available = m_available - :no_tickets
   where  m_id = :m_id
   ```

6. SQL — fetch the freshly inserted order:
   ```sql
   select * from order_table WHERE o_id = (SELECT MAX(o_id) FROM order_table)
   ```
   This relies on `o_id` being an auto-increment / monotonic column. It is
   **not** scoped per movie or per session, so a concurrent insert can race
   and return the wrong row (see migration notes).

7. Transform JDBC result → JSON (identity transform).

### Response — `200 OK`

`application/json` representation of the new `order_table` row, e.g.:

```json
[
  { "o_id": 42, "m_id": 2, "no_tickets": 3, "price": 300 }
]
```

The auto-generated *Output-Payload* type hints at
`{ message?: String }` with example `{"message":"ticket(s) booked"}`
(`application-types.xml`, lines 152–158), but **the actual implementation
returns the order row, not that message** — the auto-generated type is
stale. Migration should mirror the *implementation*, not the type hint.

### Response — booking rejected (insufficient seats)

When the validation step fails, the `on-error-continue` handler returns
HTTP `200` with the following JSON body:

```json
{
  "error": "avaible tickets is only <m_available> but you have ordered <no_tickets>"
}
```

Notes:

- The status code is **200**, not `400` or `409`, because
  `on-error-continue` swallows the error and the listener falls through to
  the default `vars.httpStatus default 200`. The migration should likely
  surface a `4xx` here — flagged in `08-migration-notes.md`.
- The spelling `avaible` is preserved from the legacy code.

### Failure modes

- `VALIDATION:INVALID_BOOLEAN` — handled inline (see above).
- APIKit-level errors — `07-error-handling.md`.
- DB errors during `INSERT`/`UPDATE`/second `SELECT` — **not handled in the
  flow**; they propagate to the global handler. There is no compensating
  transaction; a failure between `INSERT` and `UPDATE` (or vice versa) will
  leave the database inconsistent. See `08-migration-notes.md`.

## 2.3 `GET /console/*` — APIKit console

`movie-console` flow. Hosts the APIKit console UI for the same RAML. Has a
single `APIKIT:NOT_FOUND` handler returning `404` with body
`{"message":"Resource not found"}`. Not part of the business API contract;
typically disabled in production.
