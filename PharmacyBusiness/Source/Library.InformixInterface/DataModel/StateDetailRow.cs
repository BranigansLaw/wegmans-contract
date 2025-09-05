namespace Library.InformixInterface.DataModel
{
    public class StateDetailRow
    {
        public required string? agent_name { get; set; }
        public required string? agent_login_id { get; set; }
        public required string? agent_extension { get; set; }
        public required DateTime? transition_time { get; set; }
        public required string? agent_state { get; set; }
        public required string? reason_code { get; set; }
        public required int? duration { get; set; }
        public required DateTime? latestsynchedtime { get; set; }
    }
}
