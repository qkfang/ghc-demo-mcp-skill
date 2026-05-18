@description('Azure region for the Function App.')
param location string

@description('Function App name.')
param functionAppName string

@description('App Service plan resource ID.')
param appServicePlanId string

@description('Application Insights connection string.')
param appInsightsConnectionString string

@description('Key Vault URI.')
param keyVaultUri string

@description('Key Vault secret name for storage connection string.')
param storageConnectionStringSecretName string

@description('Key Vault secret name for SQL connection string.')
param sqlConnectionStringSecretName string

@description('Log Analytics workspace resource ID.')
param logAnalyticsWorkspaceId string

@description('Tags applied to Function App resources.')
param tags object

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlanId
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|10.0'
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: '@Microsoft.KeyVault(SecretUri=${keyVaultUri}secrets/${storageConnectionStringSecretName}/)'
        }
        {
          name: 'SqlConnectionString'
          value: '@Microsoft.KeyVault(SecretUri=${keyVaultUri}secrets/${sqlConnectionStringSecretName}/)'
        }
      ]
    }
  }
}

resource functionAppDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: '${functionAppName}-diag'
  scope: functionApp
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

output functionAppName string = functionApp.name
output principalId string = functionApp.identity.principalId
