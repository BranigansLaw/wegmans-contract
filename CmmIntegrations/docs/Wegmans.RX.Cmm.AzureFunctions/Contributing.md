# Contributing

Below is all of the information you need to contribute to Wegmans.RX.Cmm.AzureFunctions project.

## Development Configuration

### local.settings.json

```yaml
{
  "IsEncrypted": false,
  "Values": {
    "REGION_NAME": "East US",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AuthorizationOptions__Audience__0": "https://wegmansspecialtypharmacydev.azurefd.net",
    "CmmStorageAccount": "UseDevelopmentStorage=true",
    "APPINSIGHTS_INSTRUMENTATIONKEY": "5918d187-9a68-4d43-aa95-1a8ac54a0498",
    "AppCmmClient__BaseUrl": "https://env11-hubservices.integration.covermymeds.com/api/",
    "AppCmmClient__Username": "wegmans_consumer",
    "AppCmmClient__Password": "",
    "AppAstuteClient__AddressUrl": "https://socialconnect.myastutesolutions.com/WegmansPharmaTrainAIS/AddressService.svc",
    "AppAstuteClient__CaseUrl": "https://socialconnect.myastutesolutions.com/WegmansPharmaTrainAIS/CaseService.svc",
    "AppAstuteClient__CaseStreamUrl": "https://socialconnect.myastutesolutions.com/WegmansPharmaTrainAIS/CaseStreamService.svc",
    "AppAstuteClientSecurity__Username": "",
    "AppAstuteClientSecurity__Password": "",
    "AzureWebJobs.SpecialtyDrugsPatientStatusUpdate.Disabled": false, //This flag can be used to turn off the timer function
    "SpecialtyDrugsPatientStatusUpdateTimerTrigger": "0 0 */1 * * *" //This is the NCRONTAB expression that determines how often the timer will run.currently it is set to run every hour at 0 minutes and 0 seconds
  }
}
```

## Function Naming Convention and Routing

Functions are named based on the business functionality program (i.e. Specialty Drugs) and then the noun(s) that you are operating on (ex: Patient) and then the verb (ex: Update) that describes the action.

> **NOTE:**
>Verbs do not need to match up with CRUD names. They just need to describe what action is being taken from a business standpoint, this will help with supportability when problems are called in from the business

Function routes use REST standard url design. Rest design is: plural nouns separated by "/{id}" where {id} is the id for the noun (ex: specialtydrugs/patients/12345/cases/1). The action being taken on the specific object will be determined by the HTTP VERB (POST, PUT, PATCH, DELETE, GET).

## Function Logging

By default all functions log level is limited to WARN or above. If you want to include INFORMATION level logs you need to add the function to the LogLevel Setting in the host.json.