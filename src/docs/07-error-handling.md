# 7. Error Handling and HTTP Status Mapping

Errors are handled in two places:

1. **The `movie-main` flow's global error handler** in
   `mulesoft/src/main/mule/interface.xml:15-94` — handles APIKit dispatch
   errors before any business flow runs.
2. **The `BookTickets` flow's local error handler** in
   `mulesoft/src/main/mule/implementation.xml:62-74` — handles the
   `VALIDATION:INVALID_BOOLEAN` thrown by the availability check.

The HTTP listener returns `vars.httpStatus` as the response status code,
defaulting to `200` on success and `500` on `http:error-response`
(`interface.xml:6-12, 98-104`).

## 7.1 APIKit dispatch errors (global handler)

These are raised by `apikit:router` *before* a business flow runs (e.g.
wrong path, wrong method, malformed request matching the RAML). All are
mapped to JSON with `{ "message": "<text>" }` and a specific HTTP status:

| Error type                       | Status | Body                                       |
|----------------------------------|--------|--------------------------------------------|
| `APIKIT:BAD_REQUEST`             | 400    | `{ "message": "Bad request" }`             |
| `APIKIT:NOT_FOUND`               | 404    | `{ "message": "Resource not found" }`      |
| `APIKIT:METHOD_NOT_ALLOWED`      | 405    | `{ "message": "Method not allowed" }`      |
| `APIKIT:NOT_ACCEPTABLE`          | 406    | `{ "message": "Not acceptable" }`          |
| `APIKIT:UNSUPPORTED_MEDIA_TYPE`  | 415    | `{ "message": "Unsupported media type" }`  |
| `APIKIT:NOT_IMPLEMENTED`         | 501    | `{ "message": "Not Implemented" }`         |

All handlers use `on-error-propagate`, meaning the error continues to
propagate after the response payload/status are set, which causes the
HTTP listener to use `http:error-response` (the body and headers are
identical to `http:response`).

Each handler sets `vars.httpStatus` *and* sets the payload via a DataWeave
transform (see [`06-transformations.md`](./06-transformations.md#68-apikit-error-handler-transforms-interfacexml)).

## 7.2 `BookTickets` validation rejection

Source — `mulesoft/src/main/mule/implementation.xml:62-74`:

```xml
<on-error-continue type="VALIDATION:INVALID_BOOLEAN">
  <ee:transform>  <!-- output application/java; --- payload -->
  </ee:transform>
  <set-payload value='#[output application/json
                       ---
                       { "error": "avaible tickets is only $(payload[0].m_available)
                          but you have ordered $(vars.no_tickets.no_tickets)" }]' />
</on-error-continue>
```

Behaviour:

- **HTTP status: 200**. Because this is `on-error-continue`, the error is
  swallowed, so `vars.httpStatus` is never set by an error path. The HTTP
  listener falls through to its default of `200`.
- **Body**:
  `{ "error": "avaible tickets is only <m_available> but you have ordered <no_tickets>" }`.
  Typo (`avaible`) preserved.

Migration note: returning `200` for a business-rule failure is unusual and
arguably wrong; clients cannot distinguish success from rejection without
parsing the body. See `08-migration-notes.md` for a recommendation to map
this to `409 Conflict` or `400 Bad Request` in the new implementation.

## 7.3 Unhandled errors

Any error not covered by §7.1 or §7.2 — for example:

- A MySQL connection failure mid-flow.
- An `UPDATE` or `INSERT` SQL error (constraint violation, deadlock,
  truncation, etc.).
- A `NullPointerException` when `payload[0]` is empty because `m_id` was
  not found in `movie_table`.

…is **not** caught by any flow handler. It bubbles back through the APIKit
router and the HTTP listener. The listener's `http:error-response` sets
status to `vars.httpStatus default 500` and writes `#[payload]` as the
body, so the raw Mule error payload is returned to the client. This is a
defect — error responses should be sanitised before leaving the boundary.

## 7.4 `movie-console` flow

Source — `mulesoft/src/main/mule/interface.xml:96-122`. The console flow
has a single `APIKIT:NOT_FOUND` handler identical in structure to §7.1.
No business meaning.

## 7.5 Status-code map (cheat sheet for migration)

| Scenario                                           | Legacy status | Legacy body                                                                                       | Notes for migration             |
|----------------------------------------------------|---------------|---------------------------------------------------------------------------------------------------|---------------------------------|
| `GET /api/movies` success                          | 200           | JSON array of available movies                                                                    |                                 |
| `POST /api/movies/{m_id}?no_tickets=N` success     | 200           | JSON array containing the inserted order row                                                      | Consider returning a single object (not array) and 201 |
| Booking rejected (insufficient seats)              | **200**       | `{ "error": "avaible tickets is only X but you have ordered Y" }`                                | Migrate to **409** or **400**; fix typo |
| Path not in RAML / method not allowed              | 404 / 405     | `{ "message": "..." }`                                                                            |                                 |
| Body type wrong / accept header wrong              | 400/406/415   | `{ "message": "..." }`                                                                            |                                 |
| Database/transport failure                         | 500           | Raw Mule error payload                                                                            | Migrate to structured error body |
