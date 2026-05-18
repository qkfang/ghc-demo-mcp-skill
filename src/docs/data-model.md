# Data Model Inventory

## Database Entities (inferred from SQL)

## 1) `movie_table`
- **Used by**: `GetMovies`, `BookTickets`
- **Observed fields**:
  - `m_id` (numeric identifier; used in WHERE clause)
  - `m_available` (numeric ticket inventory)
  - Additional columns may exist (`select *` is used in `GetMovies`)
- **Relationships**:
  - `order_table.m_id` references `movie_table.m_id` logically.

## 2) `order_table`
- **Used by**: `BookTickets`
- **Observed fields**:
  - `o_id` (order identifier; max used to retrieve latest order)
  - `m_id` (movie identifier)
  - `no_tickets` (ordered quantity)
  - `price` (calculated total order price)

## API/Flow Data Contracts

## `odder_details` (from `application-types.xml`)
- `no_tickets: String`
- `m_id: String`
- Example: `{"no_tickets":"1","m_id":"2"}`

## `availableSeats` (from `application-types.xml`)
- `Array<String>`
- Example: `["1"]`

## Runtime Attributes Used
- `queryParams.no_tickets`
- `uriParams.m_id`
- `outboundHeaders`, `httpStatus` flow variables
