using Library.McKessonCPSInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportEnrollmentFactFromMcKessonCPSHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="EnrollmentFactRow"/> to a collection of <see cref="EnrollmentFact"/>
        /// </summary>
        IEnumerable<EnrollmentFact> MapToTenTenEnrollmentFact(IEnumerable<EnrollmentFactRow> enrollmentFactRows);
    }
}
