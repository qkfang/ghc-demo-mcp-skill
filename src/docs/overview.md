# Legacy MuleSoft Overview

## Context
The Mule app (`mulesoft/`) exposes a REST API for movie inventory and ticket booking using APIKit routing and a MySQL backend.

## Runtime Components
- **HTTP listener config**: `movie-httpListenerConfig` on `0.0.0.0:${http.port}`
- **APIKit config**: `movie-config` (RAML artifact in Exchange)
- **DB config**: `Database_Config` (`db.host`, `db.port`, `db.user`, `db.pass`, `db.database`)
- **Properties source**: `src/main/resources/config.yaml`

## Flow Topology
```mermaid
flowchart TD
  C[Client] --> A[/api/* listener\nmovie-main]
  C --> K[/console/* listener\nmovie-console]
  A --> B[APIKit Router]
  B --> D[GET /movies]
  B --> E[POST /movies/{m_id}]
  D --> F[GetMovies]
  E --> G[BookTickets]
  F --> H[(movie_table)]
  G --> H
  G --> I[(order_table)]
  A --> J[APIKit error mappings\n400/404/405/406/415/501]
```

## Main Business Capability
1. **List available movies**: read all rows where `m_available > 0`.
2. **Book tickets**: validate requested tickets against availability, insert an order, decrement availability, return latest order row.

## Cross-Cutting Notes
- Logging is file-based (`bookmyshow.log`) with INFO root level.
- No explicit auth/authorization policy is implemented in Mule flows.
- DB credentials are currently plaintext in `config.yaml` and should be externalized during migration.
