using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportEnrollmentFactFromMcKessonCPSHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<EnrollmentFact> MapToTenTenEnrollmentFact(IEnumerable<EnrollmentFactRow> enrollmentFactRows)
        {
            return enrollmentFactRows.Select(r => new EnrollmentFact
            {
                //TODO: UNDER CONSTRUCTION
                StoreNum = r.StoreNum
            });
        }
    }
}
