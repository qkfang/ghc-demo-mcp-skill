# 8. Migration Notes, Gaps, and Open Questions

This document collects everything the migration team needs to **decide**
or **clarify** before re-implementing the legacy MuleSoft application on
the target platform (per the existing `src/app` .NET app, `src/sql` SQL
deliverable, and `src/bicep` infrastructure).

## 8.1 Behavioural defects to fix in migration

These are bugs in the legacy implementation. Documenting them so the
migration team can fix them deliberately rather than re-implement them
accidentally.

| # | Defect | Legacy source | Recommended fix |
|---|--------|---------------|-----------------|
| D1 | **Oversell race**: availability is read, validated, decremented in separate non-transactional steps. Two concurrent bookings can both pass validation. | `implementation.xml:27-44` | Wrap the SELECT/INSERT/UPDATE in a transaction; use `SELECT … FOR UPDATE` or an `UPDATE … WHERE m_available >= :n` guarded by `ROW_COUNT()`. |
| D2 | **Wrong confirmation row**: `SELECT … WHERE o_id = (SELECT MAX(o_id) FROM order_table)` can return another caller's order under concurrency. | `implementation.xml:52` | Use `OUTPUT inserted.*` (SQL Server) / `RETURNING *` (Postgres) / `LAST_INSERT_ID()` (MySQL) from the same connection. |
| D3 | **No transaction across writes**: a failure between the `INSERT` and the `UPDATE` leaves the database inconsistent. | `implementation.xml:34-44` | Single transaction or compensating action. |
| D4 | **Booking rejection returns HTTP 200** instead of a 4xx, hiding the failure from non-body-parsing clients. | `implementation.xml:62-74`, `07-error-handling.md` §7.2 | Return `409 Conflict` (preferred) or `400 Bad Request`. |
| D5 | **Unknown `m_id` is not handled**: `payload[0].m_available` will throw on an empty result set, which is not a `VALIDATION:INVALID_BOOLEAN`, so it falls through to a 500. | `implementation.xml:27, 32` | Detect empty result and return `404 Not Found` for the movie. |
| D6 | **No request-body validation** on POST: the body is ignored. If clients are expected to send a body, this should be enforced (or documented as ignored). | `implementation.xml:25` | Decide intended contract; reject unexpected payloads or codify "body ignored". |
| D7 | **Typo** in error message: `avaible` should be `available`. | `implementation.xml:72` | Fix in migration; consider a structured error code as well. |
| D8 | **Plaintext DB credentials committed** to `config.yaml`. | `config.yaml:1-8` | Treat as compromised, rotate immediately, store in a secrets manager. |
| D9 | **Tier-boundary pricing inversion**: 11 tickets (880) is cheaper than 10 (900). | `implementation.xml:38-40`, `03-business-rules.md` §3.4 | Confirm intended pricing with the business; the legacy table can be preserved if intended, otherwise switch to "discount per ticket" or "lowest of …" formulas. |
| D10 | **Stale auto-generated type hint** in `application-types.xml` claims the response is `{ "message": "ticket(s) booked" }`. The implementation returns the order row. | `application-types.xml:152-158` vs `implementation.xml:54-60` | Decide which contract is correct. The implementation has been live; clients likely depend on it. Prefer the implementation, update the contract. |

## 8.2 Information gaps

Things the migration team needs to obtain from outside this repo before
implementing.

| # | Gap | How to close |
|---|-----|--------------|
| G1 | **Authoritative RAML** for the API. It lives in Anypoint Exchange as `dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml:zip`, not in source. | Pull the artifact from Exchange (or ask the team that owns the Anypoint org), commit a copy to `src/docs/contracts/` for offline reference. |
| G2 | **Real `movie_table` schema**. `SELECT *` in `GetMovies` exposes every column to clients; only `m_id` and `m_available` are named in source. | Connect to the legacy MySQL instance (with rotated credentials, §D8) and run `SHOW CREATE TABLE movie_table;` and `DESCRIBE movie_table;`. Update `04-data-model.md`. |
| G3 | **Real `order_table` schema** — is `o_id` truly auto-increment? Are there other columns (created_at, status, customer)? | Same approach as G2. |
| G4 | **Indexes and constraints** on both tables (in particular foreign keys, unique constraints, default values). | Same approach as G2. |
| G5 | **Production data volume and concurrency**. The legacy design assumes serialised access. The migration target should size the DB and transaction strategy based on real traffic. | Ask SRE / observability for request rate and concurrent-user numbers. |
| G6 | **Currency and locale of `price`**. The number is stored without a currency code or scale. | Ask product. |
| G7 | **MUnit / acceptance tests**: none committed despite MUnit being on the classpath. | If MUnit suites exist elsewhere, mirror them into the migration target's test plan; otherwise create new acceptance tests against the documented behaviour in `02-api-endpoints.md` and `03-business-rules.md`. |
| G8 | **Production configuration values** (CloudHub workers, environment names, business group) — referenced as Maven properties but not committed. | Obtain from the CloudHub deployment owner. |

