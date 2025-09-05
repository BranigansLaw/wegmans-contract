namespace Wegmans.POS.DataHub.BatchModifier
{
    public class BatchModifierOptions
    {
        public const string Category = "BatchModifierOptions";

        public int MaxDegreeOfParallelismForQueueing { get; set; }
    }
}
