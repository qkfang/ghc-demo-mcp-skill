using '../main.bicep'

param environmentName = 'test'
param location = 'eastus'
param tags = {
  environment: 'test'
  owner: 'api-team'
}
param sqlAdministratorLogin = 'sqladminuser'
param sqlAdministratorPassword = 'ChangeM3BeforeDeployment!'
param appServicePlanSkuName = 'S1'
param appServicePlanSkuCapacity = 1
param linuxFxVersion = 'DOTNETCORE|8.0'
param appServiceAlwaysOn = true
param sqlDatabaseSkuName = 'S0'
param sqlDatabaseSkuTier = 'Standard'
param sqlDatabaseMaxSizeBytes = 268435456000
param allowAzureServicesToAccessSql = true
param sqlAllowedIpAddresses = []
