using CaseServiceWrapper;

namespace Library.EmplifiInterface.Helper;

public interface IJpapDispenseAndStatusHelper
{
    IEnumerable<ICollection<Filter>> GetOutboundStatusRequests(DateTime startDate, DateTime endDate);

    (DateTime startDateTime, DateTime endDateTime) GetDateRangeForOutboundStatus(DateTime runFor);
}
