targetScope = 'resourceGroup'

@description('Deployment environment name (for example: dev, test, prod).')
param environmentName string

@description('Azure region for all resources.')
param location string = resourceGroup().location

@description('Workload name used as the naming prefix.')
param workloadName string = 'ghcdemo'

@description('Common tags applied to all resources.')
param tags object = {
  'azd-env-name': environmentName
  'azd-service-name': 'api'
  'managed-by': 'bicep'
}

@description('SQL administrator login name.')
param sqlAdministratorLogin string = 'sqladminuser'

@description('SQL administrator password.')
@secure()
param sqlAdministratorPassword string

@description('SQL database name.')
param sqlDatabaseName string = 'appdb'

var uniqueSuffix = toLower(take(uniqueString(subscription().id, resourceGroup().id, workloadName, environmentName), 6))
var baseName = toLower('${workloadName}-${environmentName}')
var compactBaseName = toLower(replace(baseName, '-', ''))

var managedIdentityName = toLower(take('id-${baseName}-${uniqueSuffix}', 128))
var logAnalyticsWorkspaceName = toLower(take('log-${baseName}-${uniqueSuffix}', 63))
var appInsightsName = toLower(take('appi-${baseName}-${uniqueSuffix}', 64))
var sqlServerName = toLower(take('sql-${compactBaseName}-${uniqueSuffix}', 63))
var keyVaultName = toLower(take('kv-${compactBaseName}-${uniqueSuffix}', 24))
var functionAppName = toLower(take('func-${baseName}-${uniqueSuffix}', 60))
var functionPlanName = toLower(take('plan-${baseName}-${uniqueSuffix}', 40))
var storageAccountName = toLower(take(replace('st${compactBaseName}${uniqueSuffix}', '-', ''), 24))

var sqlConnectionSecretName = 'sql-connection-string'
var storageConnectionSecretName = 'azure-webjobs-storage'

module identity './modules/identity.bicep' = {
  name: 'identity-${uniqueSuffix}'
  params: {
    name: managedIdentityName
    location: location
    tags: tags
  }
}

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring-${uniqueSuffix}'
  params: {
    location: location
    workspaceName: logAnalyticsWorkspaceName
    appInsightsName: appInsightsName
    tags: tags
  }
}

module sql './modules/sql.bicep' = {
  name: 'sql-${uniqueSuffix}'
  params: {
    location: location
    serverName: sqlServerName
    databaseName: sqlDatabaseName
    administratorLogin: sqlAdministratorLogin
    administratorPassword: sqlAdministratorPassword
    tags: tags
  }
}

var sqlConnectionString = 'Server=tcp:${sql.outputs.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

module keyVault './modules/keyVault.bicep' = {
  name: 'kv-${uniqueSuffix}'
  params: {
    keyVaultName: keyVaultName
    location: location
    tags: tags
    managedIdentityPrincipalId: identity.outputs.principalId
    sqlConnectionSecretName: sqlConnectionSecretName
    sqlConnectionString: sqlConnectionString
  }
}

module functionApp './modules/functionApp.bicep' = {
  name: 'function-${uniqueSuffix}'
  params: {
    location: location
    functionAppName: functionAppName
    hostingPlanName: functionPlanName
    storageAccountName: storageAccountName
    userAssignedIdentityId: identity.outputs.resourceId
    appInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    keyVaultUri: keyVault.outputs.keyVaultUri
    sqlConnectionSecretName: sqlConnectionSecretName
    storageConnectionSecretName: storageConnectionSecretName
    tags: tags
  }
}

resource keyVaultResource 'Microsoft.KeyVault/vaults@2023-07-01' existing = {
  name: keyVaultName
}

resource functionStorageConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  name: storageConnectionSecretName
  parent: keyVaultResource
  properties: {
    value: functionApp.outputs.storageConnectionString
  }
}

output functionAppResourceName string = functionApp.outputs.functionAppName
output keyVaultResourceName string = keyVault.outputs.keyVaultName
output sqlServerResourceName string = sql.outputs.serverName
output applicationInsightsResourceName string = monitoring.outputs.appInsightsName
output userAssignedManagedIdentityResourceName string = identity.outputs.name
