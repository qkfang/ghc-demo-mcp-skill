# Flow: get:\movies:movie-config

- **Source**: `mulesoft/src/main/mule/interface.xml`
- **Trigger/Endpoint**: Routed by APIKit for **GET `/movies`** under `/api/*`.
- **Primary role**: Route-to-implementation shim.

## Inputs
- Request resolved by APIKit route key (`GET /movies`).

## Outputs
- Forwards output from `GetMovies`.

## Business Rules / Transformations
- No transformation in this flow.
- Single `flow-ref` to `GetMovies`.

## Dependencies
- `GetMovies` implementation flow.
