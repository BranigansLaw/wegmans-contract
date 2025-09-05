using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFullFromMcKessonCPSHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<PCSPrefFull> MapToTenTenPCSPrefFull(IEnumerable<PCSPrefFullRow> pcsPrefFullRows)
        {
            return pcsPrefFullRows.Select(r => new PCSPrefFull
            {
                //TODO: UNDER CONSTRUCTION
                StoreNum = r.StoreNum
            });
        }
    }
}
