namespace RX.PharmacyBusiness.ETL.CRX540.Core
{
    using System;

    /// <summary>
    /// Initializes a new instance of the PrescriptionSale class
    /// </summary>
    /// <remarks>
    /// Class that represents a Mail Sales record source from McKesson.
    /// These fields are based on the raw data returned from the query to ERx.
    /// </remarks>
    public class PrescriptionSale
    {
        /// <summary>
        /// Initializes a new instance of the PrescriptionSale class 
        /// </summary>
        public PrescriptionSale()
        {
            this.PrescriptionSaleType = PrescriptionSaleTypes.Undefined;
        }

        public DateTime? CancelledDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int CurrentWorkflowStep { get; set; }
        public string LatestFillStatusDesc { get; set; }
        public DateTime? DateOfService { get; set; }
        public long FacilityTableFacilityNumber { get; set; }
        public long FillFactTableFacilityKey { get; set; }
        public long FillFactTableFillSequence { get; set; }
        public long FillFactTableKey { get; set; }
        public DateTime FillFactTablePartitionDate { get; set; }
        public long FillFactTablePatientKey { get; set; }
        public long FillFactTablePrimaryPlanKey { get; set; }
        public long FillFactTableRecordNumber { get; set; }
        public int FillFactTableThirdPartyPlanKey { get; set; }
        public string FillStateCode { get; set; }
        public string HealthCardDesignation { get; set; }
        public string IsBilledAfterReturn { get; set; }
        public string IsReversalForCob { get; set; }
        public long ItemTableOrderNumber { get; set; }
        public long ItemTablePaymentGroupNumber { get; set; }
        public long ItemTableProductNumber { get; set; }
        public string LastFour { get; set; }
        public string OrderNumber { get; set; }
        public int? PartialFillSequence { get; set; }
        public decimal? PatientPricePaid { get; set; }
        public decimal? PatientPricePaidForOrder { get; set; }
        public decimal? PaymentAmount { get; set; }
        public long PaymentNumber { get; set; }
        public long PaymentTableAccountNumber { get; set; }
        public long PaymentTablePaymentTypeNumber { get; set; }
        public long PaymentTableReversalNumber { get; set; }
        public string PaymentTypeName { get; set; }
        public string PrescriptionNumber { get; set; }

        /// <summary>
        /// Gets or sets PrescriptionSaleType which is computed based on a set of business rules.
        /// </summary>
        public PrescriptionSaleTypes PrescriptionSaleType { get; set; }

        public long ProductTableDrugNumber { get; set; }
        public int RefillNumber { get; set; }
        public decimal? ReversalPaymentAmount { get; set; }
        public decimal? ShipHandleFee { get; set; }
        public int SoldDateKey { get; set; }
        public int SoldDateSeconds { get; set; }
        public string StoreNationalDrugProgramsId { get; set; }
        public string StoreNationalProviderId { get; set; }
        public string StoreNumber { get; set; }
        public long? TargetCentralFillFacility { get; set; }
        public decimal? TotalPricePaid { get; set; }
        public decimal? TotalUsualAndCustomary { get; set; }
        public string TrackingNumber { get; set; }

        /// <summary>
        /// These fields are to support feeding to RxMail.Sales_Stage, which in turn feeds Mail Exception reports.
        /// </summary>
        public string DeliveryMethod { get; set; }
        public string CourierName { get; set; }
        public string ShippingMethod { get; set; }
        public decimal? CourierShipCharge { get; set; }
        public long PatientNum { get; set; }
        public long PatientAddressKey { get; set; }
        public string PatientCity { get; set; }
        public string PatientState { get; set; }
        public string PatientZip { get; set; }
        public string CardType { get; set; }

        /// <summary>
        /// Override to format all of the properties of this class.
        /// </summary>
        /// <returns>String representation</returns> 
        public override string ToString()
        {
            var output =
                string.Format(
                    "Sale for Store:{0}, Order:{1}, Rx:{2}, Refill:{3}, FillFactTableKey:{4}, PrescriptionSaleType:{5}",
                    this.StoreNumber,
                    this.OrderNumber,
                    this.PrescriptionNumber,
                    this.RefillNumber,
                    this.FillFactTableKey,
                    this.PrescriptionSaleType);

            return output;
        }
    }
}