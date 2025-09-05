//Name of the storage account. Found top left of overview
@minLength(3)
@maxLength(24)
param storageAccountName string
param function_app_name string
param workspaceName string
param applicationInsightsName string

//https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-storage-tiers
@allowed([
	'Hot'
  'Cool'
])
param accessTier string = 'Hot'
//https://docs.microsoft.com/en-us/azure/templates/microsoft.storage/storageaccounts?tabs=json
@allowed([
  'TLS1_2'
])
param minimumTlsVersion string = 'TLS1_2'
param supportsHttpsTrafficOnly bool = true
param allowBlobPublicAccess bool = false
param allowSharedKeyAccess bool = true
param networkAclsBypass string = 'AzureServices'
param networkAclsDefaultAction string = 'Allow'
param isBlobSoftDeleteEnabled bool = true
param blobSoftDeleteRetentionDays int = 7
//Will need this param for "Store Sales hub -No ePHI data
//param isHnsEnabledBool bool = true

@allowed([
	'Storage'
  'StorageV2'
  'BlobStorage'
  'FileStorage'
  'BlockBlobStorage'
])
param kind string = 'StorageV2'
param location string = resourceGroup().location

@allowed([
	'Premium_LRS'
	'Premium_ZRS'
	'Standard_GRS'
	'Standard_LRS'
	'Standard_RAGRS'
	'Standard_ZRS'
  'Standard_RAGZRS'
])
param accountType string = 'Standard_RAGZRS'

var globalTags = {
  'wfm-team': 'POS'  //TODO: TEMPORARY FIX FOR TESTING, REMOVE THIS AGE AFTER HE ISSUE ON THE ENABLEMENT OF THE TAGS RESOLVED
  'wfm-application': 'Point of Sale Data Hub'
  'wfm-data-classification': 'Regulatory'
  'wfm-data-classification-ephi': 'true'
  'wfm-data-classification-pci': 'false'
  'wfm-data-classification-pi': 'false'
  'wfm-data-classification-employee': 'true'
  'wfm-data-classification-customer': 'true'
  'wfm-data-protection': 'Administrative'
  'wfm-data-usage': 'Transient'
  'wfm-data-encrypted': 'false'
  'wfm-data-sharing': 'Internal'
}

@allowed([
  '60acc713-2029-42eb-9ca8-187009d38f16' //Test 
  '01fc07d9-9e5d-4465-8d5a-ecc962f03913' //PROD
])
param principalId string
@allowed([
	'Owner'
  'Contributor'
  'Reader'
  'StorageBlobDataReader'
  'StorageBlobDataContributor'
])
param builtInRoleType string

//az role definition list --output json --query '[].{roleName:roleName, description:description, id:id}'
var roleDefinitionId = {
  Owner: {
    id: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '8e3af657-a8ff-443c-a75c-2fe8c4bcb635')
  }
  Contributor: {
    id: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b24988ac-6180-42a0-ab88-20f7382dd24c')
  }
  Reader: {
    id: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'acdd72a7-3385-48ef-bd42-f606fba81ae7')
  }
  StorageBlobDataReader: {
    id: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '2a2b9908-6ea1-4ae2-8e65-a410df84e7d1')
  }
  StorageBlobDataContributor: {
    id: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
  }
}

param EventGridEndpoint string
@secure()
param EventGridKey string
param CloudEventSource__Name string

param vnetName string
param privateLinkAddressPrefix string
param subnetName string 
param vnetResourceGroupName string
param customerClientServerAddress string
param productApiConfigBaseAddress string
@secure()
param productApiConfigApiKey string
param itemFunctionAppURL string
param priceApiConfigBaseAddress string
@secure()
param priceApiConfigApiKey string
param priceFunctionAppURL string
@secure()
param possalesdatahubfunctionkey string 

var environments = {
  test: {
    dataProtection:  {
      'wfm-data-protection': 'Administrative'
    }
    incidentWebhookUri: 'https://nonprod.api.wegmans.io'
    incidentWebhookServicePrincipal: 'bbf5f0ff-0dad-4091-a582-9190b6f86759'
}
  prod: {
    dataProtection:  {
      'wfm-data-protection': 'Business Critical'
    }
    incidentWebhookUri: 'https://api.wegmans.io'
    incidentWebhookServicePrincipal: '22c44006-a97f-42bc-8373-e04e19c3e6ea'
  }
}

