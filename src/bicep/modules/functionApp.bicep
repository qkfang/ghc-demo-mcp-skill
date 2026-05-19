@description('Azure region for Function resources.')
param location string

@description('Function App name.')
param functionAppName string

@description('App Service plan name.')
param hostingPlanName string

@description('Storage account name used by the Function App.')
param storageAccountName string

@description('Resource ID of the user-assigned managed identity.')
param userAssignedIdentityId string

@description('Application Insights connection string.')
param appInsightsConnectionString string

@description('Key Vault URI (for app setting references).')
param keyVaultUri string

@description('Key Vault secret name for SQL connection string.')
param sqlConnectionSecretName string

@description('Key Vault secret name for AzureWebJobsStorage connection string.')
param storageConnectionSecretName string

@description('Resource tags.')
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  tags: tags
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: hostingPlanName
  location: location
  tags: tags
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  kind: 'functionapp'
  properties: {
    reserved: true
  }
}

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  tags: tags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentityId}': {}
    }
  }
  properties: {
    serverFarmId: hostingPlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNET-ISOLATED|10.0'
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsightsConnectionString
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'AzureWebJobsStorage'
          value: '@Microsoft.KeyVault(SecretUri=${keyVaultUri}secrets/${storageConnectionSecretName})'
        }
        {
          name: 'ConnectionStrings__Default'
          value: '@Microsoft.KeyVault(SecretUri=${keyVaultUri}secrets/${sqlConnectionSecretName})'
        }
      ]
    }
  }
}

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};AccountKey=${storageAccount.listKeys().keys[0].value};EndpointSuffix=${environment().suffixes.storage}'

output functionAppName string = functionApp.name
output storageConnectionString string = storageConnectionString
