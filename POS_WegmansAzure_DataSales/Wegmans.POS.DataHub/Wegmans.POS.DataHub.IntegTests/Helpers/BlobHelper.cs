using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Reflection;

namespace Wegmans.POS.DataHub.IntegTests.Helpers
{
    public static class BlobUtility
    {
        private static DefaultAzureCredential credential = new DefaultAzureCredential();
        private static string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string blobUrl = ApiHelper.GetBlobStorageAddress();
        private static string connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        //public static async Task<int> GetBlobCount(string containerName, string pathToFolder)
        //{
        //    int count = 0;

        //    BlobContainerClient containerClient = new BlobContainerClient(new Uri(blobUrl + containerName), credential);
        //    var resultSegment = containerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", pathToFolder).AsPages(default, 100);

        //    await foreach (Page<BlobHierarchyItem> page in resultSegment)
        //    {
        //        count = page.Values.Count();
        //    }

        //    return await Task.FromResult(count);
        //}

        public static async Task<bool> WaitUntilBlobsAreProccessed(string containerName, string pathToFile)
        {
            BlobClient blobClient = new BlobClient(new Uri(blobUrl + $"{containerName}/{pathToFile}"), credential);

            bool blobExists = blobClient.Exists();

            int retryAttempts = 0;
            while (!blobExists && retryAttempts < 20)
            {
                retryAttempts++;
                Thread.Sleep(3000);
                blobExists = await blobClient.ExistsAsync();
            }

            return blobExists;
        }

        public static async Task AddBlobToRawTlog(string fileName)
        {
            BlobClient blobClient = new BlobClient(new Uri(blobUrl + $"raw-tlog/2023/01/01/00/{fileName}"), credential);

            string fileToUpload = currentPath + $"/Samples/RawTlog/{fileName}";

            using FileStream stream = File.OpenRead(fileToUpload);
            await blobClient.UploadAsync(fileToUpload, true);
            stream.Close();
        }

        public static void DeleteFile(string containerName, string pathToFile)
        {
            BlobClient blobClient = new BlobClient(new Uri(blobUrl + $"{containerName}/{pathToFile}"), credential);

            blobClient.DeleteIfExists();
        }

        public static async Task<string> ReadBlob(string containerName, string pathToFile)
        {
            BlobClient blobClient = new BlobClient(new Uri(blobUrl + $"{containerName}/{pathToFile}"), credential);

            BlobDownloadResult downloadResult = await blobClient.DownloadContentAsync();
            string blobContents = downloadResult.Content.ToString();

            return blobContents;
        }
    }
}