namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper
{
    public interface ISnowflakeToTenTenDataSftpExportHelper
    {
        /// <summary>
        /// Runs snowflake query, DurConflict_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportDurConflictAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, PRESCRIBER_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportPrescribersAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, PRESCRIBERADDRESS_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportPrescriberAddressAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, RxTransfer_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportRxTransferAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, SupplierPriceDrugFile_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportSupplierPriceDrugFileExportAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, PetPtNums_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportPetPtNumsAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, InvAdj_YYYYMMDD.sql and exports it to file
        /// </summary>
        Task ExportInvAdjAsync(DateOnly runFor, CancellationToken c);
    }
}
