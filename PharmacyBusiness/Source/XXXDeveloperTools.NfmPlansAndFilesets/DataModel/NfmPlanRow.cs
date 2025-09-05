namespace XXXDeveloperTools.NfmPlansAndFilesets.DataModel
{
    public class NfmPlanRow
    {
        public required string PlanName { get; set; }
        public required string FilesetName { get; set; }
        public required string SourceNodeName { get; set; }
        public required string TargetNodeName { get; set; }
        public required string ParentPlanName { get; set; }
        public required IEnumerable<string> ChildPlanNames { get; set; }
    }
}
