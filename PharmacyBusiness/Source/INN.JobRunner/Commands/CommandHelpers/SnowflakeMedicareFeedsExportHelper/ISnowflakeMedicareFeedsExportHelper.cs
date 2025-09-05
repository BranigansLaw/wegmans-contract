namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper
{
    public interface ISnowflakeMedicareFeedsExportHelper
    {
        /// <summary>
        /// Runs snowflake query, FDS_Pharmacies.sql and exports a file
        /// </summary>
        Task ExportFdsPharmaciesAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, FDS_Perscriptions.sql and exports a file
        /// </summary>
        Task ExportFdsPrescriptionsAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Runs snowflake query, Wegmans_HPOne_Pharmacies_YYYYMMDD.sql and exports a file
        /// </summary>
        Task ExportHpOnePharmaciesAsync(CancellationToken c);

        /// <summary>
        /// Runs snowflake query, Wegmans_HPOne_Prescriptions_YYYYMMDD.sql and exports a file
        /// </summary>
        Task ExportHpOnePrescriptionsExport(DateOnly runFor, CancellationToken c);
    }
}
