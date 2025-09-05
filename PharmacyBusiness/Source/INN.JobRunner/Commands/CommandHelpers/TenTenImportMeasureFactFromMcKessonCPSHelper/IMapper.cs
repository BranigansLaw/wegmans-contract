using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportMeasureFactFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="MeasureFactRow"/> to a collection of <see cref="MeasureFact"/>
        /// </summary>
        IEnumerable<MeasureFact> MapToTenTenMeasureFact(IEnumerable<MeasureFactRow> measureFactRows);
    }
}
