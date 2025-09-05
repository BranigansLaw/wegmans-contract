using Library.TenTenInterface.DataModel.UploadRow;
using Library.TenTenInterface.DownloadsFromTenTen;
using Library.TenTenInterface.UploadsToTenTen;
using Library.TenTenInterface.XmlTemplateHandlers;

namespace Library.TenTenInterface
{
    public interface ITenTenInterface
    {
        /// <summary>
        /// Creates or appends data to TenTen from the given <paramref name="rows"/> that implement <see cref="ITenTenUploadConvertible"/>
        /// </summary>
        Task CreateOrAppendTenTenDataAsync<T>(DateOnly runFor, IEnumerable<T> rows, CancellationToken c) where T : ITenTenUploadConvertible;

        /// <summary>
        /// Confirm rows of data uploaded to TenTen.
        /// </summary>
        /// <param name="etl"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<int> RowCountQueryTenTenAsync<T>(
            TenTenEtl<T> etl,
            CancellationToken c) where T : class, new();

        /// <summary>
        /// Data extract query to TenTen.
        /// </summary>
        /// <param name="dataExtract"></param>
        /// <param name="outboxPath"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task OutputDataExtractQueryResultsTenTenAsync(
            TenTenDataExtracts dataExtract,
            CancellationToken c
        );

        /// <summary>
        /// Data extract query to TenTen returned as a collection of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetQueryResultsForTransformingToCollectionsAsync<T>(TenTenDataQuery query, CancellationToken c);

        /// <summary>
        /// Data extract query to TenTen returned as a CSV string.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<string> GetQueryResultsAsCsvAsync(TenTenDataQuery query, CancellationToken c);

        /// <summary>
        /// Upload <paramref name="toUpload"/> rows to TenTen
        /// </summary>
        Task UploadDataAsync<T>(IEnumerable<T> toUpload, CancellationToken c, DateOnly? runDate = null) where T : IAzureBlobUploadRow;
    }
}
