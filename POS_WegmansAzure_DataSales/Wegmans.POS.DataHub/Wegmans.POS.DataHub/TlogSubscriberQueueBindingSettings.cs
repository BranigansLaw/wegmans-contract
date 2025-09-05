namespace Wegmans.POS.DataHub
{
    public static class TlogSubscriberQueueBindingSettings
    {
        public const string RawTlogTransactionsEventQueueName = "%" + nameof(RawTlogTransactionsEventQueueName) + "%";
        public const string RawTlogTransactionsPoisonQueueName = "%" + nameof(RawTlogTransactionsPoisonQueueName) + "%";
        public const string TlogEventSubscriberQueueName = "%" + nameof(TlogEventSubscriberQueueName) + "%";
        public const string RawTlogStoreCloseEventQueueName = "%" + nameof(RawTlogStoreCloseEventQueueName) + "%";
    }
}
