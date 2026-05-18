# DataWeave Transformations Inventory

## Inline DataWeave in `interface.xml`
- APIKit error payload mappers in `movie-main` and `movie-console`:
  - `{message: "Bad request"}` -> 400
  - `{message: "Resource not found"}` -> 404
  - `{message: "Method not allowed"}` -> 405
  - `{message: "Not acceptable"}` -> 406
  - `{message: "Unsupported media type"}` -> 415
  - `{message: "Not Implemented"}` -> 501

## Inline DataWeave in `implementation.xml`
- `GetMovies` transform:
  - `output application/json`
  - `--- payload` (pass-through DB result)
- `BookTickets` success transform:
  - `output application/json`
  - `--- payload` (pass-through latest order select result)
- `BookTickets` validation-error transform block:
  - initial transform outputs Java payload pass-through
  - followed by inline expression setting JSON error payload with interpolated availability/requested quantities

## DataWeave Type Definitions in `application-types.xml`
- Auto-generated type metadata for:
  - flow input/output payloads and HTTP attributes
  - flow variables (`outboundHeaders`, `httpStatus`)
- Named domain types:
  - `availableSeats` (array of strings, example in `sample.json`)
  - `odder_details` (`no_tickets`, `m_id`, example in `order_details.json`)

## Files under `src/main/resources/weave/`
- Contains auto-generated `.wev` DataWeave type declarations for APIKit metadata:
  - `.../4c5336f2...` (GetMovies route metadata)
  - `.../2df5fe63...` (BookTickets route metadata)
- No additional custom business transformation scripts were found in this folder.