var tags = union(globalTags, environments[what_env].dataProtection)

resource storageAccountName_resource 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: storageAccountName
  location: location
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  sku: {
    name: accountType
  }
  kind: kind
  properties: {
    isHnsEnabled: true
    accessTier: accessTier
    minimumTlsVersion: minimumTlsVersion
    supportsHttpsTrafficOnly: supportsHttpsTrafficOnly
    allowBlobPublicAccess: allowBlobPublicAccess
    allowSharedKeyAccess: allowSharedKeyAccess
    networkAcls: {
      bypass: networkAclsBypass
      defaultAction: networkAclsDefaultAction
    }
  }
  dependsOn: []
}

resource storageAccountName_default 'Microsoft.Storage/storageAccounts/blobServices@2021-04-01' = {
  parent: storageAccountName_resource
  name: 'default'
  properties: {
    containerDeleteRetentionPolicy: {
      enabled: isBlobSoftDeleteEnabled
      days: blobSoftDeleteRetentionDays
    }
  }
}

param userAssignedIdentityName string = 'weg-${storageAccountName}'
resource userAssignedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userAssignedIdentityName
  location: location
  tags: tags
}

resource storageAccountName_queue 'Microsoft.Storage/storageAccounts/queueServices@2021-04-01' = {
  parent: storageAccountName_resource
  name: 'default'
}

//Create Container for raw XML from controller
resource raw_tlog_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'raw-tlog'
}

//Create Container for JSON Store Close
resource close_string_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'store-close-events'
}

//Create Container for JSON transformation
resource transaction_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'transactions'
}

//Create Container for JSON exceptions
resource exceptions_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'exceptions'
}

//Create Container for list of json uris to deidentify
resource json_uris_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'json-uris'
}

//Create Container for reprocessing transactions settings
resource reprocess_transactions_container 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'reprocess-transactions'
}

//////////////////////////////
// DeadLettering Containers //
/////////////////////////////
//Create Container for Deadlettering failed
resource event_publisher_container_dl 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'event-publishing-deadletter'
}

resource raw_tlog_container_dl 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  parent: storageAccountName_default
  name: 'raw-tlog-deadletter'
}


////////////
// Tables //
///////////

resource storageAccountName_table 'Microsoft.Storage/storageAccounts/tableServices@2021-06-01' = {
  name: 'default'
  parent: storageAccountName_resource
}
 
resource itemTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2021-06-01' = {
  name: 'ItemsTable'
  parent: storageAccountName_table
}

resource reprocessSettingsTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2021-06-01' = {
  name: 'ReprocessSettingsTable'
  parent: storageAccountName_table
}

////////////
// Queues //
///////////

//Create queue for store closing events
resource create_queue_storeclose 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: 'raw-tlog-store-close-events'
  parent: storageAccountName_queue
}

//Create queue for transaction events
resource create_queue_transactions 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: 'raw-tlog-transactions'
  parent: storageAccountName_queue
}

//Create queue for exception events
resource create_queue_exceptions 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: 'raw-tlog-exceptions'
  parent: storageAccountName_queue
}

//Create queue for reprocessing transactions messages
resource create_queue_reprocess_transactions 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: 'reprocess-transactions'
  parent: storageAccountName_queue
}

//Create queue for batch modifier messages
resource create_queue_batch_modifier 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: 'batch-modifier'
  parent: storageAccountName_queue
}

resource storageTopic 'Microsoft.EventGrid/systemTopics@2020-04-01-preview' = {
  name: storageAccountName_resource.name
  location: location
  tags: tags
  properties: {
    source: storageAccountName_resource.id
    topicType: 'Microsoft.Storage.StorageAccounts'
  }
}

/////////////////////////////////
// Databricks Queues and Topic //
/////////////////////////////////

resource create_queue_pos_databricks_sandbox 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = if (what_env == 'test') {
    name: 'possalesdatahub-v1-pos-databricks-sandbox-01'
    parent: storageAccountName_queue
}

