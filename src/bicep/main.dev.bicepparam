using './main.bicep'

param environmentName = 'dev'
param workloadName = 'ghcdemo'
param location = 'eastus'
param sqlAdministratorLogin = 'sqladminuser'
param sqlAdministratorPassword = readEnvironmentVariable('AZURE_SQL_ADMIN_PASSWORD')
param sqlDatabaseName = 'ghcdemoapp'
param tags = {
  'azd-env-name': 'dev'
  'azd-service-name': 'api'
  environment: 'development'
  owner: 'platform-team'
}
