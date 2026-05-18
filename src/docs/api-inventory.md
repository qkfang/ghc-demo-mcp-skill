# API Inventory

Base listener path: `/api/*` (plus `/console/*` API console route)

## 1) GET `/api/movies`
- **Flow mapping**: `get:\movies:movie-config` -> `GetMovies`
- **Purpose**: Return movies with available seats.
- **Inputs**:
  - No required query/path params in implementation.
- **Processing**:
  - SQL: `select * from movie_table where m_available > 0`
- **Success response**:
  - **Status**: `200`
  - **Body**: JSON array from DB rows (`payload` passthrough)
- **Error responses (APIKit/global)**:
  - `400`, `404`, `405`, `406`, `415`, `500`, `501`

## 2) POST `/api/movies/{m_id}`
- **Flow mapping**: `post:\movies\(m_id):movie-config` -> `BookTickets`
- **Purpose**: Book tickets for a movie and create an order.
- **Inputs**:
  - Path param: `m_id` (string in APIKit metadata, cast to number in DB ops)
  - Query param: `no_tickets` (number/string, cast to number)
  - Request body: not used by implementation
- **Processing**:
  - Read availability for `m_id`
  - Validate `(m_available - no_tickets) >= 0`
  - Insert into `order_table` with tiered pricing
  - Update `movie_table` availability
  - Return latest order row
- **Success response**:
  - **Status**: `200`
  - **Body**: JSON array (selected latest order row)
- **Business validation response**:
  - **Status**: `200` (handled by `on-error-continue`)
  - **Body**: `{ "error": "avaible tickets is only ..." }`
- **Error responses (APIKit/global)**:
  - `400`, `404`, `405`, `406`, `415`, `500`, `501`

## 3) GET `/console/*`
- **Flow mapping**: `movie-console`
- **Purpose**: APIKit console endpoint for API exploration.
- **Responses**:
  - `200` success
  - `404` when console resource not found
