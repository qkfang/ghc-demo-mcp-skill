# Flow: GetMovies

- **Source**: `mulesoft/src/main/mule/implementation.xml`
- **Invoked by**: `get:\movies:movie-config`
- **Primary role**: Fetch currently available movies.

## Inputs
- No request body required.

## Outputs
- JSON payload containing DB rows from `movie_table` where availability is positive.

## Business Rules / Transformations
1. Execute SQL:
   - `select * from movie_table where m_available > 0`
2. DataWeave transform returns `payload` as `application/json`.
3. Logs at INFO via `<logger/>`.

## External Dependencies
- MySQL via `Database_Config`.
