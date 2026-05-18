using './main.bicep'

param environmentName = 'test'
param location = 'eastus2'
param workloadName = 'ghcdemo'
param resourceGroupName = 'rg-ghcdemo-test'
param owner = 'platform-team'
param storageSkuName = 'Standard_LRS'
param appServicePlanSkuName = 'FC1'
param sqlDatabaseSkuName = 'S0'
param sqlAdminLogin = 'sqladminuser'
param sqlAdminPassword = readEnvironmentVariable('SQL_ADMIN_PASSWORD', 'ChangeM3Now!')
