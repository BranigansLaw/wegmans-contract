param functionAppName string
@allowed([
  'Development'
  'Staging'
  'Production'  
])
param environment string
param orbitaStorageName string
param appAstuteClientAddressUrl string
param appAstuteClientCaseUrl string
param appAstuteClientSecurityUserName string
@secure()
param astutePassword string
param azureWebJobsGetAstutePatientsDisabled bool
param getAstutePatientsTimerTrigger string
param orbitaClientServerAddress string
@secure()
param orbitaApiKey string
param itsmSubscriptionKey string
 @allowed([
   'https://itsm-api.wegmans.io/'           //PROD
   'https://sandbox-itsm-api.wegmans.io/'   //NON-PROD
 ])
param itsmUrl string
var tags = {
  'wfm-application': 'Specialty'
  'rx-service': 'Orbita'
  'wfm-environment': environment
}
var orbitaQueueName = 'orbita-outbound'
resource orbita_storage 'Microsoft.Storage/storageAccounts@2021-04-01'={
  name: orbitaStorageName
  location: resourceGroup().location
  kind: 'StorageV2'
  tags: tags
  sku:{
    name: 'Standard_ZRS'
  }
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
  }
}
resource orbita_storage_queue 'Microsoft.Storage/storageAccounts/queueServices@2021-04-01' = {
  parent: orbita_storage
  name: 'default'
}
resource orbita_storage_outbound_orbita_queue 'Microsoft.Storage/storageAccounts/queueServices/queues@2021-04-01' = {
  parent: orbita_storage_queue
  name: orbitaQueueName 
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
        value: functionAppName
      }
      appsettings: {
        value: {
          AZURE_FUNCTIONS_ENVIRONMENT: environment
          PatientsToProcessQueueName: orbitaQueueName 
          StorageAccountConnection: 'DefaultEndpointsProtocol=https;AccountName=${orbitaStorageName};AccountKey=${listKeys(orbita_storage.id, '2018-07-01').keys[0].value};EndpointSuffix=core.windows.net'
          AppAstuteClient__AddressUrl: appAstuteClientAddressUrl
          AppAstuteClient__CaseUrl: appAstuteClientCaseUrl
          AppAstuteClientSecurity__Username: appAstuteClientSecurityUserName
          AppAstuteClientSecurity__Password: astutePassword
          'AzureWebJobs.GetAstutePatients.Disabled': azureWebJobsGetAstutePatientsDisabled
          GetAstutePatientsTimerTrigger: getAstutePatientsTimerTrigger
          OrbitaClient__ServerAddress: orbitaClientServerAddress
          OrbitaClient__ApiKey: orbitaApiKey
        }
      }
    }
  }
}

resource orbitaQueueAlert 'microsoft.insights/metricAlerts@2018-03-01' = {
  name: 'Orbita Poison Queue Message Count'
  location: 'global'
  tags: tags
  properties: {
    description: 'Whenever a queue is less than 100% available then we need to know.'
    severity: 1
    enabled: true
    scopes: [
      function_app.properties.outputs.applicationInsights.value.id
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
            {
              name: 'availabilityResult/name'
              operator: 'Include'
              values: [
                'Orbita:PatientsToProcessQueueName-poison'
              ]
            }
          ]
          operator: 'LessThan'
          timeAggregation: 'Average'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
    }
    autoMitigate: true
    targetResourceType: 'microsoft.insights/components'
    actions: [
      {
        actionGroupId: actionGroups.id
        webHookProperties: {}
      }
    ]
  }
}

resource orbitaExceptionAlert 'microsoft.insights/metricAlerts@2018-03-01' = {
  name: 'Orbita Exception Count'
  location: 'global'
  tags: tags
  properties: {
    description: 'At least one exception has occurred in the Orbita function app. Investigate via Application Insights.'
    severity: 1
    enabled: true
    scopes: [
      function_app.properties.outputs.applicationInsights.value.id
    ]
    evaluationFrequency: 'PT1M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          threshold: 1
          name: 'Metric1'
          metricNamespace: 'microsoft.insights/components'
          metricName: 'exceptions/count'
          operator: 'GreaterThanOrEqual'
          timeAggregation: 'Count'
          criterionType: 'StaticThresholdCriterion'
        }
      ]
      'odata.type': 'Microsoft.Azure.Monitor.SingleResourceMultipleMetricCriteria'
    }
    autoMitigate: true
    targetResourceType: 'microsoft.insights/components'
    actions: [
      {
        actionGroupId: actionGroups.id
        webHookProperties: {}
      }
    ]
  }
}

resource actionGroups 'Microsoft.Insights/actionGroups@2019-03-01' = {
  name: 'orbitaAlertGroup'
  location: 'Global'
  properties: {
    groupShortName: 'orbAlertGrp'
    enabled: true
    emailReceivers: [
      {
        name: 'Dev Pharm Group_-EmailAction-'
        emailAddress: 'PharmacyIT@wegmans.com'
        useCommonAlertSchema: false
      }
    ]
    webhookReceivers: [
      {
         name: 'Incident'
         serviceUri: '${itsmUrl}incidents/webhook?group=DEV%20PHARM&service=Orbita&urgency=Low&impact=Minor&api-version=2018-12-01&subscription-key=${itsmSubscriptionKey}'
         useCommonAlertSchema: false
      }
    ]
  }
  tags: tags
}

output functionAppName string = function_app.properties.outputs.functionApp.value.name

output functionAppDefaultHostName string = function_app.properties.outputs.functionApp.value.defaultHostName

output appInsightsKey string = function_app.properties.outputs.applicationInsights.value.instrumentationKey
