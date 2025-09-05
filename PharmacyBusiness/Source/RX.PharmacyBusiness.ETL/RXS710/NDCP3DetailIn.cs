namespace RX.PharmacyBusiness.ETL.RXS710
{
    using System;

    public class NDCP3DetailIn
    {
        public string PharmacyNpi { get; set; }
        public string PrescriberNpi { get; set; }
        public decimal? DaysSupply { get; set; }
        public string RxNumber { get; set; }
        public decimal RefillNumber { get; set; }
        public string Gender { get; set; }
        public string TpPlanNum { get; set; }
        public decimal? QuantityDispensed { get; set; }
        public string IsCompound { get; set; }
        public decimal TotalUserDefined { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal PatientPayAmount { get; set; }
        public DateTime DispenseDate { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime PrescribedDate { get; set; }
        public string DawCode { get; set; }
        public string TpPersonCode { get; set; }
        public decimal? RefillsAuthorized { get; set; }
        public string BinNumber { get; set; }
        public string UnexpandedDirections { get; set; }
        public string NcpdpNumber { get; set; }
        public string PlanCode { get; set; }
        public decimal? CompoundNumber { get; set; }
        public string DispensedNdc { get; set; }
        public string ProductSchedule { get; set; }
        public string WrittenNdc { get; set; }
        public string PrescriberDea { get; set; }
        public string PrescriberZip { get; set; }
        public string PatientZip { get; set; }
        public string PlanGroup { get; set; }
        public string PrescriberFirstName { get; set; }
        public string PrescriberMiddleName { get; set; }
        public string PrescriberLastName { get; set; }
        public string PlanName { get; set; }
        public string DrugDescription { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientMiddleName { get; set; }
        public DateTime FillStateChangeTimestamp { get; set; }
    }
}