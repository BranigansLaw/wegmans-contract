using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportSoldDetailFromMcKessonDWHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="SoldDetailRow"/> to a collection of <see cref="SoldDetail"/>
        /// </summary>
        IEnumerable<SoldDetail> MapToTenTenSoldDetail(IEnumerable<SoldDetailRow> soldDetailRows);
    }
}
