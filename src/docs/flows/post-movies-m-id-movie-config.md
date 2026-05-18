# Flow: post:\movies\(m_id):movie-config

- **Source**: `mulesoft/src/main/mule/interface.xml`
- **Trigger/Endpoint**: Routed by APIKit for **POST `/movies/{m_id}`** under `/api/*`.
- **Primary role**: Route-to-implementation shim.

## Inputs
- Path parameter `m_id`
- Query parameter `no_tickets`

## Outputs
- Forwards output from `BookTickets`.

## Business Rules / Transformations
- No transformation in this flow.
- Single `flow-ref` to `BookTickets`.

## Dependencies
- `BookTickets` implementation flow.
