namespace RX.PharmacyBusiness.ETL.CRX582.Core
{
    using System;

    public class SupplementalData
    {
        //Claim keys:
        public DateTime ClaimDate { get; set; }
        public string OriginatingStoreId { get; set; }
        public string PrescriptionNumber { get; set; }
        public int RefillNumber { get; set; }
        public string ERxSplitFillNumber { get; set; }
        public string FillFactFillStateCode { get; set; }
        public long OrderNumber { get; set; }

        //Supplemental data elements:
        public string FacilityOrderNumber { get; set; }
        public string StoreNPI { get; set; }

        public bool IsReversalTransaction()
        {
            return this.FillFactFillStateCode == FillFactStatus.NegativeAdjudication;
        }
    }
}
