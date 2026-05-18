# Flow: movie-console

- **Source**: `mulesoft/src/main/mule/interface.xml`
- **Trigger/Endpoint**: HTTP Listener `path="/console/*"`
- **Primary role**: Serve APIKit console.

## Inputs
- Console HTTP request attributes.

## Outputs
- API console response with default `200` status.

## Business Rules / Transformations
- Calls `apikit:console`.
- Handles `APIKIT:NOT_FOUND` with JSON `{ "message": "Resource not found" }` and status `404`.

## Dependencies
- `movie-httpListenerConfig`
- `movie-config` APIKit config
