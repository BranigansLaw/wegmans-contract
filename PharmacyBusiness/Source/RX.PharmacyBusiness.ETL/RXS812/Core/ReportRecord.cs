namespace RX.PharmacyBusiness.ETL.RXS812.Core
{
    public class ReportRecord
    {
        
        public string ReportingName { get; set; } //"Name of person providing reports"
        public string ReportingPhone { get; set; } //"Telephone number of person providing report"
        public string ReportingEmail { get; set; } //"Email of person providing directory information"
        public string PharmacyName { get; set; } //"Pharmacy name (how pharmacy is generally known in the community)"
        public string PharmacyNbr { get; set; } //"Store Number"
        public string ZipCode { get; set; } //"ZipCode"
        public string PharmacyNPI { get; set; } //"Pharmacy NPI"
        public string PrescriberNPI { get; set; } //"Dispensing Encounter: Linked NPI (provider)"
        public string DrugName { get; set; } //"Please include each  Dispensing Encounter: Formulation name"
        public string DrugNDC { get; set; } //"Dispensing per Encounter: Formulation NDC"
        public string DrugQty { get; set; } //"Dispensing Encounter: Count of NDC dispensed per encounter"
    }
}