resource saleseventsdatabrickssandbox 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = if (what_env == 'test') {
    name: 'possalesdatahub-v1-pos-databricks-sandbox-blobCreated-01'
    parent: storageTopic
    properties: {
    destination: {
        properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_pos_databricks_sandbox.name
        queueMessageTimeToLiveInSeconds: -1
        }
        endpointType: 'StorageQueue'
    }
    filter: {
        subjectBeginsWith: '/blobServices/default/containers/transactions/blobs/v1/'
        includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
        ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
        maxDeliveryAttempts: 30
        eventTimeToLiveInMinutes: 1440
    }
    }
}

resource create_queue_pos_databricks_test 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = if (what_env == 'test') {
    name: 'possalesdatahub-v1-pos-databricks-test-01'
    parent: storageAccountName_queue
}

resource saleseventsdatabrickstest 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = if (what_env == 'test') {
    name: 'possalesdatahub-v1-pos-databricks-test-blobCreated-01'
    parent: storageTopic
    properties: {
    destination: {
        properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_pos_databricks_test.name
        queueMessageTimeToLiveInSeconds: -1
        }
        endpointType: 'StorageQueue'
    }
    filter: {
        subjectBeginsWith: '/blobServices/default/containers/transactions/blobs/v1/'
        includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
        ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
        maxDeliveryAttempts: 30
        eventTimeToLiveInMinutes: 1440
    }
    }
}

resource create_queue_pos_databricks_preview 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = if (what_env == 'prod') {
    name: 'possalesdatahub-v1-pos-databricks-preview-01'
    parent: storageAccountName_queue
}

resource saleseventsdatabrickspreview 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = if (what_env == 'prod') {
    name: 'possalesdatahub-v1-pos-databricks-preview-blobCreated-01'
    parent: storageTopic
    properties: {
    destination: {
        properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_pos_databricks_preview.name
        queueMessageTimeToLiveInSeconds: -1
        }
        endpointType: 'StorageQueue'
    }
    filter: {
        subjectBeginsWith: '/blobServices/default/containers/transactions/blobs/v1/'
        includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
        ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
        maxDeliveryAttempts: 30
        eventTimeToLiveInMinutes: 1440
    }
    }
}

resource create_queue_pos_databricks_prodeus2 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = if (what_env == 'prod') {
    name: 'possalesdatahub-v1-pos-databricks-prod-eus2-01'
    parent: storageAccountName_queue
}

resource saleseventsdatabricksprodeus2 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = if (what_env == 'prod') {
    name: 'possalesdatahub-v1-pos-databricks-prod-eus2-blobCreated-01'
    parent: storageTopic
    properties: {
    destination: {
        properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_pos_databricks_prodeus2.name
        queueMessageTimeToLiveInSeconds: -1
        }
        endpointType: 'StorageQueue'
    }
    filter: {
        subjectBeginsWith: '/blobServices/default/containers/transactions/blobs/v1/'
        includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
        ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
        maxDeliveryAttempts: 30
        eventTimeToLiveInMinutes: 1440
    }
    }
}

resource create_queue_pos_databricks_prodcus 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = if (what_env == 'prod') {
    name: 'possalesdatahub-v1-pos-databricks-prod-cus-01'
    parent: storageAccountName_queue
}

resource saleseventsdatabricksprodcus 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = if (what_env == 'prod') {
    name: 'possalesdatahub-v1-pos-databricks-prod-cus-blobCreated-01'
    parent: storageTopic
    properties: {
    destination: {
        properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_pos_databricks_prodcus.name
        queueMessageTimeToLiveInSeconds: -1
        }
        endpointType: 'StorageQueue'
    }
    filter: {
        subjectBeginsWith: '/blobServices/default/containers/transactions/blobs/v1/'
        includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
        ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
        maxDeliveryAttempts: 30
        eventTimeToLiveInMinutes: 1440
    }
    }
}


/////////////////////////////////////////
// Event movement from BLOB to queue  ///
/////////////////////////////////////////

resource storecloseevents 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'raw-tlog-store-close-events'
  parent: storageTopic
  properties: {
    destination: {
      properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_storeclose.name
        queueMessageTimeToLiveInSeconds: -1
      }
      endpointType: 'StorageQueue'
    }
    filter: {
      subjectBeginsWith: '/blobServices/default/containers/raw-tlog/'
      subjectEndsWith: '_storecloseevent.xml'
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}

