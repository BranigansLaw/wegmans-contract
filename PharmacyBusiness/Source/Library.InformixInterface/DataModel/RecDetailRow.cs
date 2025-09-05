namespace Library.InformixInterface.DataModel
{
    public class RecDetailRow
    {
        public required string? agent_name { get; set; }
        public required string? agent_login_id { get; set; }
        public required string? agent_extension { get; set; }
        public required string? agent_nonipcc_extn { get; set; }
        public required DateTime? call_start_time { get; set; }
        public required DateTime? call_end_time { get; set; }
        public required int? call_duration { get; set; }
        public required string? called_number { get; set; }
        public required string? call_ani { get; set; }
        public required string? call_routed_csq { get; set; }
        public required string? other_csqs { get; set; }
        public required string? call_skills { get; set; }
        public required int? talk_time { get; set; }
        public required int? hold_time { get; set; }
        public required int? wrapup_time { get; set; }
        public required string? type_call { get; set; }
        public required DateTime? latestsynchedtime { get; set; }
    }
}
