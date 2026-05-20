# Movie API migration notes

## Migration baseline reviewed

- Legacy implementation:
  - `mulesoft/src/main/mule/interface.xml`
  - `mulesoft/src/main/mule/implementation.xml`
- Legacy project metadata:
  - `mulesoft/pom.xml`
- Legacy presentation artifact:
  - `docs/Mini Project Presentation.pptx`

## Recreated APIs under `src/app`

- `GET /api/movies`
  - Mirrors Mule `GetMovies` by returning only records where `m_available > 0`.
- `POST /api/movies/{m_id}?no_tickets={count}`
  - Mirrors Mule `BookTickets` pricing tiers and insufficient-ticket error behavior.
  - Preserves `Bad request` and `Resource not found` response message formats.

## Assumptions and known gaps

- Mule RAML package from Exchange is unavailable in this repository, so route and validation behavior were inferred from the Mule flows.
- Database operations were replaced with an in-memory repository and a clear `IMovieRepository` abstraction for future SQL/infrastructure integration.
- API security/auth and deployment infrastructure are intentionally deferred to follow-on issues.
