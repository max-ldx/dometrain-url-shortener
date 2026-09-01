param location string
param name string

resource staticWebApp 'Microsoft.Web/staticSites@2025-03-01' = {
  location: location
  name: name
  sku: {
    tier: 'Standard'
    name: 'Standard'
  }
}

output id string = staticWebApp.id