resource transactions 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'raw-tlog-transactions'
  parent: storageTopic
  properties: {
    destination: {
      properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_transactions.name
        queueMessageTimeToLiveInSeconds: -1
      }
      endpointType: 'StorageQueue'
    }
    filter: {
      subjectBeginsWith: '/blobServices/default/containers/raw-tlog/'
      subjectEndsWith: '_transaction.xml'
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}

resource exceptions 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'raw-tlog-exceptions'
  parent: storageTopic
  properties: {
    destination: {
      properties: {
        resourceId: storageAccountName_resource.id
        queueName: create_queue_exceptions.name
        queueMessageTimeToLiveInSeconds: -1
      }
      endpointType: 'StorageQueue'
    }
    filter: {
      subjectBeginsWith: '/blobServices/default/containers/raw-tlog/'
      subjectEndsWith: '_exceptions.xml'
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}

//Creation of Final queue - Users will subscribe to this queue
resource final_queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  name: 'event-publisher'
  parent: storageAccountName_queue
}

////////////////////////////////////////////////
// Final Queue for independent subscribers    //
////////////////////////////////////////////////
resource transactionseventpublisher 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'transactions-event-publisher'
  parent: storageTopic
  properties: {
    destination: {
      properties: {
        resourceId: storageAccountName_resource.id
        queueName: final_queue.name
        queueMessageTimeToLiveInSeconds: -1
      }
      endpointType: 'StorageQueue'
    }
    filter: {
      subjectBeginsWith: '/blobServices/default/containers/transactions'
      subjectEndsWith: '.json'
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}

resource storecloseeventseventpublisher 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'store-close-events-event-publisher'
  parent: storageTopic
  properties: {
    destination: {
      properties: {
        resourceId: storageAccountName_resource.id
        queueName: final_queue.name
        queueMessageTimeToLiveInSeconds: -1
      }
      endpointType: 'StorageQueue'
    }
    filter: {
      subjectBeginsWith: '/blobServices/default/containers/store-close-events'
      subjectEndsWith: '.json'
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}

resource exceptionseventpublisher 'Microsoft.EventGrid/systemTopics/eventSubscriptions@2021-06-01-preview' = {
  name: 'exceptions-event-publisher'
  parent: storageTopic
  properties: {
    destination: {
      properties: {
        resourceId: storageAccountName_resource.id
        queueName: final_queue.name
        queueMessageTimeToLiveInSeconds: -1
      }
      endpointType: 'StorageQueue'
    }
    filter: {
      subjectBeginsWith: '/blobServices/default/containers/exceptions'
      subjectEndsWith: '.json'
      includedEventTypes: [
        'Microsoft.Storage.BlobCreated'
      ]
    }
    eventDeliverySchema: 'EventGridSchema'
    retryPolicy: {
      maxDeliveryAttempts: 30
      eventTimeToLiveInMinutes: 1440
    }
  }
}



//Add Reader role to Storage account
resource addRoleAuthorization_reader 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(storageAccountName, principalId, roleDefinitionId['StorageBlobDataReader'].id, storageAccountName_resource.id)
  scope: storageAccountName_resource
  properties: {
    roleDefinitionId: roleDefinitionId['Reader'].id
    principalId: principalId
  }
}

//Add Blob Contributor role to Storage account (the hub normally uses reader, but contributor is needed for the ReprocessTransactionsSubscriber function)
module storageAccountRoleAssignments './storageAccountRoleAssignments.bicep' = {
  name: 'storage-account-role-assignments'
  params: {
    storageAccountName: storageAccountName
    principalId: userAssignedIdentity.properties.principalId
    roleDefinitionId: roleDefinitionId['StorageBlobDataContributor'].id
  }
}

//Add Blob Reader to transaction container
resource addRoleAuthorization_tran 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(storageAccountName, principalId, roleDefinitionId['StorageBlobDataReader'].id, transaction_container.id)
  scope: transaction_container
  dependsOn:[
    transaction_container
  ]
  properties: {
    roleDefinitionId: roleDefinitionId[builtInRoleType].id
    principalId: principalId
  }
}

