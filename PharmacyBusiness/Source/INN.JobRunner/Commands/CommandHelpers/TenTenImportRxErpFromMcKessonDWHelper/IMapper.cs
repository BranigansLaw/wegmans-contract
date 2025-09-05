using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportRxErpFromMcKessonDWHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="RxErpRow"/> to a collection of <see cref="RxErp"/>
        /// </summary>
        IEnumerable<RxErp> MapToTenTenRxErp(IEnumerable<RxErpRow> rxErpRows);
    }
}
