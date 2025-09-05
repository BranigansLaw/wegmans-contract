using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="PCSPrefRow"/> to a collection of <see cref="PCSPref"/>
        /// </summary>
        IEnumerable<PCSPref> MapToTenTenPCSPref(IEnumerable<PCSPrefRow> pcsPrefRows);
    }
}
