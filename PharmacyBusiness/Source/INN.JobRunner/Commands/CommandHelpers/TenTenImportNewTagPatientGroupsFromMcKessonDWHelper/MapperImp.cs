using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportNewTagPatientGroupsFromMcKessonDWHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<NewTagPatientGroups> MapToTenTenNewTagPatientGroups(IEnumerable<NewTagPatientGroupsRow> newTagPatientGroupsRows)
        {
            return newTagPatientGroupsRows.Select(r => new NewTagPatientGroups 
            { 
                PatientAddDate = r.PatientAddDate,
                StoreNum = r.StoreNum,
                PatientNum = r.PatientNum,
                GroupNum = r.GroupNum,
                GroupName = r.GroupName,
                GroupDescription = r.GroupDescription,
                EmployeeUserName = r.EmployeeUserName,
                EmployeeFirstName = r.EmployeeFirstName,
                EmployeeLastName = r.EmployeeLastName,
                EventDescription = r.EventDescription,
                GroupStartDate = r.GroupStartDate
            });
        }
    }
}
