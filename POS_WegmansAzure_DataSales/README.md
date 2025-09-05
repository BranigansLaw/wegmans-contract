# Wegmans Branching Strategy
---

* feat - Feature I'm adding or expanding (PCRs)
    * feat/User_Story_number/short_desc

* bug  - Bug fix (SRs)
    * bug/Bug_number/short_desc

* junk - Throwaway branch created to experiment (testing)
    * junk/User_Story_number/short_desc


Once the feat / bug / junk branches are completed, it must be merged back into the `main` branch and/or discarded.

Begin by starting a new branch `feat/290794/test_checkin`, branching off of `main`:

```
$ git checkout -b feat/290794/test_checkin main
```

Display the state of the working directory and the staging area. The following command will show modified files in your workspace.
```
git status
```
Mark changes to be included in the next commit.
```
git add $PATH_TO_MODIFIED_FILE
```

You should use `feat/290794/test_checkin` to make all changes required for your new feature.

- Make many small commits so that the history of development for your feature branch is clear.
- Avoid merging your feature branch with other feature branches being developed in parallel. 

```
git commit -m "Give a brief message about what these changes are."
```

When your feature is complete, push it to the remote repo to prepare for a pull request.

```
$ git push -u origin feat/290794/test_checkin
```

Next, you will want to [create a pull request](https://docs.microsoft.com/en-us/azure/devops/repos/git/pull-requests?view=azure-devops#after-pushing-a-branch), so that the repository administrator (Dev POS) can review and merge your feature.

* base: `main`, compare: `feat/290794/test_checkin`

Finally, after your pull request is accepted, clean up your local repositories by deleting your local feature branch:

```
$ git branch -d feat/290794/test_checkin
```
---
## Useful tools and documentation.

Overview

This code reads in xml transaction files from the ACE POS and produces a json blob in the transactions container.
And the successful creation of the json file can also cause an event to be published, which contains the path to the blob.
These events are currently triggered when the transaction contains pharmacy, instacart, instacart bypass, shopic, Amazon dash cart or Meals2Go data.
The blobs are sorted into "v1"/<year>/<month>/<day>/<hour> folders, which are automatically created as needed.
Reprocessing the same transaction will overwrite the existing json file and produce another event.
The xml data is renamed from the cryptic vendor labeling to logic names.
The Enterprise Library is required for both the formatting of the json output and the event model.
Not all of the xml data is being used.  Here are the list of currently mapped strings:
case "TransactionRecord_00":
case "TransactionRecord_01":
case "TransactionRecord_02":
case "TransactionRecord_03":
case "TransactionRecord_04":
case "TransactionRecord_05":
case "TransactionRecord_06":
case "TransactionRecord_07":
case "TransactionRecord_08":
case "TransactionRecord_09":
case "TransactionRecord_10":
case "TransactionRecord_11.BD":
case "TransactionRecord_11.DB":
case "TransactionRecord_11.DD":
case "TransactionRecord_11.EE":
case "TransactionRecord_11.FF":
case "TransactionRecord_16":
case "TransactionRecord_99.0.4":
case "TransactionRecord_99.0.50":
case "TransactionRecord_99.0.96":
case "TransactionRecord_99.10.4":
case "TransactionRecord_99.10.11":
case "TransactionRecord_99.10.12":
case "TransactionRecord_99.10.14":
case "TransactionRecord_99.10.26":
case "TransactionRecord_99.10.27":
case "TransactionRecord_99.10.28":
case "TransactionRecord_99.10.29":
case "TransactionRecord_99.10.31":
case "TransactionRecord_99.11.1":
case "TransactionRecord_99.11.2":
case "TransactionRecord_99.11.3":
case "TransactionRecord_99.11.5":
case "TransactionRecord_99.11.6":
case "TransactionRecord_99.11.7":
case "TransactionRecord_99.11.10":
case "TransactionRecord_99.50.10":
case "TransactionRecord_99.50.12":
case "TransactionRecord_99.50.13":

---
[VScode installer](https://code.visualstudio.com/download)

[VScode documentation](https://docs.microsoft.com/en-us/azure/devops/repos/git/pull-requests?view=azure-devops#after-pushing-a-branch)

[GitHub Desktop Client](https://desktop.github.com/)

[GitHub Desktop documentation](https://docs.github.com/en/desktop/contributing-and-collaborating-using-github-desktop/making-changes-in-a-branch)

[Azure DevOps documentation](https://docs.microsoft.com/en-us/azure/devops/repos/?view=azure-devops)

The [Wegmans Documentation site](https://docs.wegmans.tech/index.html) contains guidance on architectures, implementation strategies, DevOps practices and more.

---

# Setup C# environment in VS Code
---
1. Install [GIT](https://git-scm.com/downloads) for Windows.

2. Install [.NET Runtime framework 4.8.](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net48-web-installer)

3. Install [VS Code](https://code.visualstudio.com/download) if you do not have the IDE.

4. Install the [Azure Functions extension](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=csharp#install-the-azure-functions-extension) in VS Code.

5. Install [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) - VS Code extension.

6. Install [Azure Function core tools.](https://github.com/Azure/azure-functions-core-tools#windows)

7. [Clone](https://docs.microsoft.com/en-us/azure/devops/repos/git/clone?view=azure-devops&tabs=visual-studio) DevOPs `POS_WegmansAzure_DataSales` repository into VS Code.

8. Update local.settings.json file. This file should NOT be checked into GIT. The following configuration is for a local environment. Please note that since EL v2, EventGrid access uses managed identities now. See testing examples below for further details.
```JSON
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "POSDataHubAccount": "UseDevelopmentStorage=true",
    "DsarOptions__DsarStorageConnection": "UseDevelopmentStorage=true",
    "RawTlogDeadLetter": "raw-tlog-deadletter",
    "Transactions_container": "transactions",
    "EventPublisherDeadletter": "event-publishing-deadletter",
    "RawTlog_container": "raw-tlog",
    "StoreCloseEvents_container": "store-close-events",
    "RawTlogTransactionsEventQueueName": "raw-tlog-transactions",
    "RawTlogTransactionsPoisonQueueName": "raw-tlog-transactions-poison",
    "RawTlogStoreCloseEventQueueName": "raw-tlog-store-close-events",
    "TlogEventSubscriberQueueName": "event-publisher",
    "WEGMANS__ENTERPRISELIBRARY__EVENTS__PUBLISHER__TOPIC": "pos",
    "WEGMANS__ENTERPRISELIBRARY__EVENTS__PUBLISHER__URI": "[YOUR LOCAL EVENT GRID TOPIC ENDPOINT URI GOES HERE]",
    "CustomerClient__ServerAddress": "https://nonprod.api.wegmans.io/customers/",
    "ProductApiConfig__BaseAddress": "https://wegmans-es.azure-api.net/product/",
    "ProductApiConfig__ApiKey": "",
    "ItemFunctionAppURL": "http://localhost:7071",
    "priceApiConfig__BaseAddress": "https://wegmans-es.azure-api.net/price/",
    "priceApiConfig__ApiKey": "",
    "priceFunctionAppURL": "http://localhost:7071",
    "ReprocessTransactions__MaxRawTLogQueueSize": 150000,
    "ReprocessTransactions__MaxDegreesOfParallelism": 150,
    "ReprocessTransactions__CronSchedule": "0 */10 * * * *",
    "AzureWebJobs.ReprocessTransactionsSubscriber.Disabled": true,
    "AzureWebJobs.CheckReprocessSettings.Disabled": true
    "AzureWebJobs.CheckReprocessTableConfigurations.Disabled": true
  }
}
```
9. Install [NPM for Windows.](https://nodejs.org/en/download/) Once installed run the following CMD command as ADMIN.
```
npm install -g azurite
```

10. Install [Azure Storage Explorer.](https://azure.microsoft.com/en-us/features/storage-explorer/#overview)

11.  Create the following Containers and Queues in Azurite or run tools/New-LocalhostAzuriteSetup.ps1 from the VSCode terminal
    - Containers
        - raw-tlog-deadletter
        - raw-tlog
        - transactions
        - store-close-events
        - event-publishing-deadletter
    - Queues
        - raw-tlog-transactions
        - event-publisher
        - raw-tlog-store-close-events
---

## Azurite and Local Environment
---
1. Use the [Azurite emulator](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio) for local Azure Storage development.

2. If you're using Visual Studios Azurite is automatically available. Execute the code and open [Azure Storage Explorer.](https://azure.microsoft.com/en-us/features/storage-explorer/#overview)

3. In `Local & Attached` Storage Account open `Emulator.`

4. Create container & queues listed above in emulator. 


# How to add new strings for ACE to JSON mapping to current code

## Prerequistes
---

- Have a working development environment
- Able to run a transaction in development environment with XML file trigger and create a JSON file out to **POSDataHubAccount**. (i.e. VS code with Rest Client extension)
- Find *.XML* file which has been written to hub that contains the string that you are trying to map or ask DEV POS to provide one (for new features)
    - Ex. https://possalesdatahubtest.blob.core.windows.net/raw-tlog/2022/01/18/14/0278_0005_0074_1642609276038_transaction.xml


Steps:

There are 4 Categories of data being added to outgoing JSON object
1.  As an non-collection object
    * MapTo
2.  As a collection object
    * MapToCollection
3.  As a nested collection
    * MapToCollectionItem
3.  As specific item of a nested collection (as far as I can tell)
    * MapToCollectionForItem


The information below will focus on adding strings that are straight one to one mapping.  Any complicated strings (ex. 01, 02 etc.) require more complex work, which is outside the scope of this document.

1.  Verify **ACETransactionModel/TransactionLog.cs** is setup for the string to be mapped
    - Find the string name ex. (TransactionRecord11EE)
        - If it doesn't exist, then we have to create the class.
            - Look at the EnterpriseWegmansTransactionLog.xsd for specification, most likely it doesn't exist in it, if it's not part of *TransactionLog.cs* already
            - Otherwise look at the file *EnterpriseWegmansTransactionLogSchema.xml* for string information and create the class.
                - New fields should be accompanied by a new EnterpriseWegmansTransactionLogSchema.xml, which should be updated in this solution
                - Many examples in *TransactionLog.cs*
    - Ensure that the string is part of **public class HubTransaction** class.
        - Add to Collection if there is possibility of multiple strings in the same transaction.
            - ```public List<TransactionRecord11DD> TransactionRecord11DD { get; set; }```
2.  Update Enterprise library for mapping (see. [Enterprise Library Core](https://dev.azure.com/wegmans/EnterpriseLibrary/_git/EnterpriseLibrary.Core) 
    - Create new class by copying existing class (i.e. /src/Data/Hubs/src/POS/Transaction/v1/PaymentProcessorRequest.cs)
        - The names of the functions should match the function name in the Class that was found or created in Step 2. above.
            - This allows for AutoMapper to map using the function names
        - If a custom output name is needed then use the JsonPropertyName to specify (see below)
            - ```[JsonPropertyName("relativerecordnumber")]```
            -  ```public double RRN { get; set; }```
        - Alternatively (Preferred method) you can change the variable name in the TransactionLog.cs
            ```C#
            [XmlElement(ElementName = "CAUPC")]
            public string UniversalProductCode { get; set; }
    - Update **/src/Data/Hubs/src/POS/Transactions/v1/Transaction.cs** file for location of the data in the JSON object
        - Example of adding **non-collection object** to Main JSON view
            - ```public PaymentProcessorRequest? PaymentProcessorRequests { get; set; }```
                - Declare the PaymentProcessorRequest object
                ```json
                {
                    "storeNumber": 69,
                    "dateTime": "2021-06-30T07:29:00",
                    "operator": "00",
                    "grossNegative": 0,
                    "items": [],
                    "paymentProcessorRequests": {
                        "hostID": "WF",
                        "totalsType": 1,
                        ...
                    },
                    ...
                }
                ```
        - Example of adding **collection** object to main JSON view
            - ```public IEnumerable<PaymentProcessorRequest>? PaymentProcessorRequests { get; set; }```
                - Since PaymentProcessorRequest is a collection, we are going to declare it has an IEnumerable
                ```json
                {
                    "storeNumber": 69,
                    "dateTime": "2021-06-30T07:29:00",
                    "operator": "00",
                    "grossNegative": 0,
                    "items": [],
                    "paymentProcessorRequests": [
                        {
                        "hostID": "WF",
                        "totalsType": 1,
                        ...
                        },
                        {
                        "hostID": "WG",
                        "totalsType": 1,
                        ...
                        }
                    ],
                    ...
                }
                ```
        - Example of adding **boolean** object to main JSON view (which would be needed if an event should be triggered by this data)
            - ```public bool? IsPharmacyTransaction { get; set; }```
        - If you want to have events created for specific transaction, then booleans also need to be added in **/src/Events/src/Models/POS/TransactionV1BlobCreated.cs**
    NOTE: Approved pull requests for the Enterprise Library will be available only as prerelease NuGet packages, until the next scheduled build. 
            Changes to the Transaction.cs will require updating the Wegmans.EnterpriseLibrary.Data.Hubs package.
            Changes to the TransactionV1BlobCreated.cs will require updating the Wegmans.EnterpriseLibrary.Events package.
3.  Update the profile for the string to do the mapping:
    -For most standard strings do the update in: **MappingConfigurations/ProfileStandardStrings.cs**
        - Add a the mapping between the ACETransactionModel and the JSONOutputModel to the *ProfileStandardStrings()* function
            - Ex. ```CreateMap<TransactionRecord16, PaymentProcessorRequest>();```
    - For non-standard standard strings (ones that have indicate values)
        - Copy the Profile02String.cs to Profile{StringNumber}String.cs
        - Update the new file/class to the new name
        - Add the custom mapping like below:
            - Ex. ```.ForMember(dest => dest.HasCouponQuantity, opt => opt.MapFrom(src => src.Indicat1.Bit0.ToNullableBoolean()))```
                - replace *.HasCouponQuantity* with destination mapping name (ie. JSON model)
                - replace *..Indicat1.Bit0* with source mapping name (ie. ACE model)
                - everything else should remain unchanged
                - Please note in this mapping we also call an extension method to make the boolean nullable.
4. Update **TransactionController.cs** function *updateTransactionData*
    - Create a case statement in the switch for the string to be mapped:
        -  The name of the case will be TransactionRecord_[name of the string derived from *EnterpriseWegmansTransactionLogSchema.xml*], or look in the existing XML
        - Ex. ```case "TransactionRecord_16":``` for 16 string 
        - Ex. ```case TransactionRecord_11.CC``` for 11.CC string
    - Now configure the mapping inside the case statement:
        - For non-collection
            - Ex. ```MapTo<TransactionRecord16, PaymentProcessorRequest>(record, outputTransaction);```
                - **TransactionRecord16** comes from *ACETransactionModel/TransactionLog.cs* class in Step 1.
                - **PaymentProcessorRequest** comes from the *JSONOutputModel/{class from Step 2.}
                - **record** - do not change this - this remains the same
                - **outputTransaction** - do not change this - this remains the same
        For collection
            - Ex. ```MapToCollection<TransactionRecord16, PaymentProcessorRequest>(record, outputTransaction.PaymentProcessorRequests);```
                - **TransactionRecord16** comes from *ACETransactionModel/TransactionLog.cs* class in Step 1.
                - **PaymentProcessorRequest** comes from the *JSONOutputModel/{class from Step 2.}
                - **record** - do not change this - this remains the same
                - **outputTransaction** - do not change this - this remains the same
        NOTE: There are examples of the other two types of categories of data as well (MapToCollectionItem & MapToCollectionForItem)
                but if you use the wrong one (i.e. MapToCollectionItem instead of MapToCollection) the program will hang indefinitely.
5.  Run the pre-requiste **.xml** file through and hopefully a **.json** is created with the new string being mapped.

# Running a development test
## XML to JSON mapping test
---
- [Local Development Testing](https://docs.wegmans.tech/engineering/integration/develop/azureFunctions.html#local-development-testing).
- Download XML from Raw-Tlog container in `possalesdatahub` Storage account.
- Add XML into Azurite environment
- After you upload file to Azurite right click and `Copy URL.`
- Update the localHost.http file for the new XML data
    * Paste your URL (from prior step) in place of `http://127.0.0.1:10000/devstoreaccount1/input/0045_0020_0005_1638569711656_corrected_transaction.xml` in the message below....
    ```YAML
        ###
        POST http://localhost:7071/admin/functions/TransactionSubscriber
        Content-Type: application/json
        {
            "input": "[{\n  \"topic\": \"/subscriptions/2a19ee82-f2f4-4fe3-b08b-58f92a7c69d2/resourceGroups/Storage/providers/Microsoft.Storage/storageAccounts/playingaroundstorage\",\n  \"subject\": \"/blobServices/default/containers/raw-tlog/Tlogs/0222_0066_0001_1631210673634_transaction.xml\",\n  \"eventType\": \"Microsoft.Storage.BlobCreated\",\n  \"eventTime\": \"2021-07-02T18:41:00.9584103Z\",\n  \"id\": \"831e1650-001e-001b-66ab-eeb76e069631\",\n  \"data\": {\n    \"api\": \"PutBlockList\",\n    \"clientRequestId\": \"6d79dbfb-0e37-4fc4-981f-442c9ca65760\",\n    \"requestId\": \"831e1650-001e-001b-66ab-eeb76e000000\",\n    \"eTag\": \"\\\"0x8D4BCC2E4835CD0\\\"\",\n    \"contentType\": \"text/xml\",\n    \"contentLength\": 67875,\n    \"blobType\": \"BlockBlob\",\n    \"url\": \"http://127.0.0.1:10000/devstoreaccount1/input/0045_0020_0005_1638569711656_corrected_transaction.xml\",\n    \"sequencer\": \"00000000000004420000000000028963\",\n    \"storageDiagnostics\": {\n      \"batchId\": \"b68529f3-68cd-4744-baa4-3c0498ec19f0\"\n    }\n  },\n  \"dataVersion\": \"\",\n  \"metadataVersion\": \"1\"\n}]"
        }

    ```
## XML to JSON exception test (Poison queue)
---
- Create an exception
    - Create a message directly on the **raw-tlog-transactions** queue
        - Change the "data" field to "Data"
            - The function will be executed 5 times, then the data will be moved to Poison queue
        - Change the encoding to **UTF8** from **Base 64**
> NOTE: There is likely a way to do this using the REST API call, however currently using that approach only leads to single call instead Azure working through DequeueCount to 5 and then moving the message to Poison queue.
## XML to JSON DeadLetter development test
---
Create an exception
    - Upload deadletter json (see *tests\http\deadletter.json*) to raw-tlog-deadletter container
    - Execute the **Send Request** in **processDeadLetterLocalhost.http**
    - The execution should should show function **TlogProcessDeadLetter** getting called
        - Then transition to processing the deadletter in **TlogSubscriber**
> NOTE: One could change the "data" field to "Data" in the deadletter.json if wanted to excercise the Poison to Failed queue as well during the Deadletter process execution.

## Event subscriber and publish CloudEvent development test
---
    NOTE: This is one method of testing.  You can also use just step 3, as long as your local.settings.json file contains a valid url for the eventpublisher.
    1. Connect Azurite to connect to MSDN account which accepts CloudEvent subscription
            -  Under your MSDN account, create Event Grid Topic resource using cloud event schema (copy endpoint uri to paste in local.settings.json shown below)
               - Add a new 'Job function roles' role assignment to the topic for 'EventGrid Data Sender' role using 'User, group, or service principal', and add yourself as a member.
            -  Then create event subscription using cloud event schema (This is currently how events are created.  Event Grid System Topic can handle this or event grid schema)
                -  Then create storage queue endpoint called event-publisher
    2. Update local.settings.json with the following:
```json
        - "TlogEventSubscriberQueueName": "event-publisher",
        - "WEGMANS__EVENTS__PUBLISHER__TOPIC": "POS",
        - "WEGMANS__EVENTS__PUBLISHER__URI": "https://devpharm-ras-event-publisher-test2.eastus2-1.eventgrid.azure.net/api/events", <use uri from step 1>

```
    3. Update the **localhost-post-tlog-event-subscriber.http** with the json file to be processed (as described in local development testing section above)
      Troubleshooting tips:
        - Put a break point at the "await _publisher.SendEventAsync" line of the Wegmans.POS.DataHub\EventPublisher.cs (currently line 66)
        - Execute the **Send Request** in **localhost-post-tlog-event-subscriber.http**
        - The execution should should show function 'PublishTransactionV1BlobCreatedEvent' getting called
        - Refer to the locals tab, to verify that "data" contains the proper boolean values for the type of transaction you processed (i.e. IsPharmacy = true while the rest are false)
    4. Verify that the queue shows the updated message
    - Ex.
```json
{
    "id": "9e26022c-0cb7-4ebf-a47c-f5f8b6bfa8d0",
    "source": "https://events.wegmans.cloud/pos",
    "type": "cloud.wegmans.events.pos.transaction",
    "data": {
        "source": "http://127.0.0.1:10000/devstoreaccount1/transactions/v1/2021/09/03/09/0_0001_0037.json",
        "isInstacart": true
    },
    "time": "2021-09-14T20:20:55.8894062+00:00",
    "specversion": "1.0",
    "datacontenttype": "application/json"
}

```
## Health Check monitor testing
---
    - Create a test storage account with all the containers and queues that are part of health check monitor.
        - Note: Azurite can used for queue monitoring test, however there are some interface issues between Azurite and App-Insights application for containers
    - Create App-Insights account for publishing the data
    - Update local.settings.json with the following:
```json
        - "APPINSIGHTS_INSTRUMENTATIONKEY": "{instrumentation key}",
        - "POSDataHubAccount": "{Key from Azure storage account}"
```
    - Run test that create exceptions and check the `App-Insights - Availability` section
        - See **XML to JSON exception test**

# Generate App Registrations Keys for SIUSERDI
---
1. Log into [Azure Portal.](https://portal.azure.com/#home)
2. Search for `App registrations` in Azure Portal. 
3. Select Display Name dev.azure.com/wegmans/PointOfSale.
4. Select Certificates & secrets.
5. Fill in Description field - Specify Test vs Prod. 
6. Extend Expiration date to 24 Months.
    * Process will be swapping keys every 12 Months.
7. Add `New Client Secret`
    * Once azure generates the secret copy the value. After the page is refressed the value will be masked. This value will be required in siuserdi.
 
## ADX_IPGM:SIUSERDI.PRO Configuration
---
 
Modify ADX_IPGM:SIUSERDI.PRO - `TEST`
```
WegmansXMLTlogTransactionTrickleActor.EndPointURL=https://possalesdatahubtest.blob.core.windows.net/raw-tlog
WegmansXMLTlogTransactionTrickleActor.ClientSecret=CHANGE_TO_VALUE_FROM_AZURE
```
 
Modify ADX_IPGM:SIUSERDI.PRO - `PROD`
```
WegmansXMLTlogTransactionTrickleActor.EndPointURL=https://possalesdatahubprod.blob.core.windows.net/raw-tlog
WegmansXMLTlogTransactionTrickleActor.ClientSecret=CHANGE_TO_VALUE_FROM_AZURE
```

## Troubleshooting Remedy Incident
---

Unhealthy Azure Availability will generate automatic tickets. The linked [Remedy](https://wegmans-smartit.onbmc.com/smartit/app/#/incident/AGGG7X89BYDHJAREMX4IRDNRUJYFSO) ticket is an example. You will notice there is **NOT** a ton of information in the incident. You will need to troubleshoot in Azure. 

When an incident is generated:
1. Travel to [Azure.](https://portal.azure.com/#home)
2. Navigate to [Subscriptions.](https://portal.azure.com/#blade/Microsoft_Azure_Billing/SubscriptionsBlade) Select *__POS - Point Of Sale - PROD.__*
3. Select *Resources.*
4. Select *possalesdatahub-prod-eastus2* -> Type is Application Insights.
5. ![Select Availability.](../diagrams/Supporting_Images/AI_Availability.PNG)
6. Troubleshoot the Availability that is <100%.

Most availability alerts will be triggered from *TlogSubscriberHub:raw-tlog-transactions-poison.*

Troubleshoot why message went to poison. Once issue has been remediated you may [PIM - Privileged Identity Management](https://docs.wegmans.tech/azure/best-practices/pim.html?q=PIM) up and reprocess the messages by moving them from *raw-tlog-transactions-poison* to *raw-tlog-transactions* queue.

## Known issues
---
1. Whenever Toshiba's CheckOut support app has an issue (i.e. can't handle an ampersand in the credit customer name), the send us a filenamed with the defaults (9999). We fix the data when possible but store number is only in the file for pharmacy and loyalty customers. So we currently do NOT send those to the poison queue but Karen Neary did ask Toshiba to include that data in the file.  Until then, if we want to commit to manually fixing these (instead of the accountants), we could send them to the poison queue and attempt to determine the store number from the user id or lane with that missing transaction number. 
2. Sometimes transactionas end up in the poison queue because they are missing 99.10.27 records (which the API call for item lookup depends on).  This has happened multiple times since that functionality was deployed and were WIC transactions.  The file can be manually fixed for items that have a weight or type 2 UPC and reprocessed but should also be reported to Toshiba on case#14633002
3. The new Dash Cart stores are sending us malformed 99.10.31 records with do NOT trigger the IsAmazonDashCart flag.
4. When we double manufacturer coupons, the raw tlog contains a store coupon record as well, which does not contain enough data for us to assocaite it to a wegmans code.  We have asked the downstream teams to create a default for coupons without a code.
5. We are now dependent on the product & price API (to handle items sold by EA or with type 2 UPCs) and the customer API for cuid. So if either is down for a significant period of time, then transactions will be going to the poison queue.
6. Door Dash orders are causing us to get 99.10.26 records that are NOT instacart.  So to prevent us from incorrectly flagging these as Instacart, we are excluding the BINs assocaited with this vendor.  We will need to expand this list as more vendors are added (i.e. Uber Eats). Also the Instacart collection is still populated, so if we had a databricks for this hub we could identify these orders.
7. Most downstream subscribers ignore reprocessed orders by excluding events where is republished is true, this means if we want to fix a transaction for them, we would have to delete the existing JSON before reprocessing the transaction so that our code wouldn't say the JSON was republished.

## Future Opportunities in Transaction Hub
---
- [ ] Remove unnecessarily reported weights - (ie. fixed weight items)
- [ ] Logic for weighed items could be made simpler (i.e. use flag for required price rather than penny price)
- [X] Deal with type 2 UPCs (i.e. Add estimation logic or calculated quantity field) - Completed in September
- [X] Add Elera flag - Completed (added an is ace transaction flag)
- [ ] Add strings or create dedicated hub for Secure5

## Reprocessing raw Transactions by source uri for DSAR
---
Under the Wegmans.POS.Datahub project, you will find the Dsar.cs and DsarOptions.cs, which were designed to make remove customer loyalty information from the raw tlog XMLs associated with a specified list of historical transaction blobs (jsons). At the time of this writing, the code will extract the path of the raw tlog blob and then replace the values in the loyalty fields with a series of 1's, which triggers the overwriting of the related json with the new data. This was implemented so we could pass it the transaction uris of customers that have requested their data be de-identified. The entire DSAR process is detailed here https://wegmans.sharepoint.com/sites/DevStore&Rx/_layouts/OneNote.aspx?id=%2Fsites%2FDevStore%26Rx%2FSiteAssets%2FDev%20Store%20%26%20Rx%20Notebook&wd=target%28DataSales%20Hub.one%7CB60BD1E9-ACE8-4A2A-85ED-9339B2C8F1BC%2FHandling%20of%20requests%20using%20Azure%20function%7C735FF53C-ADF8-4589-8E30-96D6C850BCDD%2F%29
onenote:https://wegmans.sharepoint.com/sites/DevStore&Rx/SiteAssets/Dev%20Store%20&%20Rx%20Notebook/DataSales%20Hub.one#Handling%20of%20requests%20using%20Azure%20function&section-id={B60BD1E9-ACE8-4A2A-85ED-9339B2C8F1BC}&page-id={735FF53C-ADF8-4589-8E30-96D6C850BCDD}&object-id={E630364A-4650-059E-1B40-7BAE831FA4D6}&10 .  The overview of this process is as follows:
* Based on the customer's loyalty number, use databricks to create the list of uris to deidentify
* In the json-uris container, update the list-of-json-to-deidentify.csv.  This will automatically trigger the DSAR function.  After a few minutes, Application insights can be used to verify the number of files updated.
 
## Reprocessing Transactions by source uri (works for less than 1000 transactions at a time)
---
Under the Wegmans.POS.DataHub project, you will find a BatchModifier folder containing code designed to make edits to a specified list of historical transaction blobs (jsons). At the time of this writing, the code has only one "UpdateQuantity" function, which was implemented to mitigate the inflated quantities reported after one of our enhancements. Future functions could be added to the BatchModifierMethodsImp.cs and simply specified during the http call. The code will also set the hasBeenRepublished flag on each transaction so that the events created can be filtered by downstream consumers as needed. The process currently works as follows:
* Deployment creates the batch-modifier queue and batch modifier http endpoint can be called directly from your VM if you do the following:
  * PIM up
  * In the Azure portal, navigate to the function app but click the subheader called Get Function URL.  Copy the default function key and paste it at the end of the POST command.
  * Update the list of itemPaths and modifiersToRun in the body of the POST command below. The itemPaths should be the full path to the transaction blob and is the same format as the sourceuri column in databricks. NOTE: Trying to send more than a thousand uris at once seemed to confuse VS.
  * Execute the POST command (shown below and also located in a couple of files in the tests/http folder) 
  * Application Insights will see the function triggered but significant logging has not been implemented. 

POST https://possalesdatahub-test-eastus2.azurewebsites.net/api/BatchModifierHttpEndpoint?code=<AZURE_FUNCTION_KEY>
Content-Type: application/json

{
   "itemPaths": [
      "https://possalesdatahubtest.blob.core.windows.net/transactions/v1/2020/05/28/08/281_0001_0001.json",
      "https://possalesdatahubtest.blob.core.windows.net/transactions/v1/2020/05/28/08/281_0009_0001.json"
   ],
   "modifiersToRun": ["UpdateQuantity"]
}

## Reprocessing Transactions by date range
---
Under the Wegmans.POS.DataHub project, you will find a ReprocessTransactions folder containing code designed to make edits to a number of historical transaction blobs (jsons) over a date range. At the time of this writing, the code will lookup and add a customer CUID if a loyaltyNumber is present, and remove the loyaltyNumber and customerNumberAsEntered from a range of historical transactions. The code will also set the hasBeenRepublished flag on each transaction so that the events created can be filtered by downstream consumers as needed. This code could be re-used in the future, but the ReprocessTransactionsSubscriber function would need to be updated if different edits are needed. The process currently works as follows.  

* Create and upload a file called ReprocessTransactionsSettings.json to the reprocess-transactions container containing the StartDate, EndDate, and NextDateToProcess for the date range you want to reprocess (see sample below).  
* The ReprocessTransactions_BlobTrigger function will be triggered by the upload of the settings file and will kick off the ReprocessTransactions_Orchestrator function.  
* The ReprocessTransactions_Orchestrator function will call the ReprocessTransactions_GetNextFolderToProcess function to get the next date to process. If the next date falls outside the start/end date range, the function will exit.  
* If a valid date is returned, the ReprocessTransactions_Orchestrator will asynchronously trigger the ReprocessTransactions_QueueTransactions function for each hour of the date, which will in turn send the URL of each transaction blob in that hour to the reprocess-transactions queue.  
* Once all of the asynchronous functions have completed, the orchestrator will call the  ReprocessTransactions_SetNextFolderToProcess function to update the NextDateToProcess field in the ReprocessTransactionsSettings.json blob, which will in turn trigger the ReprocessTransactions_Orchestrator function for the next date.
* Independently of the orchestrastor, the ReprocessTransactionsSubscriber function will be triggered by the messages in the reprocess-transactions queue and will call the ReprocessTransactions_BlobTrigger function for each transaction blob.  It will also try multiple each message multiple times before sending it to the reprocess-transactions-poison queue. Note that every deployment of this repo will disable this function and you will need to PIM up to enable it.
* NOTE: This method was chosen to avoid the time out limit of azure functions. The slowest part of the process is the subscriber, which takes about 8 hours per month of transactions.  The cost associated with running this non-stop  ~ 9k/day.  And to limit exceptions from the customer API and network latency, we turned down the batch size to 4.
* Sample ReprocessTransactionsSettings.json  

``` json
{"StartDate":"2024-02-12T00:00:00-05:00","EndDate":"2024-02-12T00:00:00-05:00","NextDateToProcess":"2024-02-12T00:00:00-05:00"}
```
