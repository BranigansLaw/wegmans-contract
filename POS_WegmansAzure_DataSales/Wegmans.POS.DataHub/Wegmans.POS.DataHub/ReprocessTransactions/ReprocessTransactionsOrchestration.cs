using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Wegmans.POS.DataHub.ReprocessTransactions;

public class ReprocessTransactionsOrchestration
{
    private const string Connection = "POSDataHubAccount";
    private const string TransactionsContainerName = "transactions";
    private const string ReprocessContainerName = "reprocess-transactions";
    private const string ReprocessQueueName = "reprocess-transactions";
    private const string SettingsFileName = "ReprocessTransactionsSettings.json";

    private readonly IOptions<DsarOptions> _options;

    public ReprocessTransactionsOrchestration(IOptions<DsarOptions> options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    [FunctionName(nameof(ReprocessTransactions_Orchestrator))]
    public static async Task ReprocessTransactions_Orchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var folderToProcess = await context.CallActivityAsync<string>(nameof(ReprocessTransactions_GetNextFolderToProcess), null);

        if (string.IsNullOrEmpty(folderToProcess))
        {
            return;
        }

        var parallelTasks = new List<Task>();

        for (int i = 0; i < 24; i++)
        {
            Task task = context.CallActivityAsync(nameof(ReprocessTransactions_QueueTransactions), $"{folderToProcess}{i:00}/");
            parallelTasks.Add(task);
        }

        await Task.WhenAll(parallelTasks);

        await context.CallActivityAsync(nameof(ReprocessTransactions_SetNextFolderToProcess), folderToProcess);
    }

    [FunctionName(nameof(ReprocessTransactions_BlobTrigger))]
    public async Task ReprocessTransactions_BlobTrigger(
        [BlobTrigger($"{ReprocessContainerName}/{SettingsFileName}", Connection = Connection)] Stream settingsBlob,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        string instanceId = await starter.StartNewAsync(nameof(ReprocessTransactions_Orchestrator));

        log.LogInformation($"{nameof(ReprocessTransactions_BlobTrigger)}: Started orchestration with ID {instanceId}");
    }

    [FunctionName(nameof(ReprocessTransactions_GetNextFolderToProcess))]
    public async Task<string> ReprocessTransactions_GetNextFolderToProcess(
        [ActivityTrigger] string input,
        [Blob($"{ReprocessContainerName}/{SettingsFileName}", FileAccess.ReadWrite, Connection = Connection)] BlockBlobClient settingsClient,
        ILogger log,
        CancellationToken cancellationToken)
    {
        var settingsResult = await settingsClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        
        var settings = JsonSerializer.Deserialize<ReprocessTransactionsSettings>(settingsResult.Value.Content) ?? throw new NullReferenceException($"{SettingsFileName} is null.");

        if (settings.NextDateToProcess.Date < settings.StartDate.Date || settings.NextDateToProcess.Date > settings.EndDate.Date)
        {
            log.LogWarning($"{nameof(ReprocessTransactions_GetNextFolderToProcess)}: Next date to process falls outside the start and end date range, exiting.");
            return null;
        }

        var folderToProcess = $"v1/{settings.NextDateToProcess:yyyy/MM/dd/}";

        log.LogInformation($"{nameof(ReprocessTransactions_GetNextFolderToProcess)}: Started processing for {folderToProcess}");

        return folderToProcess;
    }

    [FunctionName(nameof(ReprocessTransactions_QueueTransactions))]
    public async Task ReprocessTransactions_QueueTransactions(
        [ActivityTrigger] string folderToProcess,
        [Queue(queueName: ReprocessQueueName, Connection = Connection)] IAsyncCollector<ReprocessTransaction> reprocessQueue,
        CancellationToken cancellationToken)
    {
        var blobContainerClient = new BlobContainerClient(_options.Value.DsarStorageConnection, TransactionsContainerName);

        var blobHierarchyItems = blobContainerClient.GetBlobsByHierarchy(BlobTraits.None, BlobStates.None, "/", folderToProcess, cancellationToken);

        await Parallel.ForEachAsync(blobHierarchyItems, async (blobHierarchyItem, cancellationToken) =>
        {
            var blobUri = new Uri(blobContainerClient.Uri, $"{TransactionsContainerName}/{blobHierarchyItem.Blob.Name}");
            await reprocessQueue.AddAsync(new ReprocessTransaction { Uri = blobUri }, cancellationToken).ConfigureAwait(false);
        });
    }

    [FunctionName(nameof(ReprocessTransactions_SetNextFolderToProcess))]
    public async Task ReprocessTransactions_SetNextFolderToProcess(
        [ActivityTrigger] string processedFolder,
        [Blob($"{ReprocessContainerName}/{SettingsFileName}", FileAccess.ReadWrite, Connection = Connection)] BlockBlobClient settingsClient,
        ILogger log,
        CancellationToken cancellationToken)
    {
        log.LogInformation($"{nameof(ReprocessTransactions_SetNextFolderToProcess)}: Completed processing for {processedFolder}");

        var processedDate = DateTimeOffset.ParseExact(processedFolder, "v1/yyyy/MM/dd/", null);
        
        var settingsResult = await settingsClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
        
        var settings = JsonSerializer.Deserialize<ReprocessTransactionsSettings>(settingsResult.Value.Content) ?? throw new NullReferenceException($"{SettingsFileName} is null.");
        
        settings.NextDateToProcess = processedDate.AddDays(1);
        
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(settings)));
        
        await settingsClient.UploadAsync(memoryStream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}