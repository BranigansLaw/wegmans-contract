using System;

namespace Wegmans.POS.DataHub.ReprocessTransactions;

public class ReprocessTransactionsSettings
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset NextDateToProcess { get; set; }
}
