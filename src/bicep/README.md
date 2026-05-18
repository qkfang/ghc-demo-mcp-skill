# Bicep infrastructure for Function App modernization

This folder contains subscription-scope Bicep templates to provision the Azure resources required for the .NET 10 isolated Function App workload.

## Structure

- `main.bicep` - entry point (creates resource group and orchestrates modules)
- `main.dev.bicepparam` / `main.test.bicepparam` / `main.prod.bicepparam` - environment parameter files
- `modules/` - resource modules (`storage`, `plan`, `functionApp`, `monitoring`, `sql`, `keyvault`, `rbac`)

## Prerequisites

- Azure CLI (`az`) with Bicep support (`az bicep version`)
- Logged into a subscription with permissions to create resource groups and role assignments

## Build

```bash
bicep build src/bicep/main.bicep
```

Set SQL admin password before `what-if`/deployment:

```bash
export SQL_ADMIN_PASSWORD='<strong-password>'
```

## What-if (subscription scope)

```bash
az deployment sub what-if \
  --name ghcdemo-dev-whatif \
  --location eastus \
  --template-file src/bicep/main.bicep \
  --parameters src/bicep/main.dev.bicepparam
```

## Deploy

```bash
az deployment sub create \
  --name ghcdemo-dev \
  --location eastus \
  --template-file src/bicep/main.bicep \
  --parameters src/bicep/main.dev.bicepparam
```

## Notes

- Secrets are written to Key Vault (`storage-connection-string`, `sql-connection-string`) and referenced by the Function App via Key Vault references.
- Managed identity RBAC is configured for Key Vault secret reads and SQL control-plane access.
- Diagnostic settings route logs/metrics from provisioned resources to Log Analytics.
- This template is subscription-scope because it can create the workload resource group.
