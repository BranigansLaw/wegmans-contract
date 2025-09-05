param storageAccountName string
param principalId string
param roleDefinitionId string

resource storageAccountResource 'Microsoft.Storage/storageAccounts@2021-04-01' existing = {
  name: storageAccountName
}

@description('Grants the requisite access to Databricks Unity Catalog')
module unityCatalogAccess 'ts/wegmans:wegmans-data-hub-storage-account-unity-catalog-access:1.0' = {
  name: '${deployment().name}.unity-catalog-access'
  params: {
    name: storageAccountName
  }
}

resource addRoleAuthorization_reprocess_contributor 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(storageAccountName, principalId, roleDefinitionId, storageAccountResource.id)
  scope: storageAccountResource
  properties: {
    roleDefinitionId: roleDefinitionId
    principalId: principalId
  }
}
