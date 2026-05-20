# Bicep deployment assets

This directory contains the initial Azure infrastructure definition for the migrated movie API and its planned relational data layer.

## What is deployed

The initial template provisions:

- a Log Analytics workspace
- an Application Insights instance for API telemetry
- a Linux App Service plan
- a Linux Web App with a system-assigned managed identity
- an Azure SQL logical server and single database
- a Key Vault that stores the SQL connection string and grants the Web App `get`/`list` access to secrets

## Why these resources

The current MuleSoft implementation exposes:

- `GET /api/movies` to read currently available movies
- `POST /api/movies/{m_id}?no_tickets=...` to create bookings and update remaining inventory

Those flows currently read and write relational tables (`movie_table` and `order_table`), so this first-pass Azure baseline assumes:

1. the migrated API will run on App Service
2. the migrated data layer will land on Azure SQL Database
3. configuration and connection strings should move into Key Vault
4. observability should be available from the first deployment

## Assumptions to reconcile later

These templates are intentionally conservative and should be revisited once the application and schema tickets are available:

- If the migrated API does **not** target App Service, update `modules/app-service.bicep`.
- If the planned data layer remains MySQL or moves to another engine, replace `modules/sql.bicep`.
- The Web App settings currently expose a generic SQL connection convention; align the final setting names with the app ticket outputs.
- SQL networking currently uses firewall rules only. Private endpoints, VNet integration, and managed identity database auth can be layered in later.

## Structure

```text
src/bicep/
├── main.bicep
├── modules/
│   ├── app-service.bicep
│   ├── key-vault.bicep
│   ├── monitoring.bicep
│   └── sql.bicep
└── parameters/
    ├── dev.bicepparam
    ├── prod.bicepparam
    └── test.bicepparam
```

## Parameters and environments

`main.bicep` contains the shared deployment shape. Environment-specific non-secret defaults live under `parameters/`.

The checked-in parameter files include placeholder SQL credentials only so that `bicep build-params` can validate successfully in CI. Override them during real deployments.

The following values should be supplied securely at deployment time:

- `sqlAdministratorLogin`
- `sqlAdministratorPassword`

## Validation

```bash
bicep build /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/bicep/main.bicep
bicep build-params /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/bicep/parameters/dev.bicepparam
```

## Example deployment

```bash
az deployment group create \
  --resource-group <resource-group> \
  --template-file /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/bicep/main.bicep \
  --parameters /home/runner/work/ghc-demo-mcp-skill/ghc-demo-mcp-skill/src/bicep/parameters/dev.bicepparam \
  --parameters sqlAdministratorLogin=<sql-admin-login> \
  --parameters sqlAdministratorPassword=<sql-admin-password>
```
