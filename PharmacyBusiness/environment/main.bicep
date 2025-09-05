param applicationPrincipalId string
param keyVaultName string
param applicationInsightsName string
param dataProtectionLevel string
param teamNotificationEmailList string
param notifyPharmacyTeamActionGroupName string
param alertOnExceptionMetricName string
param primaryLocation string = 'eastus2'

var tags = {
  'wfm-application': 'Pharmacy Business'
  'wfm-data-classification': 'Restricted'
  'wfm-data-classification-ephi': 'false'
  'wfm-data-classification-pci': 'false'
  'wfm-data-classification-pi': 'false'
  'wfm-data-classification-employee': 'false'
  'wfm-data-classification-customer': 'false'
  'wfm-data-protection': dataProtectionLevel
  'wfm-data-usage': 'Transient'
  'wfm-data-encrypted': 'false'
  'wfm-data-sharing': 'External'
}

var azureKeyVaultUrl = 'https://${keyVaultName}.vault.azure.net/'

resource keyVault 'Microsoft.KeyVault/vaults@2022-07-01' = {
  name: keyVaultName
  location: primaryLocation
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    tenantId: tenant().tenantId
    vaultUri: azureKeyVaultUrl
    accessPolicies: []
    sku: {
      family: 'A'
      name: 'standard'
    }
    enabledForDeployment: false
    enableSoftDelete: true
    enableRbacAuthorization: true
    enablePurgeProtection: true
    provisioningState: 'Succeeded'
    publicNetworkAccess: 'Enabled'
  }
}

// Output the vault name and URL for copying to the appsettings.json during deployment
output applicationKeyVaultName string = keyVault.name
output applicationKeyVaultUri string = keyVault.properties.vaultUri
output subscriptionId string = subscription().subscriptionId

var azureKeyVaultSecretsUser = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')

// Get the application rights to read secrets from the vault
resource applicationRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, azureKeyVaultSecretsUser, keyVault.id, applicationPrincipalId)
  scope: keyVault
  properties: {
    roleDefinitionId: azureKeyVaultSecretsUser
    principalId: applicationPrincipalId
  }
}

resource workspace 'Microsoft.OperationalInsights/workspaces@2021-06-01' = {
  name: applicationInsightsName
  location: primaryLocation
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    sku: {
      name: 'Standalone'
    }
    retentionInDays: 90
    features: {
      searchVersion: 1
      legacy: 0
      enableLogAccessUsingOnlyResourcePermissions: true
    }
  }
}

module applicationInsights 'ts/wegmans:microsoft-insights-components:1.0' = {
  name: '${deployment().name}-insights'
  params: {
    name: applicationInsightsName
    location: primaryLocation
    tags: tags
    workspaceId: workspace.id
  }
}

resource actionGroups_notify_pharmacy_team_resource 'microsoft.insights/actionGroups@2023-09-01-preview' = {
  name: notifyPharmacyTeamActionGroupName
  location: 'Global'
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    groupShortName: 'EmailDevPhar'
    enabled: true
    emailReceivers: [
      {
        name: 'Email PharmacyIT@wegmans.com_-EmailAction-'
        emailAddress: teamNotificationEmailList
        useCommonAlertSchema: true
      }
    ]
    smsReceivers: []
    webhookReceivers: []
    eventHubReceivers: []
    itsmReceivers: []
    azureAppPushReceivers: []
    automationRunbookReceivers: []
    voiceReceivers: []
    logicAppReceivers: []
    azureFunctionReceivers: []
    armRoleReceivers: []
  }
}

// Alert setup to send an alert to the team if any Exception is thrown by the application, checked every 5 minutes
resource scheduledqueryrules_error_in_pharmacy_business_name_resource 'microsoft.insights/scheduledqueryrules@2023-03-15-preview' = {
  name: alertOnExceptionMetricName
  location: primaryLocation
  tags: union(tags, {
    'wfm-data-usage': 'Permanent'
  })
  properties: {
    description: 'Email PharmacyIT@wegmans.com whenever an uncaught Exception is thrown over a 5 minute period'
    severity: 1
    enabled: true
    scopes: [
      applicationInsights.outputs.id
    ]
    evaluationFrequency: 'PT5M'
    windowSize: 'PT5M'
    criteria: {
      allOf: [
        {
          query: 'exceptions | where timestamp >= ago(5m)'
          dimensions: []
          threshold: 0
          operator: 'GreaterThan'
          failingPeriods: {
            numberOfEvaluationPeriods: 1
            minFailingPeriodsToAlert: 1
          }
          timeAggregation: 'Count'
        }
      ]
    }
    actions: {
      actionGroups: [
        '/subscriptions/e01522fc-9e5b-471f-9063-2f530c121632/resourcegroups/pharmacy-business-test/providers/microsoft.insights/actiongroups/pharmacybusinessalertgroup-tst'
      ]
    }
  }
}

// Output the application insights resource name and connection string for copying to the appsettings.json during deployment
output applicationInsightsConnectionString string = applicationInsights.outputs.connectionString
output applicationInsightsName string = applicationInsights.outputs.name
