namespace Library.McKessonDWInterface.DataModel
{
    public class TatSummaryRow
    {
        [ExportHeaderColumnLabel("ORDER_NUMBER")]
        public required string OrderNbr { get; set; }

        [ExportHeaderColumnLabel("FACILITY")]
        public required string Facility { get; set; }

        [ExportHeaderColumnLabel("RX_NBR")]
        public required string RxNbr { get; set; }

        [ExportHeaderColumnLabel("REFILL_NBR")]
        public required long RefillNbr { get; set; }

        [ExportHeaderColumnLabel("DATE_IN")]
        public required DateTime DateIn { get; set; }

        [ExportHeaderColumnLabel("DATE_OUT")]
        public required DateTime DateOut { get; set; }

        [ExportHeaderColumnLabel("DAYS_OVERALL")]
        public required decimal DaysOverall { get; set; }

        [ExportHeaderColumnLabel("DEDUCT_DAYS_INTERVENTION")]
        public required decimal DeductDaysIntervention { get; set; }

        [ExportHeaderColumnLabel("DEDUCT_DAYS_OFF_HOURS")]
        public required decimal DeductDaysOffHours { get; set; }

        [ExportHeaderColumnLabel("DAYS_NET_TAT")]
        public required decimal DaysNetTat { get; set; }

        [ExportHeaderColumnLabel("HAS_EXCEPTIONS")]
        public required string HasExceptions { get; set; }

    }
}
