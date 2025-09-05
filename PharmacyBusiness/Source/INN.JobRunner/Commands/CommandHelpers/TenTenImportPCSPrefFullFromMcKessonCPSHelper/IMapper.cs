using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFullFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="PCSPrefFullRow"/> to a collection of <see cref="PCSPrefFull"/>
        /// </summary>
        IEnumerable<PCSPrefFull> MapToTenTenPCSPrefFull(IEnumerable<PCSPrefFullRow> pcsPrefFullRows);
    }
}