// //Add Blob Reader to Store Close Container
resource addRoleAuthorization_close 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(storageAccountName, principalId, roleDefinitionId['StorageBlobDataReader'].id, close_string_container.id)
  scope: close_string_container
  dependsOn:[
    close_string_container
  ]
  properties: {
    roleDefinitionId: roleDefinitionId[builtInRoleType].id
    principalId: principalId
  }
}

// //Add Blob Reader to exception container
resource addRoleAuthorization_excep 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(storageAccountName, principalId, roleDefinitionId['StorageBlobDataReader'].id, exceptions_container.id)
  scope: exceptions_container
  dependsOn:[
    exceptions_container
  ]
  properties: {
    roleDefinitionId: roleDefinitionId[builtInRoleType].id
    principalId: principalId
  }
}


/////////////////////////////////////////////////////
// Function app + App Insights + App Service plan //
///////////////////////////////////////////////////

@allowed([ 'test', 'cert', 'prod' ])
param what_env string

var WEGMANS__EVENTS__PUBLISHER__URIS = {
  test: 'https://wegmans-cloud-events-test.eastus2-1.eventgrid.azure.net/api/events'
  cert: 'https://wegmans-cloud-events-cert.eastus2-1.eventgrid.azure.net/api/events'
  prod: 'https://wegmans-cloud-events-prod.eastus2-1.eventgrid.azure.net/api/events'
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: workspaceName
  location: location
  tags: tags
  properties: {
     sku: {
      name: 'Standalone'
     }
     retentionInDays: 90
     features: {
      enableLogAccessUsingOnlyResourcePermissions: true
     }
  }
}

module applicationInsights 'ts/wegmans:microsoft-insights-components:1.0' = {
  name: '${deployment().name}-insights'
  params: {
    name: applicationInsightsName
    location: location
    tags: tags
    workspaceId: workspace.id
  }
}

