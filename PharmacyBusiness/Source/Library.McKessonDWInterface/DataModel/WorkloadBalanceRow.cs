using System.Xml.Linq;

namespace Library.McKessonDWInterface.DataModel
{
    public class WorkloadBalanceRow
    {
        public string? FacilityId { get; set; }
        public string? RxNumber { get; set; }
        public string? Ndc { get; set; }
        public string? DrugName { get; set; } 
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string? WorkflowStep { get; set; }
        public string? DeliveryMethod { get; set; } 
        public string? UserId { get; set; }
        public decimal? QtyDispensed { get; set; }
        public string? OwningFacility { get; set; } 
        public string? Wlb { get; set; }
        public decimal? SkipPreVerCode { get; set; } 
    }
}
