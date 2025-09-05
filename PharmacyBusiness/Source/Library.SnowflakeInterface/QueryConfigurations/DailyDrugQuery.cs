using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{
    public class DailyDrugQuery : AbstractQueryConfiguration<DailyDrugRow>
    {
        /// <inheritdoc />
        public override string QueryFilePath => "./SnowflakeSQL/DailyDrug.sql";

        /// <inheritdoc />
        public override DailyDrugRow MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {
            logDebug("Creating DailyDrugRow");
            return new DailyDrugRow {
                Ndc = reader.GetStringByIndex(0),
                DrugName = reader.GetStringByIndex(1),
                Manufacturer = reader.GetStringByIndex(2),
                MfrNum = reader.GetStringByIndex(3),
                Strength = reader.GetStringByIndex(4),
                StrengthUnits = reader.GetStringByIndex(5),
                UnitDoseFlag = reader.GetStringByIndex(6),
                UnitOfUseFlag = reader.GetStringByIndex(7),
                PackageQty = reader.GetNullableValueByIndex<decimal>(8),
                PackSize = reader.GetNullableValueByIndex<decimal>(9),
                InnerPack = reader.GetStringByIndex(10),
                OuterPack = reader.GetStringByIndex(11),
                CaseSize = reader.GetNullableValueByIndex<long>(12),
                Unit = reader.GetStringByIndex(13),
                DispensingUnits = reader.GetNullableValueByIndex<long>(14),
                DrugShipper = reader.GetNullableValueByIndex<long>(15),
                PackDesc = reader.GetStringByIndex(16),
                OrderNum = reader.GetNullableValueByIndex<long>(17),
                PrevNdc = reader.GetStringByIndex(18),
                ReplacementNdc = reader.GetStringByIndex(19),
                ProdSourceInd = reader.GetStringByIndex(20),
                FdbAdded = reader.GetStringByIndex(21),
                DateAdded = reader.GetStringByIndex(22),
                ObsoleteDate = reader.GetStringByIndex(23),
                DeactivateDate = reader.GetStringByIndex(24),
                LastProviderUpdate = reader.GetStringByIndex(25),
                MaintenanceDrugFlag = reader.GetStringByIndex(26),
                GenericName = reader.GetStringByIndex(27),
                Gcn = reader.GetStringByIndex(28),
                GcnSeqNum = reader.GetStringByIndex(29),
                DrugUpc = reader.GetNullableValueByIndex<long>(30),
                Sdgi = reader.GetStringByIndex(31),
                SdgiOverride = reader.GetStringByIndex(32),
                DrugSchedule = reader.GetStringByIndex(33),
                DeaClass = reader.GetStringByIndex(34),
                PriceMaintained = reader.GetStringByIndex(35),
                GroupName = reader.GetStringByIndex(36),
                PgPrdProductKey = reader.GetNullableValueByIndex<long>(37),
                PgMemberStatus = reader.GetStringByIndex(38),
                GsGrpGroupNumber = reader.GetNullableValueByIndex<long>(39),
                Decile = reader.GetStringByIndex(40),
                AhfsTherapClass = reader.GetStringByIndex(41),
                AhfsTherClassDesShort = reader.GetStringByIndex(42),
                AhfsTherClassDesLong = reader.GetStringByIndex(43),
                SigVerb = reader.GetStringByIndex(44),
                SigVerbOverride = reader.GetStringByIndex(45),
                SigRoute = reader.GetStringByIndex(46),
                SigRouteOverride = reader.GetStringByIndex(47),
                SigUnit = reader.GetStringByIndex(48),
                SigUnitOverride = reader.GetStringByIndex(49),
                DosageForm = reader.GetStringByIndex(50),
                DesiIndicator = reader.GetNullableValueByIndex<long>(51),
                OrangeBookCode = reader.GetStringByIndex(52),
                OrangeBookCodeOverride = reader.GetStringByIndex(53),
                DefaultDaw = reader.GetStringByIndex(54),
                WarehouseFlag = reader.GetStringByIndex(55),
                OriginatorInnovator = reader.GetStringByIndex(56),
                EnhancedRefillOptional = reader.GetStringByIndex(57),
                GenSubPackRestriction = reader.GetStringByIndex(58),
                MinDispQty = reader.GetNullableValueByIndex<decimal>(59),
                Bbawp = reader.GetNullableValueByIndex<decimal>(60),
                Distributor = reader.GetStringByIndex(61),
                BbawpOverride = reader.GetNullableValueByIndex<decimal>(62),
                CompoundFlag = reader.GetStringByIndex(63),
                AlternateLabel = reader.GetStringByIndex(64),
                BlockedProductFlag = reader.GetStringByIndex(65),
                RawPcfrCost = reader.GetNullableValueByIndex<decimal>(66),
                RawPcfcCost = reader.GetNullableValueByIndex<decimal>(67),
                RawPcfnCost = reader.GetNullableValueByIndex<decimal>(68),
                UserDeffEffectiveEndDate = reader.GetNullableValueByIndex<DateTime>(69),
                NteEffectiveEndDate = reader.GetNullableValueByIndex<DateTime>(70),
                ConEffectiveEndDate = reader.GetNullableValueByIndex<DateTime>(71),
                RepackConEffectiveEndDate = reader.GetNullableValueByIndex<DateTime>(72),
                PPrdProductKey = reader.GetNullableValueByIndex<long>(73),
                Cost = reader.GetNullableValueByIndex<decimal>(74),
                CostManuallyMaintained = reader.GetStringByIndex(75),
                PercentModifier = reader.GetNullableValueByIndex<decimal>(76),
                CostBasisEffDate = reader.GetStringByIndex(77),
                OtcType = reader.GetStringByIndex(78),
                UnitCost = reader.GetNullableValueByIndex<decimal>(79)
            };
        }
    }
}