var userAssignedIdentityParam = {
  type: 'UserAssigned'
  userAssignedIdentities: {
    '${userAssignedIdentity.id}': {}
  }
}
resource function_app 'Microsoft.Resources/deployments@2021-04-01' ={
  name: '${deployment().name}.function_app'
  properties: {
    mode:'Incremental'
    templateLink:{
      id: '/subscriptions/f211da81-4b80-49dc-8ff4-4463da1362f0/resourceGroups/template-specs/providers/Microsoft.Resources/templateSpecs/microsoft-web-sites-function-app/versions/1.0'
    }
    parameters:{
      tags: {
        value: tags
      }
      name: {
        value: function_app_name
      }
      version: {
        value: '~4'
      }
      identity: {
        value: userAssignedIdentityParam
      }
      applicationInsightsName: {
        value: applicationInsightsName
      }
      applicationInsightsWorkspaceId: {
        value: workspace.id
      }
      appsettings: {
        value: {
          WEBSITE_RUN_FROM_PACKAGE: '1'
          DEFAULTAZURECREDENTIAL__MANAGEDIDENTITYCLIENTID: userAssignedIdentity.properties.clientId
          POSDataHubAccount: 'DefaultEndpointsProtocol=https;AccountName=${toLower(storageAccountName_resource.name)};AccountKey=${listKeys(storageAccountName_resource.id, '2021-04-01').keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
          DsarOptions__DsarStorageConnection:  'DefaultEndpointsProtocol=https;AccountName=${toLower(storageAccountName_resource.name)};AccountKey=${listKeys(storageAccountName_resource.id, '2021-04-01').keys[0].value};EndpointSuffix=${environment().suffixes.storage}'
          RawTlogTransactionsEventQueueName: 'raw-tlog-transactions'
          RawTlogTransactionsPoisonQueueName: 'raw-tlog-transactions-poison'
          TlogEventSubscriberQueueName: 'event-publisher'
          RawTlogDeadLetter: 'raw-tlog-deadletter'
          Transactions_container: 'transactions'
          EventPublisherDeadletter: event_publisher_container_dl.name
          RawTlog_container: 'raw-tlog'
          EventGridEndpoint: EventGridEndpoint
          EventGridKey: EventGridKey
          CloudEventSource__Name: CloudEventSource__Name
          RawTlogStoreCloseEventQueueName: 'raw-tlog-store-close-events'
          StoreCloseEvents_container: 'store-close-events'
          WEGMANS__ENTERPRISELIBRARY__EVENTS__PUBLISHER__TOPIC: 'pos'
          WEGMANS__ENTERPRISELIBRARY__EVENTS__PUBLISHER__URI: WEGMANS__EVENTS__PUBLISHER__URIS[what_env]
          CustomerClient__ServerAddress: customerClientServerAddress
          ProductApiConfig__BaseAddress: productApiConfigBaseAddress
          ProductApiConfig__ApiKey: productApiConfigApiKey
          PriceApiConfig__BaseAddress: priceApiConfigBaseAddress
          PriceApiConfig__ApiKey: priceApiConfigApiKey
          ItemFunctionAppURL: itemFunctionAppURL
          PriceFunctionAppURL: priceFunctionAppURL
          possalesdatahubfunctionkey: possalesdatahubfunctionkey
          'AzureWebJobs.TlogProcessDeadLetters.Disabled': true
          'AzureWebJobs.ReprocessTransactionsSubscriber.Disabled': true
          'AzureWebJobs.CheckReprocessTableConfigurations.Disabled': true
          ReprocessTransactions__MaxRawTLogQueueSize: 150000
          ReprocessTransactions__MaxDegreesOfParallelism: 150
          ReprocessTransactions__CronSchedule: '0 */10 * * * *'
          BatchModifierOptions__MaxDegreeOfParallelismForQueueing: 50
        }
      }
    }
  }
}
////////////////////////////////////////////////////////////////////////
//https://docs.wegmans.tech/engineering/feedback/monitor/azure-alert.html?tabs=deadletter-arm
//Create a new alert Group for Metric Alerts
resource alertGroup 'Microsoft.Insights/actionGroups@2019-06-01' = {
  name: 'alertGroup'
  location: 'Global'
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    groupShortName: 'alertGroup'
    enabled: true
    webhookReceivers: [
      {
        name: 'Incident'
        serviceUri: '${environments[what_env].incidentWebhookUri}/incidents/webhook?group=DEV%20PHARM&service=ACE&urgency=Low&impact=Minor&api-version=2022-08-01'
        useCommonAlertSchema: true
        useAadAuth: true
        objectId: environments[what_env].incidentWebhookServicePrincipal
        tenantId: subscription().tenantId
        identifierUri: environments[what_env].incidentWebhookUri
      }
    ]
  	emailReceivers: [
      {
        emailAddress: 'Michael.Zobel@wegmans.com'
        name: 'DevPOS'
      }
      {
        emailAddress: 'PharmacyIT@wegmans.com'
        name: 'DevPharm'
      }
	  ]
  }
}

//Monitor the event-publisher queue health
resource EventPublisherAlert 'Microsoft.Insights/metricAlerts@2018-03-01' = {
  name: 'Unhealthy Availability - TlogSubscriberHub'
  location: 'global'
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    description: 'Whenever a availability is less than 100%.'
    autoMitigate: true
    severity: 1
    enabled: true
    scopes: [
      applicationInsights.outputs.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          threshold: 100
          name: 'Metric1'
          metricNamespace: 'microsoft.insights/components'
          metricName: 'availabilityResults/availabilityPercentage'
          dimensions: [
          ]
          operator: 'LessThan'
          timeAggregation: 'Average'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
    }
    targetResourceType: 'microsoft.insights/components'
    actions: [
      {
        actionGroupId: alertGroup.id
        webHookProperties: {}
      }
    ]
  }
}
///////////////////////////////////////////////////////////////////////

module subnets 'subnets.bicep' = {
  name: '${deployment().name}.subnets'
  scope: resourceGroup(vnetResourceGroupName)
  params: {
    subnetName: subnetName
    vnetName: vnetName
    privateLinkAddressPrefix: privateLinkAddressPrefix
  }
}

resource storageEndpoint 'Microsoft.Network/privateEndpoints@2020-11-01' = {
  name: storageAccountName_resource.name
  location: location
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    privateLinkServiceConnections: [
      {
        name: storageAccountName_resource.name
        properties: {
          privateLinkServiceId: storageAccountName_resource.id
          groupIds: [
            'blob'
          ]
        }
      }
    ]
    subnet: {
      id: subnets.outputs.privateLinkSubnetId
    }
  }
}
