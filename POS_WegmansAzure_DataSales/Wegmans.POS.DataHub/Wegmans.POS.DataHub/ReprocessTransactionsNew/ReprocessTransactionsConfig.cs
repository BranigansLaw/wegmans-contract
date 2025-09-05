namespace Wegmans.POS.DataHub.ReprocessTransactionsNew
{
    public class ReprocessTransactionsConfig
    {
        public const string Category = "ReprocessTransactions";

        public int MaxRawTLogQueueSize { get; set; }

        public int MaxDegreesOfParallelism { get; set; }
    }
}
