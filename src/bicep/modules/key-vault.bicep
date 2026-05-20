param location string
param tags object = {}
param keyVaultName string
param tenantId string
param webAppPrincipalId string
param sqlConnectionStringSecretName string = 'SqlConnectionString'

@secure()
param sqlConnectionString string

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    tenantId: tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        tenantId: tenantId
        objectId: webAppPrincipalId
        permissions: {
          secrets: [
            'get'
            'list'
          ]
        }
      }
    ]
    enableRbacAuthorization: false
    enabledForTemplateDeployment: true
    enablePurgeProtection: true
    publicNetworkAccess: 'Enabled'
    softDeleteRetentionInDays: 90
  }
}

resource sqlConnectionStringSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: sqlConnectionStringSecretName
  properties: {
    value: sqlConnectionString
    contentType: 'text/plain'
  }
}

output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output sqlConnectionStringSecretUri string = '${keyVault.properties.vaultUri}secrets/${sqlConnectionStringSecret.name}'
