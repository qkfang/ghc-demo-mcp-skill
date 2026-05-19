# SQL Schema (EF Core)

This folder contains a standalone EF Core code-first project for the modernized relational model.

## Prerequisites

- .NET SDK 10.x
- SQL Server LocalDB/SQL Server/Azure SQL
- EF Core CLI (`dotnet tool install --global dotnet-ef`)

## Project build

```bash
dotnet build /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/sql/GhcDemo.Sql.csproj
```

## Create or update migration

```bash
cd /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/sql
dotnet ef migrations add InitialCreate
```

## Apply migration to a database

```bash
cd /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/sql
dotnet ef database update --connection "Server=<server>;Database=ghc_demo;User Id=<user>;Password=<password>;TrustServerCertificate=True"
```

## Generate SQL from migrations

```bash
cd /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/sql
dotnet ef migrations script --idempotent --output scripts/schema.sql
```

## Seed data

Seed data is defined in `Data/SeedData.cs` (via `HasData`) and emitted in migrations.

To apply the standalone seed script manually:

```sql
:r ./scripts/seed.sql
```

Or run with sqlcmd:

```bash
sqlcmd -S <server> -d ghc_demo -i /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/sql/scripts/seed.sql
```
