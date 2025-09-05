using Library.McKessonDWInterface.DataModel;
using Library.SnowflakeInterface.Data;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportStoreInventoryHistoryFromMcKessonDWHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<StoreInventoryHistory> MapMcKessonToTenTenStoreInventoryHistory(IEnumerable<StoreInventoryHistoryRow> storeInventoryHistoryRows)
        {
            return storeInventoryHistoryRows.Select(r => new StoreInventoryHistory 
            {
                DateOfService = r.DateOfService,
                StoreNbr = r.StoreNbr,
                NdcWithoutDashes = r.NdcWithoutDashes,
                DrugName = r.DrugName,
                Sdgi = r.Sdgi,
                Gcn = r.Gcn,
                GcnSequence = r.GcnSequence,
                OrangeBookCode = r.OrangeBookCode,
                FormCode = r.FormCode,
                PackSize = r.PackSize,
                TruePack = r.TruePack,
                PM = r.PM,
                IsPreferred = r.IsPreferred,
                LastAcquisitionCostPack = r.LastAcquisitionCostPack,
                LastAcquisitionCostUnit = r.LastAcquisitionCostUnit,
                LastAcquisitionCostDate = r.LastAcquisitionCostDate,
                OnHandQty = r.OnHandQty,
                OnHandValue = r.OnHandValue,
                ComittedQty = r.ComittedQty,
                ComittedValue = r.ComittedValue,
                TotalQTY = r.TotalQTY,
                TotalValue = r.TotalValue,
                AcquisitionCostPack = r.AcquisitionCostPack,
                AcquisitionCostUnit = r.AcquisitionCostUnit,
                PrimarySupplier = r.PrimarySupplier,
                LastChangeDate = r.LastChangeDate
            });
        }

        /// <inheritdoc />
        public IEnumerable<StoreInventoryHistory> MapSnowflakeToTenTenStoreInventoryHistory(IEnumerable<SelectStoreInventoryHistoryRow> snowflakeStoreInventoryRows)
        {
            return snowflakeStoreInventoryRows.Select(r => new StoreInventoryHistory
            {
                DateOfService = r.DateOfService,
                StoreNbr = int.TryParse(r.StoreNum, out int storeNbrParsed) ? storeNbrParsed : null,
                NdcWithoutDashes = r.NdcWithoutDashes,
                DrugName = r.DrugName,
                Sdgi = r.Sdgi,
                Gcn = r.Gcn,
                GcnSequence = decimal.TryParse(r.GcnSeqNum, out decimal gcnSeqNumParsed) ? gcnSeqNumParsed : null,
                OrangeBookCode = r.OrangeBookCode,
                FormCode = r.FormCode,
                PackSize = r.PackSize,
                TruePack = r.TruePack,
                PM = r.Pm,
                IsPreferred = r.IsPreferred,
                LastAcquisitionCostPack = r.LastAcqCostPack,
                LastAcquisitionCostUnit = r.LastAcqCostUnit,
                LastAcquisitionCostDate = r.LastAcqCostDate,
                OnHandQty = r.OnHandQty,
                OnHandValue = r.OnHandValue,
                ComittedQty = r.CommitedQty,
                ComittedValue = r.CommitedValue,
                TotalQTY = r.TotalQty,
                TotalValue = r.TotalValue,
                AcquisitionCostPack = r.AcqCostPack,
                AcquisitionCostUnit = r.AcqCostUnit,
                PrimarySupplier = r.PrimarySupplier,
                LastChangeDate = r.LastChangeDate,
            });
        }
    }
}
