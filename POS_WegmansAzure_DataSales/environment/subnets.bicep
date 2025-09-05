param subnetName string 
param vnetName string
param privateLinkAddressPrefix string

resource privateLinkSubnet 'Microsoft.Network/virtualNetworks/subnets@2020-05-01' = {
  name: '${vnetName}/${subnetName}'
  properties: {
    addressPrefix: privateLinkAddressPrefix
    privateEndpointNetworkPolicies: 'Disabled'
  }
}

output privateLinkSubnetId string = privateLinkSubnet.id
