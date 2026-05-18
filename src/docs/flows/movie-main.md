# Flow: movie-main

- **Source**: `mulesoft/src/main/mule/interface.xml`
- **Trigger/Endpoint**: HTTP Listener `path="/api/*"`
- **Primary role**: API gateway flow for APIKit routing.

## Inputs
- HTTP request payload (passed through to APIKit route flow)
- Attributes used by downstream route flows (`uriParams`, `queryParams`)
- Variables:
  - `httpStatus` (defaulted to 200/500 in listener responses)
  - `outboundHeaders` (defaulted to `{}`)

## Outputs
- HTTP response from routed flow (`GetMovies` or `BookTickets`), including propagated status/headers.

## Business Rules / Transformations
- Delegates to `apikit:router`.
- Error handler maps APIKit exceptions to JSON messages and status codes:
  - `APIKIT:BAD_REQUEST` -> 400
  - `APIKIT:NOT_FOUND` -> 404
  - `APIKIT:METHOD_NOT_ALLOWED` -> 405
  - `APIKIT:NOT_ACCEPTABLE` -> 406
  - `APIKIT:UNSUPPORTED_MEDIA_TYPE` -> 415
  - `APIKIT:NOT_IMPLEMENTED` -> 501

## Dependencies
- `movie-httpListenerConfig`
- `movie-config` APIKit config (RAML in Exchange)
