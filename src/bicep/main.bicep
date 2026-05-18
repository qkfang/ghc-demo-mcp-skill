targetScope = 'subscription'

@description('Deployment environment.')
@allowed([
  'dev'
  'test'
  'prod'
])
param environmentName string

@description('Azure region for the resource group and resources.')
param location string = deployment().location

@description('Workload short name used as part of resource naming.')
param workloadName string = 'ghcdemo'

@description('Resource group name for the workload resources.')
param resourceGroupName string = 'rg-${workloadName}-${environmentName}'

@description('Owner tag value.')
param owner string

@description('Storage account SKU.')
@allowed([
  'Standard_LRS'
  'Standard_GRS'
  'Standard_ZRS'
])
param storageSkuName string = 'Standard_LRS'

@description('App Service plan SKU name.')
param appServicePlanSkuName string = 'FC1'

@description('Azure SQL Database SKU name.')
param sqlDatabaseSkuName string = 'Basic'

@description('Administrator login for Azure SQL Server.')
param sqlAdminLogin string

@description('Administrator password for Azure SQL Server.')
@secure()
param sqlAdminPassword string

var normalizedPrefix = toLower(replace('${workloadName}${environmentName}', '-', ''))
var uniqueSuffix = toLower(uniqueString(subscription().id, resourceGroupName, workloadName, environmentName))
var functionAppName = toLower(take('func-${workloadName}-${environmentName}-${uniqueSuffix}', 60))
var storageName = toLower(take('st${normalizedPrefix}${uniqueSuffix}', 24))
var appServicePlanName = toLower('plan-${workloadName}-${environmentName}')
var workspaceName = toLower('law-${workloadName}-${environmentName}')
var appInsightsName = toLower('appi-${workloadName}-${environmentName}')
var sqlServerName = toLower('sql-${workloadName}-${environmentName}-${take(uniqueSuffix, 6)}')
var sqlDatabaseName = toLower('sqldb-${workloadName}-${environmentName}')
var keyVaultName = toLower(take('kv-${workloadName}-${environmentName}-${take(uniqueSuffix, 6)}', 24))
var tags = {
  env: environmentName
  workload: workloadName
  owner: owner
}

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: {
    location: location
    workspaceName: workspaceName
    appInsightsName: appInsightsName
    tags: tags
  }
}

module storage './modules/storage.bicep' = {
  name: 'storage'
  scope: rg
  params: {
    location: location
    storageAccountName: storageName
    storageSkuName: storageSkuName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    tags: tags
  }
}

module plan './modules/plan.bicep' = {
  name: 'plan'
  scope: rg
  params: {
    location: location
    appServicePlanName: appServicePlanName
    appServicePlanSkuName: appServicePlanSkuName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    tags: tags
  }
}

module sql './modules/sql.bicep' = {
  name: 'sql'
  scope: rg
  params: {
    location: location
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    sqlDatabaseSkuName: sqlDatabaseSkuName
    sqlAdminLogin: sqlAdminLogin
    sqlAdminPassword: sqlAdminPassword
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    tags: tags
  }
}

resource functionStorageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  scope: rg
  name: storageName
}

var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageName};AccountKey=${functionStorageAccount.listKeys().keys[0].value};EndpointSuffix=${az.environment().suffixes.storage}'
var sqlConnectionString = 'Server=tcp:${sql.outputs.sqlServerFqdn},1433;Initial Catalog=${sql.outputs.sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

module keyVault './modules/keyvault.bicep' = {
  name: 'keyvault'
  scope: rg
  params: {
    location: location
    keyVaultName: keyVaultName
    storageConnectionStringSecretName: 'storage-connection-string'
    storageConnectionStringSecretValue: storageConnectionString
    sqlConnectionStringSecretName: 'sql-connection-string'
    sqlConnectionStringSecretValue: sqlConnectionString
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    tags: tags
  }
}

module functionApp './modules/functionApp.bicep' = {
  name: 'functionApp'
  scope: rg
  params: {
    location: location
    functionAppName: functionAppName
    appServicePlanId: plan.outputs.appServicePlanId
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    keyVaultUri: keyVault.outputs.keyVaultUri
    storageConnectionStringSecretName: keyVault.outputs.storageConnectionStringSecretName
    sqlConnectionStringSecretName: keyVault.outputs.sqlConnectionStringSecretName
    logAnalyticsWorkspaceId: monitoring.outputs.logAnalyticsWorkspaceId
    tags: tags
  }
}

module rbac './modules/rbac.bicep' = {
  name: 'rbac'
  scope: rg
  params: {
    principalId: functionApp.outputs.principalId
    keyVaultName: keyVault.outputs.keyVaultName
    sqlServerName: sql.outputs.sqlServerName
  }
}

output functionAppName string = functionApp.outputs.functionAppName
output appInsightsConnectionString string = monitoring.outputs.appInsightsConnectionString
output sqlServerFqdn string = sql.outputs.sqlServerFqdn
output keyVaultUri string = keyVault.outputs.keyVaultUri
