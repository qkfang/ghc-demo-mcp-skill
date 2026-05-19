# Azure Infrastructure (Bicep)

This folder contains Bicep templates to provision the Azure infrastructure for the modernized application.

## What gets deployed

- Azure Function App on Linux Consumption plan (configured for .NET isolated worker)
- Storage Account for Function runtime state
- Application Insights + Log Analytics workspace
- Azure SQL Server + SQL Database
- Key Vault using RBAC authorization
- User-assigned managed identity
- RBAC assignment so the Function managed identity can read Key Vault secrets

## File layout

- `main.bicep` - top-level resource group deployment
- `modules/identity.bicep` - user-assigned managed identity
- `modules/monitoring.bicep` - Log Analytics + Application Insights
- `modules/sql.bicep` - SQL Server + SQL Database
- `modules/keyVault.bicep` - Key Vault + secret creation + RBAC assignment
- `modules/functionApp.bicep` - Function App + hosting plan + Storage Account
- `main.dev.bicepparam` - development parameters
- `main.prod.bicepparam` - production parameters

## Prerequisites

- Azure CLI (`az`) installed and logged in
- Bicep CLI installed (`az bicep install`)
- Target resource group already created
- SQL admin password provided as an environment variable

```bash
export AZURE_SQL_ADMIN_PASSWORD='<strong-password>'
```

## Validate template

```bash
bicep build src/bicep/main.bicep
```

## Dry-run deployment (`what-if`)

Because `main.bicep` is resource-group scope, use `az deployment group what-if`:

```bash
az deployment group what-if \
  --resource-group <resource-group-name> \
  --name ghcdemo-dev-whatif \
  --template-file src/bicep/main.bicep \
  --parameters src/bicep/main.dev.bicepparam
```

For production parameters:

```bash
az deployment group what-if \
  --resource-group <resource-group-name> \
  --name ghcdemo-prod-whatif \
  --template-file src/bicep/main.bicep \
  --parameters src/bicep/main.prod.bicepparam
```

## Deploy

```bash
az deployment group create \
  --resource-group <resource-group-name> \
  --name ghcdemo-dev \
  --template-file src/bicep/main.bicep \
  --parameters src/bicep/main.dev.bicepparam
```
