# GhcDemo.Sql

Standalone Entity Framework Core schema artifacts derived from the legacy MuleSoft movie booking flows.

## Scope

This project intentionally covers only database schema and development seed data. It does **not** integrate with any application code yet.

## Legacy model captured

- `movie_table`
  - `m_id` (primary key)
  - `m_available`
- `order_table`
  - `o_id` (primary key)
  - `m_id` (foreign key to `movie_table.m_id`)
  - `no_tickets`
  - `price`

## Seed data

The initial seed data covers the core development scenarios visible in MuleSoft:

- a high-availability movie (`m_available = 150`)
- a low-stock movie (`m_available = 4`)
- a sold-out movie (`m_available = 0`)
- example orders that match the legacy pricing tiers (`2 x 100`, `6 x 90`)

## Commands

```bash
cd /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/sql
dotnet build GhcDemo.Sql.csproj
dotnet tool restore
dotnet ef database update
```

## Assumptions and open questions

- Only columns directly evidenced by the MuleSoft flows were included. No extra descriptive movie metadata has been invented yet.
- A foreign key from `order_table.m_id` to `movie_table.m_id` was added because the booking flow always inserts orders against a movie id.
- Check constraints were added to prevent negative availability, non-positive ticket counts, and negative prices.
- The legacy flow computes pricing in application logic. If future work needs tier rules inside the database, that should be designed separately.
- The legacy flow fetches the most recent order with `MAX(o_id)`. If ordering semantics matter later, an explicit timestamp column may be needed in a future ticket.
