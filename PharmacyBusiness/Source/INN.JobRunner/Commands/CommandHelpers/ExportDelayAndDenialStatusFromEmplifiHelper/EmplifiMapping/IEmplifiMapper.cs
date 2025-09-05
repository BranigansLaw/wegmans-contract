using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifiMapping
{
    public interface IEmplifiMapper
    {
        DelayAndDenialStatusRow MappingForEmplifi(Case emplifiData, Address address, Issue issue, DateTime startDateTime, DateTime endDateTime);
    }
}
