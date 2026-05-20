targetScope = 'resourceGroup'

@allowed([
  'dev'
  'test'
  'prod'
])
@description('Environment name used for resource naming and tags.')
param environmentName string

@description('Workload name shared by all resources for this deployment.')
param workloadName string = 'movie-api'

@description('Azure region for all resources. Defaults to the resource group location.')
param location string = resourceGroup().location

@description('Optional tags applied to all resources.')
param tags object = {}

@description('App Service plan SKU name.')
param appServicePlanSkuName string = 'B1'

@minValue(1)
@description('App Service plan instance count.')
param appServicePlanSkuCapacity int = 1

@description('Linux runtime stack for the migrated API.')
param linuxFxVersion string = 'DOTNETCORE|8.0'

@description('Whether Always On should be enabled for the API app.')
param appServiceAlwaysOn bool = false

@description('SQL administrator login used for the initial logical server deployment.')
param sqlAdministratorLogin string

@secure()
@description('SQL administrator password used for the initial logical server deployment.')
param sqlAdministratorPassword string

@description('Azure SQL Database SKU name.')
param sqlDatabaseSkuName string = 'Basic'

@description('Azure SQL Database SKU tier.')
param sqlDatabaseSkuTier string = 'Basic'

@minValue(2147483648)
@description('Azure SQL Database max size in bytes.')
param sqlDatabaseMaxSizeBytes int = 2147483648

@description('Whether to create the AllowAzureServices firewall rule on the SQL server.')
param allowAzureServicesToAccessSql bool = false

@description('Optional list of client IP addresses allowed to access the SQL server.')
param sqlAllowedIpAddresses array = []

var uniqueSuffix = substring(uniqueString(subscription().id, resourceGroup().id, workloadName, environmentName), 0, 6)
var normalizedWorkloadName = toLower(replace(workloadName, '-', ''))
var mergedTags = union(tags, {
  environment: environmentName
  workload: workloadName
  managedBy: 'bicep'
})

var logAnalyticsWorkspaceName = 'log-${workloadName}-${environmentName}'
var applicationInsightsName = 'appi-${workloadName}-${environmentName}'
var appServicePlanName = 'asp-${workloadName}-${environmentName}'
var webAppName = 'app-${workloadName}-${environmentName}-${uniqueSuffix}'
var keyVaultName = substring('kv${normalizedWorkloadName}${environmentName}${uniqueSuffix}', 0, min(length('kv${normalizedWorkloadName}${environmentName}${uniqueSuffix}'), 24))
var sqlServerName = 'sql-${workloadName}-${environmentName}-${uniqueSuffix}'
var sqlDatabaseName = '${workloadName}-${environmentName}'
var keyVaultUri = 'https://${keyVaultName}.${environment().suffixes.keyvaultDns}/'
var sqlServerFullyQualifiedDomainName = '${sqlServerName}.${environment().suffixes.sqlServerHostname}'
var sqlConnectionString = 'Server=tcp:${sqlServerFullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabaseName};Persist Security Info=False;User ID=${sqlAdministratorLogin};Password=${sqlAdministratorPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  params: {
    location: location
    tags: mergedTags
    workspaceName: logAnalyticsWorkspaceName
    applicationInsightsName: applicationInsightsName
  }
}

module appService './modules/app-service.bicep' = {
  name: 'appService'
  params: {
    location: location
    tags: mergedTags
    appServicePlanName: appServicePlanName
    webAppName: webAppName
    appServicePlanSkuName: appServicePlanSkuName
    appServicePlanSkuCapacity: appServicePlanSkuCapacity
    linuxFxVersion: linuxFxVersion
    appServiceAlwaysOn: appServiceAlwaysOn
    applicationInsightsConnectionString: monitoring.outputs.applicationInsightsConnectionString
    keyVaultUri: keyVaultUri
    sqlServerFullyQualifiedDomainName: sqlServerFullyQualifiedDomainName
    sqlDatabaseName: sqlDatabaseName
  }
}

module sql './modules/sql.bicep' = {
  name: 'sql'
  params: {
    location: location
    tags: mergedTags
    sqlServerName: sqlServerName
    sqlDatabaseName: sqlDatabaseName
    administratorLogin: sqlAdministratorLogin
    administratorPassword: sqlAdministratorPassword
    allowAzureServices: allowAzureServicesToAccessSql
    allowedIpAddresses: sqlAllowedIpAddresses
    sqlDatabaseSkuName: sqlDatabaseSkuName
    sqlDatabaseSkuTier: sqlDatabaseSkuTier
    sqlDatabaseMaxSizeBytes: sqlDatabaseMaxSizeBytes
  }
}

module keyVault './modules/key-vault.bicep' = {
  name: 'keyVault'
  params: {
    location: location
    tags: mergedTags
    keyVaultName: keyVaultName
    tenantId: subscription().tenantId
    webAppPrincipalId: appService.outputs.webAppPrincipalId
    sqlConnectionStringSecretName: 'SqlConnectionString'
    sqlConnectionString: sqlConnectionString
  }
}

output applicationInsightsName string = monitoring.outputs.applicationInsightsName
output appServicePlanName string = appService.outputs.appServicePlanName
output keyVaultName string = keyVault.outputs.keyVaultName
output keyVaultUri string = keyVault.outputs.keyVaultUri
output sqlConnectionStringSecretUri string = keyVault.outputs.sqlConnectionStringSecretUri
output sqlDatabaseName string = sql.outputs.sqlDatabaseName
output sqlServerFullyQualifiedDomainName string = sql.outputs.sqlServerFullyQualifiedDomainName
output webAppDefaultHostName string = appService.outputs.webAppDefaultHostName
output webAppName string = appService.outputs.webAppName
