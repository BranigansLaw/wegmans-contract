namespace RX.PharmacyBusiness.ETL.Tests.CRX540
{
    using System;
    using System.Linq;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class ERxData
    {
        public DateTime RunDate { get; set; }
        public DateTime QueryDate { get { return this.RunDate.AddDays(-1); } }
        public PrescriptionSale[] AssignSaleAsSoldTestCases { get; set; }
        public PrescriptionSale[] AssignSaleAsFullRefundTestCases { get; set; }
        public PrescriptionSale[] AssignSaleAsPartialRefundTestCases { get; set; }
        public PrescriptionSale[] SaleRejectedAsSoldTestCases { get; set; }
        public PrescriptionSale[] SaleRejectedAsFullRefundTestCases { get; set; }
        public PrescriptionSale[] SaleRejectedAsPartialRefundTestCases { get; set; }
        public PrescriptionSale[] AllRejectedSaleTestCases { get; set; }
        public PrescriptionSale[] AllAssignedSaleTestCases { get; set; }
        public PrescriptionSale[] AllSoldQueryTestCases { get; set; }
        public PrescriptionSale[] AllRefundedQueryTestCases { get; set; }
        public PrescriptionSale[] AllSaleTestCases { get; set; }

        public ERxData(DateTime runDate)
        {
            this.RunDate = runDate;

            this.SaleRejectedAsSoldTestCases = new PrescriptionSale[3] {
                this.SaleIsNotSoldBecauseIsBilledAfterReturn,
                this.SaleIsNotSoldBecauseHasInvalidSoldDateKey,
                this.SaleIsNotSoldBecauseDoesNotMeetAnyOptionalRules};

            this.AssignSaleAsSoldTestCases = new PrescriptionSale[5] {
                this.SaleIsSoldBecauseHasPositivePaymentHasCompletionDate,
                this.SaleIsSoldBecauseHasZeroPaymentHasCreditCardHasNoCancelledDate,
                this.SaleIsSoldBecauseHasPositivePaymentHasNoCreditCardHasNoCreditCardReversal,
                this.SaleIsSoldBecauseHasZeroPaymentHasNoCreditCardHasNoCreditCardReversalHasNoCancelledDate,
                this.SaleIsSoldBecauseHasValidPaymentHasNoCancelledDateHasNoCompletionDateHasRequestRefill};

            this.SaleRejectedAsFullRefundTestCases = new PrescriptionSale[4] {
                this.SaleIsNotFullRefundBecauseIsBilledAfterReturn,
                this.SaleIsNotFullRefundBecauseHasCancelledDate,
                this.SaleIsNotFullRefundBecauseHasInvalidSoldDateKey,
                this.SaleIsNotFullRefundBecauseHasFillStateCodeSold};
            
            this.AssignSaleAsFullRefundTestCases = new PrescriptionSale[2] {
                this.SaleIsFullRefundBecauseHasValidCompletionDateHasNegativePaymentHasNoWorkflowSold,
                this.SaleIsFullRefundBecauseHasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal};

            this.SaleRejectedAsPartialRefundTestCases = new PrescriptionSale[3] {
                this.SaleIsNotPartialRefundBecauseIsBilledAfterReturn,
                this.SaleIsNotPartialRefundBecauseHasCancelledDate,
                this.SaleIsNotPartialRefundBecauseHasInvalidSoldDateKey};

            this.AssignSaleAsPartialRefundTestCases = new PrescriptionSale[5] {
                this.SaleIsPartialRefundBecauseHasReversalDifferentFromPayment,
                this.SaleIsPartialRefundBecauseHasWorkflowSold,
                this.SaleIsPartialRefundBecauseHasNegativePaymentHasValidCompletionDate,
                this.SaleIsPartialRefundBecauseHasZeroPaymentHasCreditCardReversalHasValidSoldDateKey,
                this.SaleIsNotFullRefundBecauseHasReversalDifferentThanPayment};

            this.AllRejectedSaleTestCases = ConcatArrays<PrescriptionSale>(
                this.SaleRejectedAsSoldTestCases,
                this.SaleRejectedAsFullRefundTestCases,
                this.SaleRejectedAsPartialRefundTestCases);

            this.AllAssignedSaleTestCases = ConcatArrays<PrescriptionSale>(
                this.AssignSaleAsSoldTestCases,
                this.AssignSaleAsFullRefundTestCases,
                this.AssignSaleAsPartialRefundTestCases);

            this.AllSaleTestCases = ConcatArrays<PrescriptionSale>(
                this.AllRejectedSaleTestCases,
                this.AllAssignedSaleTestCases);

            this.AllSoldQueryTestCases = ConcatArrays<PrescriptionSale>(
                this.SaleRejectedAsSoldTestCases,
                this.AssignSaleAsSoldTestCases);

            this.AllRefundedQueryTestCases = ConcatArrays<PrescriptionSale>(
                this.SaleRejectedAsFullRefundTestCases,
                this.AssignSaleAsFullRefundTestCases,
                this.SaleRejectedAsPartialRefundTestCases,
                this.AssignSaleAsPartialRefundTestCases);
        }

        public static T[] ConcatArrays<T>(params T[][] arrays)
        {
            return arrays.SelectMany(array => array.Select(arr => arr)).ToArray();
        }

        /// <summary>
        /// Sold records are required to have a valid IsBilledAfterReturn value.
        /// This test case has been given an invalid IsBilledAfterReturn value.
        /// This test case should return false when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotSoldBecauseIsBilledAfterReturn
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new DateTime?(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "Y",
                    PatientPricePaid = 5,
                    PaymentAmount = 5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "1000101",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Sold records are required to have a valid SoldDateKey value.
        /// This test case has been given an invalid SoldDateKey value.
        /// This test case should return false when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotSoldBecauseHasInvalidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "1000102",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.AddDays(-1).ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Sold records are required to meet at least one of the optional Sold business rules.
        /// This test case record does not meet any of the optional business rules.
        /// This test case should return false when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotSoldBecauseDoesNotMeetAnyOptionalRules
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "1000103",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Sold records are required to meet at least one of the optional Sold business rules.
        /// This test case record meets the optional Sold business rule HasPositivePaymentHasCompletionDate.
        /// This test case should be assigned as PrescriptionSaleTypes.Sold when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsSoldBecauseHasPositivePaymentHasCompletionDate
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "1000201",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10     
                };
            }
        }

        /// <summary>
        /// Sold records are required to meet at least one of the optional Sold business rules.
        /// This test case record meets the optional Sold business rule HasZeroPaymentHasCreditCardHasNoCancelledDate.
        /// This test case should be assigned as PrescriptionSaleTypes.Sold when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsSoldBecauseHasZeroPaymentHasCreditCardHasNoCancelledDate
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "1000202",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Sold records are required to meet at least one of the optional Sold business rules.
        /// This test case record meets the optional Sold business rule HasPositivePaymentHasNoCreditCardHasNoCreditCardReversal.
        /// This test case should be assigned as PrescriptionSaleTypes.Sold when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsSoldBecauseHasPositivePaymentHasNoCreditCardHasNoCreditCardReversal
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 5,
                    PaymentTypeName = PaymentTypes.Check,
                    PrescriptionNumber = "1000203",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10       
                };
            }
        }

        /// <summary>
        /// Sold records are required to meet at least one of the optional Sold business rules.
        /// This test case record meets the optional Sold business rule HasZeroPaymentHasNoCreditCardHasNoCreditCardReversalHasNoCancellationDate.
        /// This test case should be assigned as PrescriptionSaleTypes.Sold when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsSoldBecauseHasZeroPaymentHasNoCreditCardHasNoCreditCardReversalHasNoCancelledDate
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.Check,
                    PrescriptionNumber = "1000204",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10       
                };
            }
        }

        /// <summary>
        /// Sold records are required to meet at least one of the optional Sold business rules.
        /// This test case record meets the optional Sold business rule eHasValidPaymentHasNoCancellationDateHasNoCompletionDateHasRequestRefill.
        /// This test case should be assigned as PrescriptionSaleTypes.Sold when passed into IsSoldRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsSoldBecauseHasValidPaymentHasNoCancelledDateHasNoCompletionDateHasRequestRefill
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = new Nullable<DateTime>(),
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "RR1000205",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10       
                };
            }
        }

        /// <summary>
        /// Full Refund records are required to have a valid IsBilledAfterReturn value.
        /// This test case has been given an invalid IsBilledAfterReturn value.
        /// This test case should return false when passed into IsFullRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotFullRefundBecauseIsBilledAfterReturn
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "Y",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "2000101",
                    ReversalPaymentAmount = 5,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Full Refund records are required to have a null CancelledDate value.
        /// This test case has been given an invalid CancelledDate value.
        /// This test case should return false when passed into IsFullRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotFullRefundBecauseHasCancelledDate
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = this.QueryDate,
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "2000102",
                    ReversalPaymentAmount = 5,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Full Refund records are required to have a valid SoldDateKey value.
        /// This test case has been given an invalid SoldDateKey value.
        /// This test case should return false when passed into IsFullRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotFullRefundBecauseHasInvalidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "2000103",
                    ReversalPaymentAmount = 5,
                    SoldDateKey = 0,
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Full Refund records are required to have a Reversal Payment Amount same as Payment Amount value.
        /// This test case has been given different Reversal Payment Amount and Payment Amount values.
        /// This test case should return false when passed into IsFullRefundRule.IsMetBy, but 
        /// it should be assigned as PrescriptionSaleTypes.CreditCardPartialRefund when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotFullRefundBecauseHasReversalDifferentThanPayment
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "2000104",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Full Refund records are required to meet at least one of the optional Full Refund business rules.
        /// This test case record meets the optional Full Refund business rule HasValidCompletionDateHasNegativePaymentHasNoWorkflowSold.
        /// This test case should be assigned as PrescriptionSaleTypes.CreditCardFullRefund when passed into IsFullRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsFullRefundBecauseHasValidCompletionDateHasNegativePaymentHasNoWorkflowSold
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "2000201",
                    ReversalPaymentAmount = 5,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Full Refund records are required to meet at least one of the optional Full Refund business rules.
        /// This test case record meets the optional Full Refund business rule HasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal.
        /// This test case should be assigned as PrescriptionSaleTypes.CreditCardFullRefund when passed into IsFullRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsFullRefundBecauseHasValidSoldDateKeyHasZeroPaymentHasCreditCardReversal
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "2000202",
                    ReversalPaymentAmount = 0,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to have a valid IsBilledAfterReturn value.
        /// This test case has been given an invalid IsBilledAfterReturn value.
        /// This test case should return false when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotPartialRefundBecauseIsBilledAfterReturn
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "Y",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "3000101",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to have a null CancelledDate value.
        /// This test case has been given an invalid CancelledDate value.
        /// This test case should return false when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotPartialRefundBecauseHasCancelledDate
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = this.QueryDate,
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "3000102",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to have a valid SoldDateKey value.
        /// This test case has been given an invalid SoldDateKey value.
        /// This test case should return false when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsNotPartialRefundBecauseHasInvalidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "3000103",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = 0,
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to meet at least one of the optional Partial Refund business rules.
        /// This test case record meets the optional Partial Refund business rule HasReversalDifferentFromPayment.
        /// This test case should be assigned as PrescriptionSaleTypes.CreditCardPartialRefund when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsPartialRefundBecauseHasReversalDifferentFromPayment
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "3000201",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to meet at least one of the optional Partial Refund business rules.
        /// This test case record meets the optional Partial Refund business rule HasWorkflowSold.
        /// This test case should be assigned as PrescriptionSaleTypes.CreditCardPartialRefund when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsPartialRefundBecauseHasWorkflowSold
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.Sold,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "3000202",
                    ReversalPaymentAmount = 5,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to meet at least one of the optional Partial Refund business rules.
        /// This test case record meets the optional Partial Refund business rule HasNegativePaymentHasValidCompletionDate.
        /// This test case should be assigned as PrescriptionSaleTypes.CreditCardPartialRefund when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsPartialRefundBecauseHasNegativePaymentHasValidCompletionDate
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "3000203",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// Partial Refund records are required to meet at least one of the optional Partial Refund business rules.
        /// This test case record meets the optional Partial Refund business rule HasZeroPaymentHasCreditCardReversalHasValidSoldDateKey.
        /// This test case should be assigned as PrescriptionSaleTypes.CreditCardPartialRefund when passed into IsPartialRefundRule.IsMetBy.
        /// </summary>
        public PrescriptionSale SaleIsPartialRefundBecauseHasZeroPaymentHasCreditCardReversalHasValidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "3000204",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Prescription Sale Items are required to meet all of the business rules defined within Business.IsItemPrescriptionSaleRule.
        /// This test case has been given an invalid SoldDateKey value.
        /// This test case should return false when passed into Business.IsItemPrescriptionSaleRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotPrescriptionSaleBecauseHasInvalidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "4000101",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.AddDays(-1).ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Prescription Sale Items are required to meet all of the business rules defined within Business.IsItemPrescriptionSaleRule.
        /// This test case has been given an invalid PaymentTypeName value.
        /// This test case should return false when passed into Business.IsItemPrescriptionSaleRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotPrescriptionSaleBecauseHasCustomerService
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CustomerService,
                    PrescriptionNumber = "4000102",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Prescription Sale Items are required to meet all of the business rules defined within Business.IsItemPrescriptionSaleRule.
        /// This test case has been given an invalid PaymentTypeName value.
        /// This test case should return false when passed into Business.IsItemPrescriptionSaleRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotPrescriptionSaleBecauseHasGetCreditCard
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.GetCreditCard,
                    PrescriptionNumber = "4000103",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Prescription Sale Items are required to meet all of the business rules defined within Business.IsItemPrescriptionSaleRule.
        /// This test case record meets all of the business rules.
        /// This test case should be assigned as GeneralAccountingRetailTypes.PrescriptionSale when passed into IsItemPrescriptionSaleRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsPrescriptionSaleBecauseHasValidSoldDateHasNoCustomerServiceHasNoGetCreditCard
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "4000201",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Shipping Charge Items are required to meet all of the business rules defined within Business.IsItemShippingChargeRule.
        /// This test case has been given an invalid SoldDateKey value.
        /// This test case should return false when passed into Business.IsItemShippingChargeRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotShippingChargeBecauseHasInvalidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "5000101",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.AddDays(-1).ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Shipping Charge Items are required to meet all of the business rules defined within Business.IsItemShippingChargeRule.
        /// This test case has been given an invalid SoldDate value.
        /// This test case should return false when passed into Business.IsItemShippingChargeRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotShippingChargeBecauseHasZeroShipHandleFee
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "5000102",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 0
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Shipping Charge Items are required to meet all of the business rules defined within Business.IsItemShippingChargeRule.
        /// This test case record meets all of the business rules.
        /// This test case should be assigned as GeneralAccountingRetailTypes.ShippingCharge when passed into IsItemShippingChargeRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsShippingChargeBecauseHasValidSoldDateHasNonZeroShipHandleFee
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "5000201",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Third Party Payment Items are required to meet all of the business rules defined within Business.IsItemThirdPartyPaymentRule.
        /// This test case has been given an invalid SoldDateKey value.
        /// This test case should return false when passed into Business.IsItemThirdPartyPaymentRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotThirdPartyPaymentBecauseHasInvalidSoldDateKey
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "6000101",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.AddDays(-1).ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Third Party Payment Items are required to meet all of the business rules defined within Business.IsItemThirdPartyPaymentRule.
        /// This test case has been given an invalid PaymentTypeName value.
        /// This test case should return false when passed into Business.IsItemThirdPartyPaymentRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotThirdPartyPaymentBecauseHasCustomerService
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CustomerService,
                    PrescriptionNumber = "6000102",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Third Party Payment Items are required to meet all of the business rules defined within Business.IsItemThirdPartyPaymentRule.
        /// This test case has been given an invalid PaymentTypeName value.
        /// This test case should return false when passed into Business.IsItemThirdPartyPaymentRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsNotThirdPartyPaymentBecauseHasGetCreditCard
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.GetCreditCard,
                    PrescriptionNumber = "6000103",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        /// <summary>
        /// General Accounting catagorizes the payments asociated with Prescription sales to Items of different types (see Core.GeneralAccountingRetailTypes).
        /// Third Party Payment Items are required to meet all of the business rules defined within Business.IsItemThirdPartyPaymentRule.
        /// This test case record meets all of the business rules.
        /// This test case should be assigned as GeneralAccountingRetailTypes.ThirdPartyPayment when passed into IsItemThirdPartyPaymentRule.IsMetBy.
        /// </summary>
        public PrescriptionSale ItemIsThirdPartyPaymentBecauseHasValidSoldDateHasValidPaymentTypeHasNonZeroInsurancePayment
        {
            get
            {
                return new PrescriptionSale
                {
                    PrescriptionSaleType = PrescriptionSaleTypes.Sold,
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = 0,
                    PaymentTypeName = PaymentTypes.CreditCardReversal,
                    PrescriptionNumber = "6000201",
                    ReversalPaymentAmount = 4,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10
                };
            }
        }

        public PrescriptionSale SaleIsNotFullRefundBecauseHasFillStateCodeSold
        {
            get
            {
                return new PrescriptionSale
                {
                    CancelledDate = new Nullable<DateTime>(),
                    CompletionDate = this.QueryDate,
                    CurrentWorkflowStep = WorkflowSteps.Undefined,
                    LatestFillStatusDesc = FillStatusTypes.NA,
                    FillFactTablePartitionDate = this.QueryDate,
                    IsBilledAfterReturn = "N",
                    PatientPricePaid = 5,
                    PaymentAmount = -5,
                    PaymentTypeName = PaymentTypes.CreditCard,
                    PrescriptionNumber = "2000104",
                    ReversalPaymentAmount = -5,
                    SoldDateKey = Convert.ToInt32(this.QueryDate.ToString("yyyyMMdd")),
                    SoldDateSeconds = 1,
                    TotalPricePaid = 30,
                    TotalUsualAndCustomary = 30,
                    ShipHandleFee = 10,
                    FillStateCode = FillStateCodes.Sold
                };
            }
        }
    }
}
