using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper;
using INN.JobRunner.CommonParameters;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel;
using Library.TenTenInterface.DownloadsFromTenTen;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands;

public class TenTenImportProjectionDetail : PharmacyCommandBase
{
    private readonly ITenTenInterface _tenTenInterface;
    private readonly IMapper _mapper;
    private readonly ILogger<TenTenImportProjectionDetail> _logger;

    public TenTenImportProjectionDetail(
        ITenTenInterface tenTenInterface,
        IMapper mapper,
        ILogger<TenTenImportProjectionDetail> logger,
        IGenericHelper genericHelper,
        ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
    {
        _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Command(
        "tenten-import-projection-detail",
        Description = "Query Projection Detail from TenTen, load to TenTen dated build table, and do rollup. Control-M job INN610",
        Aliases = ["INN610"]
    )]
    public async Task RunAsync(RunForParameter runFor)
    {
        _genericHelper.CheckRunForDate(runFor, new DateOnly(2023, 12, 30));

        await _genericHelper.RunFromToRunTillAsync(runFor, RunCommandLogicForDateAsync);
    }

    public async Task RunCommandLogicForDateAsync(DateOnly forDate)
    {
        string tenTenDownloadQueryXml =
            $"<import path=\"wegmans.accounting.library.projdtl\"/><insert block=\"projdtl\" run_date=\"{forDate:yyyyMMdd}\"/>";

        string[] tenTenDownloadQueryColumnNames = [
            "store_num",
            "rx_num",
            "refill_num",
            "part_seq_num",
            "tx_num",
            "store_gen_sales",
            "store_gen_cost",
            "store_gen_count",
            "store_brand_sales",
            "store_brand_cost",
            "store_brand_count",
            "cf_gen_sales",
            "cf_gen_cost",
            "cf_gen_count",
            "cf_brand_sales",
            "cf_brand_cost",
            "cf_brand_count",
            "discount",
            "bill_ind",
            "refund_price",
            "refund_youpay",
            "datafilesrc",
            "date",
            "rxid"
        ];

        _logger.LogInformation("Begin importing Projection Detail for {ForDate}.", forDate);
        var queryProjectionDetail = new TenTenDataQuery(
            tenTenDownloadQueryXml,
            tenTenDownloadQueryColumnNames,
            forDate);

        var projectionDetailRows = await _tenTenInterface.GetQueryResultsForTransformingToCollectionsAsync<ProjectionDetailRow>(
                queryProjectionDetail,
                CancellationToken).ConfigureAwait(false);
        _logger.LogDebug("Projection detail has {RowCount} rows for {ForDate}.", projectionDetailRows.Count(), forDate);

        if (projectionDetailRows.Any())
        {
            _logger.LogInformation("Upload projection detail to 1010 build table and do rollup for {ForDate}.", forDate);
            await _tenTenInterface.CreateOrAppendTenTenDataAsync(
                forDate,
                _mapper.MapToTenTenProjectionDetail(projectionDetailRows),
                CancellationToken).ConfigureAwait(false);
        }
        else
        {
            _logger.LogWarning("No projection detail rows to load for {ForDate}.", forDate);
        }

        _logger.LogInformation("Finished importing Projection Detail for {ForDate}.", forDate);
    }
}
