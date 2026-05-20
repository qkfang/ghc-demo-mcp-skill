param location string
param tags object = {}
param sqlServerName string
param sqlDatabaseName string
param administratorLogin string

@secure()
param administratorPassword string

param allowAzureServices bool = false
param allowedIpAddresses array = []
param sqlDatabaseSkuName string = 'Basic'
param sqlDatabaseSkuTier string = 'Basic'
param sqlDatabaseMaxSizeBytes int = 2147483648

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: administratorLogin
    administratorLoginPassword: administratorPassword
    publicNetworkAccess: 'Enabled'
    minimalTlsVersion: '1.2'
    version: '12.0'
  }
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
  tags: tags
  sku: {
    name: sqlDatabaseSkuName
    tier: sqlDatabaseSkuTier
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: sqlDatabaseMaxSizeBytes
  }
}

resource allowAzureServicesFirewallRule 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = if (allowAzureServices) {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource clientFirewallRules 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = [for (ipAddress, index) in allowedIpAddresses: {
  parent: sqlServer
  name: 'client-${index}'
  properties: {
    startIpAddress: ipAddress
    endIpAddress: ipAddress
  }
}]

output sqlDatabaseName string = sqlDatabase.name
output sqlServerFullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName
output sqlServerName string = sqlServer.name
