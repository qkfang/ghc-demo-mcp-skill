using './main.bicep'

param environmentName = 'prod'
param location = 'centralus'
param workloadName = 'ghcdemo'
param resourceGroupName = 'rg-ghcdemo-prod'
param owner = 'platform-team'
param storageSkuName = 'Standard_ZRS'
param appServicePlanSkuName = 'FC1'
param sqlDatabaseSkuName = 'S0'
param sqlAdminLogin = 'sqladminuser'
param sqlAdminPassword = readEnvironmentVariable('SQL_ADMIN_PASSWORD', 'ChangeM3Now!')
