using '../main.bicep'

param environmentName = 'dev'
param location = 'eastus'
param tags = {
  environment: 'dev'
  owner: 'api-team'
}
param sqlAdministratorLogin = 'sqladminuser'
param sqlAdministratorPassword = 'ChangeM3BeforeDeployment!'
param appServicePlanSkuName = 'B1'
param appServicePlanSkuCapacity = 1
param linuxFxVersion = 'DOTNETCORE|8.0'
param appServiceAlwaysOn = false
param sqlDatabaseSkuName = 'Basic'
param sqlDatabaseSkuTier = 'Basic'
param sqlDatabaseMaxSizeBytes = 2147483648
param allowAzureServicesToAccessSql = true
param sqlAllowedIpAddresses = []
