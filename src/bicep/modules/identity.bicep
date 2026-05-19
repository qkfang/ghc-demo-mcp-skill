@description('User-assigned managed identity name.')
param name string

@description('Location for the managed identity.')
param location string

@description('Resource tags.')
param tags object = {}

resource uami 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: name
  location: location
  tags: tags
}

output name string = uami.name
output principalId string = uami.properties.principalId
output resourceId string = uami.id
