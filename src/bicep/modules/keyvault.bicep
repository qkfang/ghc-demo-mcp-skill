@description('Azure region for Key Vault.')
param location string

@description('Key Vault name.')
param keyVaultName string

@description('Storage connection string secret name.')
param storageConnectionStringSecretName string

@description('Storage connection string secret value.')
@secure()
param storageConnectionStringSecretValue string

@description('SQL connection string secret name.')
param sqlConnectionStringSecretName string

@description('SQL connection string secret value.')
@secure()
param sqlConnectionStringSecretValue string

@description('Log Analytics workspace resource ID.')
param logAnalyticsWorkspaceId string

@description('Tags applied to Key Vault resources.')
param tags object

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    enableRbacAuthorization: true
    tenantId: tenant().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    publicNetworkAccess: 'Enabled'
  }
}

resource storageConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: storageConnectionStringSecretName
  properties: {
    value: storageConnectionStringSecretValue
  }
}

resource sqlConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: sqlConnectionStringSecretName
  properties: {
    value: sqlConnectionStringSecretValue
  }
}

resource keyVaultDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: '${keyVaultName}-diag'
  scope: keyVault
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        categoryGroup: 'allLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

output keyVaultId string = keyVault.id
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output storageConnectionStringSecretName string = storageConnectionSecret.name
output sqlConnectionStringSecretName string = sqlConnectionSecret.name
