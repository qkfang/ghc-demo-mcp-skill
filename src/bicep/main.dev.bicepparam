using './main.bicep'

param environmentName = 'dev'
param location = 'eastus'
param workloadName = 'ghcdemo'
param resourceGroupName = 'rg-ghcdemo-dev'
param owner = 'platform-team'
param storageSkuName = 'Standard_LRS'
param appServicePlanSkuName = 'FC1'
param sqlDatabaseSkuName = 'Basic'
param sqlAdminLogin = 'sqladminuser'
param sqlAdminPassword = readEnvironmentVariable('SQL_ADMIN_PASSWORD', 'ChangeM3Now!')
