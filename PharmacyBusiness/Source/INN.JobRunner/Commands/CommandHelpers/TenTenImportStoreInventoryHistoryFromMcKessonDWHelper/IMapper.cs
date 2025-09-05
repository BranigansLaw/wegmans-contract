using Library.McKessonDWInterface.DataModel;
using Library.SnowflakeInterface.Data;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportStoreInventoryHistoryFromMcKessonDWHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="SelectStoreInventoryHistoryRow"/> to a collection of <see cref="StoreInventoryHistory"/>
        /// </summary>
        IEnumerable<StoreInventoryHistory> MapSnowflakeToTenTenStoreInventoryHistory(IEnumerable<SelectStoreInventoryHistoryRow> snowflakeStoreInventoryRows);

        /// <summary>
        /// Maps the collection of <see cref="StoreInventoryHistoryRow"/> to a collection of <see cref="StoreInventoryHistory"/>
        /// </summary>
        IEnumerable<StoreInventoryHistory> MapMcKessonToTenTenStoreInventoryHistory(IEnumerable<StoreInventoryHistoryRow> storeInventoryHistoryRows);
    }
}
