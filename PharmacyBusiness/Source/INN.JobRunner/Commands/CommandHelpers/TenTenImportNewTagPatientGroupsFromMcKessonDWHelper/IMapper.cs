using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportNewTagPatientGroupsFromMcKessonDWHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="NewTagPatientGroupsRow"/> to a collection of <see cref="NewTagPatientGroups"/>
        /// </summary>
        IEnumerable<NewTagPatientGroups> MapToTenTenNewTagPatientGroups(IEnumerable<NewTagPatientGroupsRow> newTagPatientGroupsRows);
    }
}
