param name string
param location string
param keyVaultName string
param appPrincipalId string

resource redis 'Microsoft.Cache/redisEnterprise@2025-07-01' = {
  name: name
  location: location
  sku: {
    name: 'Balanced_B0'
  }
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource redisDatabase 'Microsoft.Cache/redisEnterprise/databases@2025-07-01' = {
  parent: redis
  name: 'default'
  properties: {
    clusteringPolicy: 'EnterpriseCluster'
    evictionPolicy: 'VolatileLRU'
    port: 10000
    accessKeysAuthentication: 'Disabled'
  }
}

resource redisAccessPolicyAssignment 'Microsoft.Cache/redisEnterprise/databases/accessPolicyAssignments@2025-07-01' = {
  parent: redisDatabase
  name: 'appEntraPolicy'
  properties: {
    accessPolicyName: 'default' // Politique intégrée autorisant les commandes courantes
    user: {
      objectId: appPrincipalId
    }
  }
}

resource keyVault 'Microsoft.KeyVault/vaults@2026-02-01' existing = {
  name: keyVaultName
}

resource redisCacheConnectionString 'Microsoft.KeyVault/vaults/secrets@2026-02-01' = {
  parent: keyVault
  name: 'Redis--ConnectionString'
  properties: {
    value: '${redis.properties.hostName}:10000,ssl=True,abortConnect=False'
  }
}

output id string = redis.id
