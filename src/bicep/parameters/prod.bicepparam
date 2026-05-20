using '../main.bicep'

param environmentName = 'prod'
param location = 'eastus2'
param tags = {
  environment: 'prod'
  owner: 'api-team'
}
param sqlAdministratorLogin = 'sqladminuser'
param sqlAdministratorPassword = 'ChangeM3BeforeDeployment!'
param appServicePlanSkuName = 'P1v3'
param appServicePlanSkuCapacity = 2
param linuxFxVersion = 'DOTNETCORE|8.0'
param appServiceAlwaysOn = true
param sqlDatabaseSkuName = 'S1'
param sqlDatabaseSkuTier = 'Standard'
param sqlDatabaseMaxSizeBytes = 268435456000
param allowAzureServicesToAccessSql = false
param sqlAllowedIpAddresses = []
