# INN.JobRunner

This application uses the [Cocona library](https://github.com/mayuki/Cocona) to parse command line arguments and run jobs based on the arguments provided.

All batch jobs are run via this EXE and passing the name of the job in as the first argument.

To get a list of commands and their options, you can run `INN.JobRunner.exe --help`.

## Application Secrets

Application Secrets have been configured to be retrieved from an Azure Key Vault that the app instance has access to once it's deployed to the batch servers. The Azure App has access to the Key Vault, and during pipeline deployment the EXE is given credentials to authenticate as that Azure App.

Developers are not given access to the Azure Key Vault for security purposes, but can override the key vault value retrieval locally by setting secret values under the `secrets` object in their user secrets.

## Debugging in Visual Studio

The `launchSettings.json` file has been configured to allow you to debug different command line commands. In Visual Studio, in the top bar find the drop down button next to the green play button. Click on it and select the command you want to run, then click the green play button to start debugging.

### `local.settings.json`

The job runner stores all secrets in Azure Vault. Locally, you will not have access to retrieve variables from Azure Vault, so you'll need to add values for all the services that you are using into a `local.settings.json` file. It's OK to have dummy values for secrets for services you're not using (for example, if the code you're running doesn't access McKessonDW, it's fine to have dummy values in there since the program will never authenticate)

#### Example

```json
{
  "AzureVaultUrl": "",
  "TenTenTablePath": "wegmans.devpharm.<your_name>.demos",
  "ConsoleLogLevel": "Trace",
  "AppInsightsLogLevel": "Trace",
  "ApplicationInsightsSettings:LogTenTenAzureUploadDependencies": false,
  "ApplicationInsightsSettings:LogSnowflakeDependencies": false,
  "OverrideTenTenFullTablePath": false,
  "NotificationEmailTo": "<your_email_address>",
  "SnowflakeUser": "<your_email_address>",
  "SnowflakeRole": "SNOWFLAKE_PHARMACY_ENGINEERS",
  "SnowflakeWarehouse": "PHARMACY_ENGINEERS",
  "SnowflakeDWDatabase": "UAT_MRXTS_DL_US_INT_ERXDW_DATAINSIGHTS",
  "SnowflakeAUDDatabase": "UAT_MRXTS_DL_US_INT_ERXAUD_DATAINSIGHTS",
  "TenTenAzureBlobEnvironmentFolderName": "dev",
  "TenTenAzureBlobUploadDeveloperMode": false,
  "ParquetUploadNotificationEmailTo": "",
  "SnowflakeDataOutputDirectories": {
    "INN901": "./inn901.txt"
  }
}
```

* `TenTenUrl` is a fake URL that always returns an OK response.

### User Secrets

You can override the call to Azure Vault by specifying secrets in the INN.JobRunner's user secrets. You can copy this example as a starting point.

**NOTE: It's not necessary to have all the secrets filled out. Only the secrets used in dependencies that you are testing locally.**

#### Example

```json
{
  "TenTenUrl": "https://c42c7dec-eedb-420b-8314-2c1bb3b447ad.mock.pstmn.io/",
  "TenTenUsername": "Nothing",
  "TenTenPassword": "Nothing",
  "NetezzaOleConnectionString": "Nothing",
  "McKessonDWTnsDescriptor": "Nothing",
  "McKessonDWUserName": "Nothing",
  "McKessonDWPassword": "Nothing",
  "McKessonCPSConnectionString": "Nothing",
  "InformixUsername": "<Informix_Username>",
  "InformixPassword": "<Informix_Password>",
  "EmplifiURL": "https://americasep24.myastutesolutions.com/WegmansPharmaAIS",
  "EmplifiUserName": "<EMPLIFY_URL>",
  "EmplifiPassword": "<EMPLIFY_PASSWORD>",
  "TenTenSamOwnerId": "wegmans_inn_batch_pool_01",
  "TenTenSamGroudId": "wegmans_inn_batch_pool",
  "TenTenSamPassword": "<TENTEN_SAM_PASSWORD>"
  "NotificationEmailTo": "your.name@wegmans.com",
  "EmplifiNotificationEmailTo": "your.name@wegmans.com",
  "TenTenAzureBlobConnectionString": "<TENTEN_AZURE_BLOB_SAS_TOKEN>",
  "EmplifiTriageNotificationEmailTo": "your.name@wegmans.com",
  "EmplifiEligibilityNotificationEmailTo" : "your.name@wegmans.com",
  "ApplicationInsightsConnectionString": "<CONNECTION_STRING_FOR_pharmbus-ai-test>"
}
```

* `TenTenUrl` is a fake URL that always returns an OK response.
* Instructions on setting up `SnowflakeUnencryptedPublicUserRsaToken` can be found [here](../Library.SnowflakeInterface/README.md#setup-local-environment)

## Adding New Commands

Command code is in the "Commands" folder. To add a new command:

1. Create a new class in the "Commands" folder (or copy an existing command and rename it).
2. In that class, add a method `RunAsync` with a `Command` attribute specifying the command (in lowercase, spaces replaced with dashes), and a `Description` attribute with a description of the command.
3. You can add options to the command with the `Option` attribute on method parameters (See [Cocona Documentation](https://github.com/mayuki/Cocona) for more on what can be done with this).
4. In `Program.cs`, locate the other `app.AddCommands` lines and add your class to it.
5. In `Properties/launchSettings.json`, add a new command to the `profiles` section. This will allow you to debug your command in Visual Studio.
6. Run your new Launch Profile in Visual Studio and test that your command is recognized and enters your new classes `RunAsync` method.

## Known Bugs

### Application starts but runs forever without doing anything

This is a known bug in Cocona that occurs when a dependency is requested but isn't registered. It is currently recognized by the developer and is being actively worked on.

https://github.com/mayuki/Cocona/issues/121

When fixed in a later version, test that it works and delete this known bug.

## Using Parameters

See the classes in the [CommonParameters](./CommonParameters) folder for examples on using the built-in parameters.
