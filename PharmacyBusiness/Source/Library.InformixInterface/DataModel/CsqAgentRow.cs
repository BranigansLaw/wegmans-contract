namespace Library.InformixInterface.DataModel
{
    public class CsqAgentRow
    {
        public required short? node_id { get; set; }
        public required decimal? session_id { get; set; }
        public required short? sequence_num { get; set; }
        public required string? session_id_seq { get; set; }
        public required DateTime? start_time { get; set; }
        public required DateTime? end_time { get; set; }
        public required short? contact_disposition { get; set; }
        public required string? originator_dn { get; set; }
        public required string? destination_dn { get; set; }
        public required string? called_number { get; set; }
        public required string? application_name { get; set; }
        public required short? qindex { get; set; }
        public required string? csq_names { get; set; }
        public required short? queue_time { get; set; }
        public required string? agent_name { get; set; }
        public required short? ring_time { get; set; }
        public required int? talk_time { get; set; }
        public required short? work_time { get; set; }
        public required DateTime? latestsynchedtime { get; set; }
    }
}
