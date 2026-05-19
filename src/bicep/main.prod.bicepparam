using './main.bicep'

param environmentName = 'prod'
param workloadName = 'ghcdemo'
param location = 'eastus2'
param sqlAdministratorLogin = 'sqladminuser'
param sqlAdministratorPassword = readEnvironmentVariable('AZURE_SQL_ADMIN_PASSWORD')
param sqlDatabaseName = 'ghcdemoapp'
param tags = {
  'azd-env-name': 'prod'
  'azd-service-name': 'api'
  environment: 'production'
  owner: 'platform-team'
}
