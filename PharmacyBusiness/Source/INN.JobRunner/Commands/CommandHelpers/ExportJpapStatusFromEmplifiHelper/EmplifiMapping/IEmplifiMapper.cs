using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiMapping;

public interface IEmplifiMapper
{
    JpapStatusRow MappingForEmplifi(Case emplifiCase, Address address, Issue issue, DateTime startDateTime, DateTime endDateTime);
}
