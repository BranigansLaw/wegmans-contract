namespace Wegmans.POS.DataHub
{
    public static class TlogSubscriberStorageSettings
    {
        public const string RawTlogDeadletter = "%" + nameof(RawTlogDeadletter) + "%";
        public const string Transactions_container = "%" + nameof(Transactions_container) + "%";
        public const string RawTlog_container = "%" + nameof(RawTlog_container) + "%";
        public const string EventPublisherDeadletter = "%" + nameof(EventPublisherDeadletter) + "%";
        public const string POSDataHubConnectionStringKey = "POSDataHubAccount";
        public const string StoreCloseEvents_container = "%" + nameof(StoreCloseEvents_container) + "%";
    }
}
