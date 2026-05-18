# Flow: BookTickets

- **Source**: `mulesoft/src/main/mule/implementation.xml`
- **Invoked by**: `post:\movies\(m_id):movie-config`
- **Primary role**: Validate and create a movie ticket order.

## Inputs
- `attributes.uriParams.m_id`
- `attributes.queryParams.no_tickets`
- Stored as variable `vars.no_tickets = { no_tickets, m_id }`

## Outputs
- Success: latest order row from `order_table` as JSON.
- Validation failure: JSON error message with available vs requested seats.

## Business Rules / Transformations
1. Read availability:
   - `select m_available from movie_table where m_id = :m_id`
2. Validate stock:
   - `(payload[0].m_available - vars.no_tickets.no_tickets) >= 0`
3. Price calculation for insert:
   - `<= 5` tickets: `qty * 100`
   - `<= 10` tickets: `qty * 90`
   - `> 10` tickets: `qty * 80`
4. Insert order into `order_table (m_id, no_tickets, price)`.
5. Update inventory:
   - `update movie_table set m_available = m_available - :no_tickets where m_id = :m_id`
6. Read latest order:
   - `select * from order_table WHERE o_id = (SELECT MAX(o_id) FROM order_table)`
7. Transform output to JSON.

## Error Handling
- `on-error-continue` for `VALIDATION:INVALID_BOOLEAN`:
  - keeps flow from failing hard
  - returns JSON: `{"error": "avaible tickets is only ..."}`

## External Dependencies
- MySQL via `Database_Config`.
