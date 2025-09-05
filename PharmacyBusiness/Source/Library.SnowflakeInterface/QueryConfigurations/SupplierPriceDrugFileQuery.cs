using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class SupplierPriceDrugFileQuery : AbstractQueryConfiguration<SupplierPriceDrugFileRow>
    {
        public required DateOnly RunDate { get; set; }

        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/SupplierPriceDrugFile_YYYYMMDD.sql";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {
            logDebug("Adding parameters");

            DbParameter paramRunDate = command.CreateParameter();
            paramRunDate.ParameterName = "RunDate";
            paramRunDate.DbType = DbType.Date;
            paramRunDate.Value = new DateTime(RunDate.Year, RunDate.Month, RunDate.Day);
            command.Parameters.Add(paramRunDate);
        }

        /// <inheritdoc />
        public override SupplierPriceDrugFileRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating SupplierPriceDrugFileRow");
            return new SupplierPriceDrugFileRow {
                DateOfService = reader.GetValueByIndex<DateTime>(0),
                SupplierName = reader.GetStringByIndex(1),
                VendorItemNumber = reader.GetStringByIndex(2),
                Ndc = reader.GetStringByIndex(3),
                NdcWo = reader.GetStringByIndex(4),
                DrugName = reader.GetStringByIndex(5),
                DrugForm = reader.GetStringByIndex(6),
                DeaClass = reader.GetStringByIndex(7),
                Strength = reader.GetStringByIndex(8),
                StrengthUnit = reader.GetStringByIndex(9),
                GenericName = reader.GetStringByIndex(10),
                PackSize = reader.GetNullableValueByIndex<decimal>(11),
                IsMaintDrug = reader.GetStringByIndex(12),
                Sdgi = reader.GetStringByIndex(13),
                Gcn = reader.GetStringByIndex(14),
                GcnSeqNumber = reader.GetStringByIndex(15),
                DrugManufacturer = reader.GetStringByIndex(16),
                OrangeBookCode = reader.GetStringByIndex(17),
                PackPrice = reader.GetNullableValueByIndex<decimal>(18),
                PricePerUnit = reader.GetNullableValueByIndex<decimal>(19),
                UnitPriceDate = reader.GetNullableValueByIndex<DateTime>(20),
                PurchIncr = reader.GetNullableValueByIndex<long>(21),
                PkgSizeIncr = reader.GetNullableValueByIndex<decimal>(22),
                Status = reader.GetStringByIndex(23),
                EffStartDate = reader.GetNullableValueByIndex<DateTime>(24),
            };
        }
    }
}
