namespace Library.McKessonDWInterface.DataModel
{
    public class TatRawDataRow
    {
        [ExportHeaderColumnLabel("ORDER_NUMBER")]
        public required string OrderNbr { get; set; }

        [ExportHeaderColumnLabel("FACILITY")]
        public required string Facility { get; set; }

        [ExportHeaderColumnLabel("MCKESSON_PATIENT_KEY")]
        public required long McKessonPatientKey { get; set; }

        [ExportHeaderColumnLabel("RX_NBR")]
        public required string RxNbr { get; set; }

        [ExportHeaderColumnLabel("REFILL_NBR")]
        public required long RefillNbr { get; set; }

        [ExportHeaderColumnLabel("DATE_IN")]
        public required DateTime DateIn { get; set; }

        [ExportHeaderColumnLabel("DATE_OUT")]
        public required DateTime DateOut { get; set; }

        [ExportHeaderColumnLabel("WFSD_KEY")]
        public required long WfsdKey { get; set; }

        [ExportHeaderColumnLabel("WFSD_STEP_DESCRIPTION")]
        public required string WfsdDescription { get; set; }
    }
}
