# External Dependencies, Config, and Cross-Cutting Concerns

## External Systems
- **MySQL database**
  - Host/port/db from `config.yaml`
  - Used by `GetMovies` and `BookTickets`
  - Connector: `mule-db-connector` + `mysql-connector-java`
- **Anypoint Exchange RAML asset**
  - APIKit references: `dd352549-b3e6-4dd6-b86f-85ed018825af:movie:1.0.0:raml:zip`
  - Required to resolve API spec; currently missing in local Maven resolution
- **CloudHub/Anypoint Platform**
  - Deployment placeholders in `pom.xml` (`anypoint.username`, `anypoint.password`, etc.)

## Config and Secrets
- `config.yaml` includes:
  - `http.port`
  - `db.host`, `db.port`, `db.user`, `db.pass`, `db.database`
- **Risk**: credentials are plaintext in source and should be moved to secure secret storage (for example: Azure Key Vault + app settings) during modernization.

## Logging
- Config file: `mulesoft/src/main/resources/log4j2.xml`
- Output log: `${mule.home}/logs/bookmyshow.log`
- Logger levels:
  - HTTP internals at WARN
  - Mule logger processor at INFO
  - Async root at INFO

## Authentication/Authorization
- No auth policy enforcement found in Mule flows (no OAuth/JWT/basic auth processors).
- API security appears to rely on network/perimeter controls outside this codebase.

## Error Handling Strategy
- APIKit-level error mapping in `movie-main` and `movie-console` with explicit HTTP status variables.
- Business validation in `BookTickets` handled via `VALIDATION:INVALID_BOOLEAN` and `on-error-continue` response payload.
