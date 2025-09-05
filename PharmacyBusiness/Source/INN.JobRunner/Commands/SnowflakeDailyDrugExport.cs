using Cocona;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper;
using INN.JobRunner.CommonParameters;
using Library.EmplifiInterface.DataModel;
using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace INN.JobRunner.Commands
{
    public class SnowflakeDailyDrugExport : PharmacyCommandBase
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly ILogger<SnowflakeDeceasedExport> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;
        private readonly IHelper _helper;

        public SnowflakeDailyDrugExport(
            ISnowflakeInterface snowflakeInterface,
            IDataFileWriter dataFileWriter,
            ILogger<SnowflakeDeceasedExport> logger,
            IOptions<SnowflakeDataOutputDirectories> snowflakeDataOutputDirectoriesOptions,
            IGenericHelper genericHelper,
            IHelper helper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snowflakeDataOutputDirectoriesOptions = snowflakeDataOutputDirectoriesOptions ?? throw new ArgumentNullException(nameof(snowflakeDataOutputDirectoriesOptions));
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
        }

        [Command(
            "snowflake-daily-drug-export",
            Description = "Runs snowflake query, DailyDrug.sql",
            Aliases = ["INN548"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation("Gathering DailyDrug data");
            IEnumerable<DailyDrugRow> dailyDrugRows = 
                await _snowflakeInterface.QuerySnowflakeAsync(new DailyDrugQuery(),
                CancellationToken);

            foreach (var row in dailyDrugRows)
            {
                row.Decile = _helper.DeriveDecile(row.GroupName,
                    row.PPrdProductKey,
                    row.PgPrdProductKey,
                    row.PgMemberStatus,
                    row.GsGrpGroupNumber);

                row.OtcType = _helper.DeriveOtcType(row.GroupName,
                    row.PPrdProductKey,
                    row.PgPrdProductKey,
                    row.PgMemberStatus,
                    row.GsGrpGroupNumber);

                row.CostBasisEffDate = _helper.DeriveCostBasisEffDate(row.RawPcfrCost,
                    row.RawPcfnCost,
                    row.RawPcfnCost,
                    (row.UserDeffEffectiveEndDate.HasValue) ? row.UserDeffEffectiveEndDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty,
                    (row.NteEffectiveEndDate.HasValue) ? row.NteEffectiveEndDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty,
                    (row.NteEffectiveEndDate.HasValue) ? row.NteEffectiveEndDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty,
                    (row.RepackConEffectiveEndDate.HasValue) ? row.RepackConEffectiveEndDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty);
            }

            _logger.LogInformation($"Returned {dailyDrugRows.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN548.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(dailyDrugRows, new DataFileWriterConfig<DailyDrugRow>
            {
                Header = "Ndc|DrugName|Manufacturer|MfrNum|Strength|StrengthUnits|UnitDoseFlag|UnitOfUseFlag|PackageQty|PackSize|InnerPack|OuterPack|CaseSize|Unit|DispensingUnits|DrugShipper|PackDesc|OrderNum|PrevNdc|ReplacementNdc" +
                "|ProdSourceInd|FdbAdded|DateAdded|ObsoleteDate|DeactivateDate|LastProviderUpdate|MaintenanceDrugFlag|GenericName|Gcn|GcnSeqNum|DrugUpc|Sdgi|SdgiOverride|DrugSchedule|DeaClass|PriceMaintained|Decile|AhfsTherapClass" +
                "|AhfsTherClassDesShort|AhfsTherClassDesLong|SigVerb|SigVerbOverride|SigRoute|SigRouteOverride|SigUnit|SigUnitOverride|DosageForm|DesiIndicator|OrangeBookCode|OrangeBookCodeOverride|DefaultDaw|WarehouseFlag|OriginatorInnovator" +
                "|EnhancedRefillOptional|GenSubPackRestriction|MinDispQty|Bbawp|Distributor|BbawpOverride|CompoundFlag|AlternateLabel|BlockedProductFlag|Cost|CostManuallyMaintained|PercentModifier|OtcType|UnitCost|",
                WriteDataLine = (DailyDrugRow c) => $"{c.Ndc }|{ c.DrugName }|{ c.Manufacturer }|{ c.MfrNum }|{ c.Strength }|{ c.StrengthUnits }|{ c.UnitDoseFlag }|{ c.UnitOfUseFlag }|{ c.PackageQty }|{ c.PackSize }|{ c.InnerPack }|{ c.OuterPack }|" +
                $"{ c.CaseSize }|{ c.Unit }|{ c.DispensingUnits }|{ c.DrugShipper }|{ c.PackDesc }|{ c.OrderNum }|{ c.PrevNdc }|{ c.ReplacementNdc }{ c.ProdSourceInd }|{ c.FdbAdded }|{ c.DateAdded }|{ c.ObsoleteDate }|{ c.DeactivateDate }|{ c.LastProviderUpdate }" +
                $"|{ c.MaintenanceDrugFlag }|{ c.GenericName }|{ c.Gcn }|{ c.GcnSeqNum }|{ c.DrugUpc }|{ c.Sdgi }|{ c.SdgiOverride }|{ c.DrugSchedule }|{ c.DeaClass }|{ c.PriceMaintained }|{ c.Decile }|{ c.AhfsTherapClass }|{ c.AhfsTherClassDesShort }" +
                $"|{ c.AhfsTherClassDesLong }|{ c.SigVerb }|{ c.SigVerbOverride }|{ c.SigRoute }|{ c.SigRouteOverride }|{ c.SigUnit }|{ c.SigUnitOverride }|{ c.DosageForm }|{ c.DesiIndicator }|{ c.OrangeBookCode }|{ c.OrangeBookCodeOverride }|{ c.DefaultDaw }" +
                $"|{ c.WarehouseFlag }|{ c.OriginatorInnovator }|{ c.EnhancedRefillOptional }|{ c.GenSubPackRestriction }|{ c.MinDispQty }|{ c.Bbawp }|{ c.Distributor }|{ c.BbawpOverride }|{ c.CompoundFlag }|{ c.AlternateLabel }|{ c.BlockedProductFlag }|{ c.Cost }|{ c.CostManuallyMaintained }|{ c.PercentModifier }" +
                $"|{ c.OtcType }|{ c.UnitCost }|",
                OutputFilePath = writePath,
            });
        }
    }
}
