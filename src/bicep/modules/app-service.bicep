param location string
param tags object = {}
param appServicePlanName string
param webAppName string
param appServicePlanSkuName string
param appServicePlanSkuCapacity int = 1
param linuxFxVersion string
param appServiceAlwaysOn bool = false
param applicationInsightsConnectionString string
param keyVaultUri string
param sqlServerFullyQualifiedDomainName string
param sqlDatabaseName string

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: appServicePlanSkuName
    capacity: appServicePlanSkuCapacity
  }
  kind: 'linux'
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: linuxFxVersion
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      alwaysOn: appServiceAlwaysOn
      appSettings: [
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsightsConnectionString
        }
        {
          name: 'KeyVault__VaultUri'
          value: keyVaultUri
        }
        {
          name: 'Sql__Server'
          value: sqlServerFullyQualifiedDomainName
        }
        {
          name: 'Sql__Database'
          value: sqlDatabaseName
        }
        {
          name: 'Sql__ConnectionStringSecretName'
          value: 'SqlConnectionString'
        }
        {
          name: 'ConnectionStrings__MovieDb'
          value: '@Microsoft.KeyVault(SecretUri=${keyVaultUri}secrets/SqlConnectionString/)'
        }
      ]
    }
  }
}

output appServicePlanName string = appServicePlan.name
output webAppDefaultHostName string = webApp.properties.defaultHostName
output webAppName string = webApp.name
output webAppPrincipalId string = webApp.identity.principalId