## 8.3 Open questions for the business

Where intent cannot be inferred from code, ask the product owners:

1. Is the **pricing tier inversion** (D9) intentional, or should the
   migration smooth it into a monotone-non-increasing schedule?
2. Should bookings be tied to a **customer / user**? The legacy data model
   has no such field; modern usage likely requires it.
3. Are bookings ever **cancelled or refunded**? The legacy code only ever
   appends to `order_table`.
4. Is there a **payment** step that happens elsewhere (out-of-band of this
   app)? If so the migration may need a `status` column and an idempotency
   key.
5. Should `m_available = 0` movies remain in `movie_table` (soft-hide
   model, current behaviour) or be archived?
6. What is the desired **timezone / showtime model** if `movie_table`
   represents physical showings?

## 8.4 Mapping legacy → target platform

This is suggested mapping only; the implementation tickets should refine
it.

| Legacy concern              | Target platform                                          | Notes |
|-----------------------------|----------------------------------------------------------|-------|
| HTTP listener `/api/*`      | ASP.NET minimal-API or controller in `src/app/MovieApi`  | `src/app/MovieApi.slnx` is the migration target. |
| APIKit routing via RAML     | OpenAPI 3 spec generated from controller, or hand-written; remove RAML dependency on Exchange. | Commit the spec to `src/docs/contracts/`. |
| MySQL on `remotemysql.com`  | Azure SQL / SQL Server via `src/sql`                     | See `src/sql/README.md` and EF migrations. |
| Mule DB connector           | EF Core (or Dapper) with a single transaction per booking. | Closes D1–D3. |
| `validation:is-true` rule   | Domain service / handler with explicit unit tests.       | Closes D5 too. |
| DataWeave identity transform| DTO objects + `System.Text.Json` (or `Newtonsoft.Json`). | Closes D10 by aligning DTO with implementation. |
| `config.yaml` properties    | `appsettings.json` + Azure Key Vault for secrets, configured via `src/bicep/main.bicep`. | Closes D8. |
| CloudHub deployment         | Azure App Service / Container Apps via `src/bicep/`.     | See `src/bicep/README.md`. |
| MUnit (absent)              | xUnit/NUnit tests under `src/app`, run via `dotnet test src/app/MovieApi.slnx`. | See repo memory: build/test commands are documented. |

## 8.5 Assumptions used in this documentation

Stated explicitly so future readers can challenge them:

- **A1** — The flow names `get:\movies:movie-config` and
  `post:\movies\(m_id):movie-config` correspond to RAML paths `/movies` and
  `/movies/{m_id}` and only those two operations exist. Other operations
  cannot exist because no other matching flows are wired in
  `interface.xml`.
- **A2** — `m_id` is numeric in the database (it is cast `as Number` in
  every DB-bound expression) and string in the HTTP layer (it is in the
  URI path).
- **A3** — `o_id` is an auto-increment primary key on `order_table`.
  Otherwise the `MAX(o_id)`-based confirmation read could not function at
  all.
- **A4** — `m_available` is a non-negative integer. The validation rule
  `>= 0` would be meaningless otherwise.
- **A5** — Pricing is in a single, undeclared currency. There is no scale
  information; values are integers.
- **A6** — The Anypoint Exchange RAML is consistent with the implemented
  flow names and parameter shapes. Where the auto-generated type hints
  diverge from the implementation, the **implementation** wins (this
  matches normal production reality — clients depend on actual behaviour).

If any of A1–A6 is wrong, the affected sections of this documentation set
must be revisited.
