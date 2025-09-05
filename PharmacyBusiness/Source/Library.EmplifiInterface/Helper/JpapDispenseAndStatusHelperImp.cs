using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using System.Net.Sockets;
using Library.EmplifiInterface.Exceptions;

namespace Library.EmplifiInterface.Helper;

public class JpapDispenseAndStatusHelperImp : IJpapDispenseAndStatusHelper
{
    /// <inheritdoc />
    public IEnumerable<ICollection<Filter>> GetOutboundStatusRequests(DateTime startDate, DateTime endDate)
    {
        ICollection<ICollection<Filter>> toReturn = [];

        //For Outbound Status feed, get the cases from the CRM application for C26.
        List<string> patientStatusList = ["C26"];

        foreach (var patientStatus in patientStatusList)
        {
            toReturn.Add([
                new Filter() //Note that using this C26 Filter might exlude issues associated with the Case.
                {
                    filter_seq = 1,
                    selection_operator = SelectionOperator.BETWEEN,
                    selection_category_id = patientStatus, // C26
                    selection_code = startDate.ToString(),
                    selection_code2 = endDate.ToString()
                },
                new Filter()
                {
                    filter_seq = 2,
                    selection_operator = SelectionOperator.ISNOTNULL,
                    selection_category_id = "C16" // Patient Status Code
                },
                new Filter()
                {
                    filter_seq = 3,
                    selection_operator = SelectionOperator.NOTIN,
                    selection_category_id = "C16", // Patient Status Code
                    selection_code = "C7=Ineligible For Program"
                },
                new Filter()
                {
                    filter_seq = 4,
                    selection_operator = SelectionOperator.NOTIN,
                    selection_category_id = "C47", // Program Header
                    selection_code = "RPh Only;Infliximab;Remicade;Simponi;Simponi Aria;Tremfya;Stelara"
                }
            ]);
        }

        return toReturn;
    }

    /// <inheritdoc />
    public (DateTime startDateTime, DateTime endDateTime) GetDateRangeForOutboundStatus(DateTime runFor)
    {
        DateTime startDateTime = runFor.Date;
        DateTime endDateTime = runFor.Date;
        DayOfWeek dayOfWeek = runFor.DayOfWeek;
        TimeSpan morningRunTime = new TimeSpan(8, 30, 0);
        TimeSpan afternoonRunTime = new TimeSpan(14, 30, 0);

        if (runFor.TimeOfDay < morningRunTime)
        {
            startDateTime = startDateTime.AddDays(-1);
            endDateTime = endDateTime.AddDays(-1);
            startDateTime += morningRunTime;
            endDateTime += afternoonRunTime;
        }
        else if (runFor.TimeOfDay >= morningRunTime && runFor.TimeOfDay < afternoonRunTime)
        {
            startDateTime = startDateTime.AddDays(-1);
            startDateTime += afternoonRunTime;
            endDateTime += morningRunTime;
        }
        else
        {
            startDateTime += morningRunTime;
            endDateTime += afternoonRunTime;
        }

        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                if (runFor.TimeOfDay < morningRunTime)
                {
                    throw new InvalidRunDateException($"Cannot run before 8:30 AM on Mondays.", runFor, dayOfWeek);
                }
                else if (runFor.TimeOfDay >= morningRunTime && runFor.TimeOfDay < afternoonRunTime)
                {
                    startDateTime = startDateTime.AddDays(-2);
                }
                break;
            case DayOfWeek.Tuesday:
                break;
            case DayOfWeek.Wednesday:
                break;
            case DayOfWeek.Thursday:
                break;
            case DayOfWeek.Friday:
                break;
            default:
                throw new InvalidRunDateException($"Jpap Outbound Status job only runs M-F.", runFor, dayOfWeek);
        }

        return (startDateTime, endDateTime);
    }
}
