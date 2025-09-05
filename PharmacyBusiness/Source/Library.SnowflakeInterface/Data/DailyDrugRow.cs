namespace Library.SnowflakeInterface.Data
{
    public class DailyDrugRow
    {
        public required string? Ndc { get; set; }

        public required string? DrugName { get; set; }

        public required string? Manufacturer { get; set; }

        public required string? MfrNum { get; set; }

        public required string? Strength { get; set; }

        public required string? StrengthUnits { get; set; }

        public required string? UnitDoseFlag { get; set; }

        public required string? UnitOfUseFlag { get; set; }

        public required decimal? PackageQty { get; set; }

        public required decimal? PackSize { get; set; }

        public required string? InnerPack { get; set; }

        public required string? OuterPack { get; set; }

        public required long? CaseSize { get; set; }

        public required string? Unit { get; set; }

        public required long? DispensingUnits { get; set; }

        public required long? DrugShipper { get; set; }

        public required string? PackDesc { get; set; }

        public required long? OrderNum { get; set; }

        public required string? PrevNdc { get; set; }

        public required string? ReplacementNdc { get; set; }

        public required string? ProdSourceInd { get; set; }

        public required string? FdbAdded { get; set; }

        public required string? DateAdded { get; set; }

        public required string? ObsoleteDate { get; set; }

        public required string? DeactivateDate { get; set; }

        public required string? LastProviderUpdate { get; set; }

        public required string? MaintenanceDrugFlag { get; set; }

        public required string? GenericName { get; set; }

        public required string? Gcn { get; set; }

        public required string? GcnSeqNum { get; set; }

        public required long? DrugUpc { get; set; }

        public required string? Sdgi { get; set; }

        public required string? SdgiOverride { get; set; }

        public required string? DrugSchedule { get; set; }

        public required string? DeaClass { get; set; }

        public required string? PriceMaintained { get; set; }

        public required string? GroupName { get; set; }

        public required long? PgPrdProductKey { get; set; }

        public required string? PgMemberStatus { get; set; }

        public required long? GsGrpGroupNumber { get; set; } 

        public required string? Decile { get; set; }

        public required string? AhfsTherapClass { get; set; }

        public required string? AhfsTherClassDesShort { get; set; }

        public required string? AhfsTherClassDesLong { get; set; }

        public required string? SigVerb { get; set; }

        public required string? SigVerbOverride { get; set; }

        public required string? SigRoute { get; set; }

        public required string? SigRouteOverride { get; set; }

        public required string? SigUnit { get; set; }

        public required string? SigUnitOverride { get; set; }

        public required string? DosageForm { get; set; }

        public required long? DesiIndicator { get; set; }

        public required string? OrangeBookCode { get; set; }

        public required string? OrangeBookCodeOverride { get; set; }

        public required string? DefaultDaw { get; set; }

        public required string? WarehouseFlag { get; set; }

        public required string? OriginatorInnovator { get; set; }

        public required string? EnhancedRefillOptional { get; set; }

        public required string? GenSubPackRestriction { get; set; }

        public required decimal? MinDispQty { get; set; }

        public required decimal? Bbawp { get; set; }

        public required string? Distributor { get; set; }

        public required decimal? BbawpOverride { get; set; }

        public required string? CompoundFlag { get; set; }

        public required string? AlternateLabel { get; set; }

        public required string? BlockedProductFlag { get; set; }

        public required decimal? RawPcfrCost { get; set; }

        public required decimal? RawPcfcCost { get; set; }

        public required decimal? RawPcfnCost { get; set; }

        public required DateTime? UserDeffEffectiveEndDate { get; set; }

        public required DateTime? NteEffectiveEndDate { get; set; }

        public required DateTime? ConEffectiveEndDate { get; set; }

        public required DateTime? RepackConEffectiveEndDate { get; set; }

        public required long? PPrdProductKey { get; set; }

        public required decimal? Cost { get; set; }

        public required string? CostManuallyMaintained { get; set; }

        public required decimal? PercentModifier { get; set; }

        public required string? CostBasisEffDate { get; set; }

        public required string? OtcType { get; set; }

        public required decimal? UnitCost { get; set; }
    }
}
