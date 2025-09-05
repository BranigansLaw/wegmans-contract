using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportMeasureFactFromMcKessonCPSHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<MeasureFact> MapToTenTenMeasureFact(IEnumerable<MeasureFactRow> measureFactRows)
        {
            return measureFactRows.Select(r => new MeasureFact
            {
                //TODO: UNDER CONSTRUCTION
                StoreNum = r.StoreNum
            });
        }
    }
}
