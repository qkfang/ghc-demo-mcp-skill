@description('Azure region for the App Service plan.')
param location string

@description('App Service plan name.')
param appServicePlanName string

@description('App Service plan SKU name.')
param appServicePlanSkuName string

@description('Log Analytics workspace resource ID.')
param logAnalyticsWorkspaceId string

@description('Tags applied to plan resources.')
param tags object

resource appServicePlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  kind: 'functionapp,linux'
  sku: {
    name: appServicePlanSkuName
    tier: 'FlexConsumption'
  }
  properties: {
    reserved: true
  }
}

resource planDiagnostics 'Microsoft.Insights/diagnosticSettings@2021-05-01-preview' = {
  name: '${appServicePlanName}-diag'
  scope: appServicePlan
  properties: {
    workspaceId: logAnalyticsWorkspaceId
    logs: [
      {
        categoryGroup: 'allLogs'
        enabled: true
      }
    ]
    metrics: [
      {
        category: 'AllMetrics'
        enabled: true
      }
    ]
  }
}

output appServicePlanId string = appServicePlan.id
