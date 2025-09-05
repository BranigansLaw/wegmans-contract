# Library.TenTenInterface

This library manages uploads and queries to TenTen.

## Uploads using Azure Blob

Uploading Parquet files to TenTens on-prem Azure Blob Storage will kick off a process on TenTen's end that will read the Parquet files and then push them to a pre-configured TenTen feed.

### What are Parquet files?

Parquet is a file format for data files. The files are in binary format and are not readable like a JSON or XML data file. They can be queried, appended, or updated like a database, but for the purposes of TenTen uploads, we are only generating the files and then uploading them to TenTen's Azure Blob, where they are then processed into TenTen feeds (appending, deduplicating, etc).

#### Reading Parquet Files

This utility is built with Parquet.Net which we use for serializing Parquet files, and is a great way to view a generated parquet file to diagnose any issues:

https://github.com/aloneguid/parquet-dotnet/releases

### Diagram

[![](https://mermaid.ink/img/pako:eNp1UctOwzAQ_BXLJxBpP8CHSq1CuaFKCUJCuSzx0Fg4dtjYh7bqv-OIJILSWj7s7GN2dvcka68hlezxFeFq5Ib2TG3lRHq7hril-rCJvXHo-8Vq9VDCpS_Wx8gQG-vflXhCKAKD2rv7n7p_OalwcUmmBA3xIfzKJoBv99wRJ3Vh-fxYKjGCAmzImiN4OZulH3VoCpRd8o_ifpFdl_XSWU8a-vYs0xK2gFZirbXIU8M_-UNooL-yrhwWAXMbsTUWMpMtkg6j0y1OA1MlQ4MWlVTJ1MSflazcOeVRDL44uFqqwBGZZB_3jVQfZPuEYpdmn644eztyb95P-PwNkOysbQ?type=png)](https://mermaid.live/edit#pako:eNp1UctOwzAQ_BXLJxBpP8CHSq1CuaFKCUJCuSzx0Fg4dtjYh7bqv-OIJILSWj7s7GN2dvcka68hlezxFeFq5Ib2TG3lRHq7hril-rCJvXHo-8Vq9VDCpS_Wx8gQG-vflXhCKAKD2rv7n7p_OalwcUmmBA3xIfzKJoBv99wRJ3Vh-fxYKjGCAmzImiN4OZulH3VoCpRd8o_ifpFdl_XSWU8a-vYs0xK2gFZirbXIU8M_-UNooL-yrhwWAXMbsTUWMpMtkg6j0y1OA1MlQ4MWlVTJ1MSflazcOeVRDL44uFqqwBGZZB_3jVQfZPuEYpdmn644eztyb95P-PwNkOysbQ)

#### Azure Parquet File and Folder Naming

The Parquet file folder and file naming conventions are set by TenTen here: https://wegmans.sharepoint.com/sites/DevStore&Rx/_layouts/OneNote.aspx?id=%2Fsites%2FDevStore%26Rx%2FSiteAssets%2FPharmacyandInovationTeamHub&wd=target%281010%20Azure.one%7C32D16BE9-D66F-4608-B0EC-CFB4FB8B2541%2FAzure%20Naming%20Conventions%7C33A91868-A9BA-4FF0-8838-1F7EB81EBD8F%2F%29

The formula generally is:

`<FEED_NAME_UPPERCASE>/<FEED_NAME>_<DATA_DATE>_<UPLOAD_DATE_AND_TIME>.parquet`

The `DATA_DATE` relates to the run date of the data. The `UPLOAD_DATE_AND_TIME` relates to the time the file was created.

The reasoning behind this is that if `DATA_DATE` has already been uploaded, TenTen will update entities that were previously created in previous files with a matching `DATA_DATE` and update it with the new values. The exact rules of how TenTen will act with a new file can be discussed with TenTen when you alert them of your new feed.

#### Publishing a Feed (How a feed goes from a Parquet file into TenTen?)

At the time of writing, there is no official process for moving from a Parquet file to a feed for the first time. TenTen handles these requests ad-hoc as we're getting setup.

When you create a new feed, you can reach out to Steven Stine *steven.stine@symphonyai.com* and CC Alex Casanova *alex.casanova@symphonyai.com* with the name of your new feed and what should be done with it. Typical options would be:

- Create a new feed based on historical data from another feed and start appending the Parquet files to that (note: you can not append to a feed that is already active or actively has TenUps/API methods appending to it)
- Create a new feed just containing the data you are uploading

**NOTE: Feeds will NEVER start appending to an existing feed regardless of how they are named, so there is no danger of naming a feed after an existing feed before contacting TenTen.**

#### Update Period

At the time of this writing, TenTen is checking every 15 minutes for new feed data, so the update period should be about 15 minutes.

#### Parquet File Deletion

Deletion handling of the Parquet files you upload is handled by TenTen and occurs right after the Parquet file is injested.

** NOTE: At the time of this writing, this is not currently implemented, but is planned by TenTen.**

### Local Testing

The token to upload data to TenTen is write-only for security reasons. If there are issues with the file uploaded, request that TenTen send you the file to investigate.

For local debugging purposes, set the app setting `TenTenAzureBlobUploadDeveloperMode` to `true`. This will create the parquet file locally on your machine so you can use the [viewer](#reading-parquet-files) to check that the file generated is working as expected. The generated file will be under *\PharmacyBusiness\Source\INN.JobRunner\bin\Debug\net8.0-windows*.

### Diagnosing Issues

For security purposes, the Parquet files are never saved on the Wegmans or TenTen side. If there are issues with data:

1. View the data in TenTen to determine the issues
2. Regenerate the Parquet file [locally](#local-testing), view it in the [viewer](#reading-parquet-files), and modify the code till the resulting Parquet file is correct
3. Deploy your changes to production
4. Rerun the job that generates the parquet file

#### Cases where the Parquet File can't be loaded into TenTen

In the case that a Parquet file can't be loaded into TenTen:

1. Try regenerating the file locally. If there are issues generating locally, investigate why
2. If the file regenerates locally, try rerunning the job in production and see if the file gets generated
3. If there are specific issues with generating in production, try pointing your local instance at the production server in question by modifying the connection string in `local.settings.json` (it will override the `appsettings.json`). If that fails, you can also create an integration test in the [Integration Tests Command](../INN.JobRunner/Commands/IntegrationTests.cs) on your local branch that generates the Parquet file and then run the integration tests on the production server using the [Developer Pipeline](https://dev.azure.com/wegmans/Pharmacy/_build?definitionId=3212).

## Uploads using API

**NOTE: Going forward, this will not be the way we upload data to TenTen, and we'll instead use the [Azure Blob Upload Method](#uploads-using-azure-blob).**

Uploads are managed by 2 files:
- `<NAME>.xml` - the UploadXmlTemplateHandlers/XmlTemplates folder contains the XML body for these data uploads. They require 4 `string.format` fields:
  - {0} - The data that will be uploaded
  - {1} - the `mode` of the upload (which is changed dynamically based on the upload type)
  - {2} - the TenTen folder path of the table (added dynamically based on the environment)
  - {3} - the date to run for (default is today's date)
- `<NAME>.cs` - the UploadXmlTemplateHandlers/Implementations contain class implementations of `ITenTenUploadConvertible`. These classes contain the relevant data fields that will be mapped to TenTen, a method that references the name of the related XML file, and the method to convert the object into an XML string

With the class in place, you can then easily call an upload with some data on the `ITenTenInterface.CreateOrAppendTenTenDataAsync` method.

```csharp
    public class UploadClass : ITenTenUploadConvertible
... 
    IEnumerable<UploadClass> data = new List<UploadClass> { ... };
    await _tenTenInterface.CreateOrAppendTenTenDataAsync(data);
```

### Creating a New Upload

1. Copy an existing template from the `UploadXmlTemplateHandlers/XmlTemplates` folder and rename it as you want.
2. Copy the XML upload body into the new file.
3. Copy an existing class from `UploadXmlTemplateHandlers/Implementations` and rename it to match the XML file you created.
4. Change the `BaseTemplatePath` to return the name of the XML template you created
5. Change `ReplaceExistingDataOnUpload` to true if you want it to always overwrite the data in the TenTen table, or false if you want it to append to the existing data.
6. Add all the fields to this new class related to what you want to push into the XML template you created (Usually GitHub Copilot can help you with this)
7. Implement the `ToTenTenUploadXml` method mapping the fields of the class in order

### Understanding som key challenges with uploading to 1010data

First of all, 1010 does not have a DELETE command to delete a prior upload.
We have just two upload commands to chose from: eitherer APPEND or REPLACE.
If we choose APPEND and an upload runs twice (when an upload job is rerun), then you can expect duplicate rows of data...or triplicate rows on a third rerun, etc.

So, we always upload data with a REPLACE command to a dedicateed table named having the Run For Date in the table name.
In the event of a second job run (maybe an upload job worked initially but failed on a last step, and somebody just restarted it), or even a third rerun and so on, the upload command keeps replacing the data so that no delete command is needed.
This strateegy worrks great for managing data integrity when uploading one specific date to one dedicated table for just that one date.
With 365 days per year, then we can expect 1,095 individual tables after three years...and so on.
Having so many tables will make site navigation difficult, so we maintain these individually dated tables in their own sub folders.
Nobody will directly query any of thse individually dated tables for regular reporting purposes.

#### Example folder name to contain all dated tables for Net Sales:
```
 wegmans.devpharm.prod.net_sales
```

Note that the folder dedicated to dated sub-tables (i.e., folder name="net_sales" and folder title="Net Sales") might need to be created manually in the TenTen environment with a valid set of permissions.
So, you may need to ask a TenTen admin to create the folder for you.

#### Example dated table names (i.e., table name="d20240201" and table title="Net Sales 20240201") within the "net_sales" folder:
```
 wegmans.devpharm.prod.net_sales.d20240201
 wegmans.devpharm.prod.net_sales.d20240202
 wegmans.devpharm.prod.net_sales.d20240203
```

Following an upload to a dated table, we run another process to merge all the individually dated tables into one big rollup table for usee in queries and reports.
Queryable tables (big rollups of smaller individually dated upload tables) are located outside of the folder containing individual dated tables.

####  Environment paths for the big rollup tables:
```
 wegmans.devpharm.prod
 wegmans.devpharm.test
 wegmans.devpharm.cert
```
