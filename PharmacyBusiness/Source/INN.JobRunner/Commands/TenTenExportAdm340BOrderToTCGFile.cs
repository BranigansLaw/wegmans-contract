using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.TenTenInterface;
using Library.TenTenInterface.DownloadsFromTenTen;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class TenTenExportAdm340BOrderToTCGFile : PharmacyCommandBase
    {
        private readonly ITenTenInterface _tenTenInterface;
        private readonly ILogger<TenTenExportAdm340BOrderToTCGFile> _logger;

        public TenTenExportAdm340BOrderToTCGFile(
            ITenTenInterface tenTenInterface,
            ILogger<TenTenExportAdm340BOrderToTCGFile> logger,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Command(
            "export-adm-340b-order-to-tcg-file", 
            Description = "Export Adm 340B Order from TenTen and send to TCG. Control-M job INN702", 
            Aliases = ["INN702"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Starting to export ADM Order from TenTen to a data file.");

            string tenTenDownloadQueryXml = @"<import path=""wegmans.shared.libraries.admop"" library=""""/><insert block=""pilot_generate_order_points""/>";
            string fileNamePattern = $"340B_WegmansShelter_OrderOpportunity_{runFor.RunFor:yyyyMMdd}.txt";
            string fieldDelimiter = "|";
            string[] columnNames = ["contract_id", "account_id", "contract_name", "is_pool", "wholesaler_num", "wholesaler_name", "ndc_wo", "drug_name", "drug_strength", "drug_pack_size", "ord_pkg", "packs_to_order", "order_on"];

            DataExtractFileSpecifications outputFileSpecifications =
                new(fileNamePattern, fieldDelimiter, hasHeaderRow: true, replacementHeader: string.Join(fieldDelimiter, columnNames));

            TenTenDataExtracts dataExtract = new(tenTenDownloadQueryXml, outputFileSpecifications, columnNames, runFor.RunFor);

            await _tenTenInterface.OutputDataExtractQueryResultsTenTenAsync(dataExtract, CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished exporting ADM Order Rows.");
        }
    }
}
