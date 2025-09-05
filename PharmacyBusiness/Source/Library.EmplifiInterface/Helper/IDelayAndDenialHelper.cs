using CaseServiceWrapper;

namespace Library.EmplifiInterface.Helper
{
    public interface IDelayAndDenialHelper
    {
        /// <summary>
        /// Get the outbound status requests for the given date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<ICollection<Filter>> GetOutboundStatusRequests(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get the date range for the outbound status
        /// </summary>
        /// <param name="runFor"></param>
        /// <returns></returns>
        (DateTime startDateTime, DateTime endDateTime) GetDateRangeForOutboundStatus(DateTime runFor);
    }
}
