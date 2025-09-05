namespace RxAccounting.ThirdPartyClaims.Interfaces.ClaimsLoad.Queries
{
    using System;
    using System.Linq;
    using Rx.Plumbing.CommandQuery;
    using Rx.Plumbing.Data;
    using RxAccounting.ThirdPartyClaims.Interfaces.ClaimsLoad.Business;
    using RxAccounting.ThirdPartyClaims.Interfaces.ClaimsLoad.Contracts;
    using RxAccounting.ThirdPartyClaims.Interfaces.ClaimsLoad.Core;
    using RxAccounting.ThirdPartyClaims.Interfaces.ClaimsLoad.Data;

    public class ThirdPartyClaimsQueryHandler : IHandlesQuery<ThirdPartyClaimsQuery, ThirdPartyClaim[]>
    {
        private readonly IDatabase db;
        private readonly IInsurancePayerFactory insuranceFactory;

        public ThirdPartyClaimsQueryHandler(IDatabase db, IInsurancePayerFactory insuranceFactory)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            if (insuranceFactory == null)
            {
                throw new ArgumentNullException("insuranceFactory");
            }

            this.db = db;
            this.insuranceFactory = insuranceFactory;
        }

        public ThirdPartyClaim[] Execute(ThirdPartyClaimsQuery query)
        {
            var queryResults =
                this.db.Query<FillFact>(
                    SqlQueries.SelectRecordsStagedFromEnterprise, new { RunDate = query.RunDate.ToString("yyyyMMdd") }).ToArray();

            // Set of fields that together should be unique.
            var claims =
                queryResults.GroupBy(
                    r =>
                    new
                        {
                            r.FillFactFillStateTimestamp.Date, 
                            r.OriginatingStoreId, 
                            r.PrescriptionNumber, 
                            r.RefillNumber, 
                            r.FillFactFillStateCode
                        })
                            .Select(
                                g =>
                                this.CreateClaim(
                                    g.OrderBy(x => x.SplitFillNumber)
                                     .ThenBy(x => x.FillFactFillStatePriceNumber)
                                     .ThenBy(x => x.FillFactFillStateCode)
                                     .ToArray()))
                            .ToArray();

            return claims;
        }

        private static void SetValuesForNoCashTransaction(ThirdPartyClaim claim, FillFact fill)
        {
            if (fill.IsCashTransaction())
            {
                claim.PrimaryInsurance.BIN = "CASH";
                return;
            }

            claim.TransactionPrice = fill.ToMoney(fill.FinalPrice);
            claim.IsThirdParty = "Y";

            var message = new MessageAdapter(fill.RequestMessage);

            claim.DiagnosisCode1 = message.DO;
            claim.IngredientCostSubmitted = fill.ToMoney(message.D9);
            claim.UsualAndCustomary = fill.ToMoney(message.DQ);

            int daysSupply;
            if (int.TryParse(message.D5, out daysSupply))
            {
                claim.DaysSupply = daysSupply;
                return;
            }

            daysSupply = fill.DaysSupply ?? 0;

            if (fill.ShortFillStatus == "P" || fill.ShortFillStatus == "C")
            {
                claim.MetricQuantityDispensed = fill.LookupQtyDispensed ?? 0;
                daysSupply = fill.LookupDaysSupply ?? 0;
                claim.AcquisitionCost = fill.ToMoney(fill.LookupAcquisitionCost);
            }

            claim.DaysSupply = daysSupply;
        }

        private ThirdPartyClaim CreateClaim(FillFact[] fills)
        {
            var fill = fills.First();
            var claim = new ThirdPartyClaim
                            {
                                ClaimDate = fill.FillFactFillStateTimestamp.Date, 
                                OriginatingStoreId = fill.OriginatingStoreId,
                                PrescriptionNumber = fill.PrescriptionNumber,
                                RefillNumber = fill.RefillNumber ?? 0, 
                                TransactionCode =
                                    fill.IsReversalTransaction()
                                        ? TransactionCode.NegativeAdjudication
                                        : TransactionCode.PositiveAdjudication, 
                                TransactionPrice = fill.ToMoney(fill.TotalUsualAndCustomary), 
                                UsualAndCustomary = fill.ToMoney(fill.TotalUsualAndCustomary), 
                                IngredientCostSubmitted = fill.ToMoney(fill.IngredientCostSubmitted), 
                                MetricQuantityDispensed = fill.MetricQuantityDispensed ?? 0, 
                                DaysSupply = fill.DaysSupply ?? 0, 
                                AcquisitionCost = fill.ToMoney(fill.AcquisitionCost),
                                AWPCost = fill.ToMoney(fill.AverageWholesalePriceCost), 
                                CompoundIngredientDrugCost1 = fill.ToMoney(fill.CompoundIngredientDrugCost1),
                                CompoundIngredientDrugCost2 = fill.ToMoney(fill.CompoundIngredientDrugCost2),
                                CompoundIngredientDrugCost3 = fill.ToMoney(fill.CompoundIngredientDrugCost3),
                                CompoundIngredientDrugCost4 = fill.ToMoney(fill.CompoundIngredientDrugCost4),
                                CompoundIngredientDrugCost5 = fill.ToMoney(fill.CompoundIngredientDrugCost5),
                                CompoundIngredientDrugCost6 = fill.ToMoney(fill.CompoundIngredientDrugCost6),
                                IncentiveAmountPaid = fill.ToMoney(fill.IncentiveAmountPaid),
                                DiagnosisCode1 = fill.DiagnosisCode1,
                                Schedule = ThirdPartyClaim.DefineScheduleFrom(fill), 
                                PatientGender = ThirdPartyClaim.DefinePatientGenderFrom(fill),
                                CardholderGender = ThirdPartyClaim.DefineCardHolderGender(fill),
                                InitialFillDate = ThirdPartyClaim.DefineInitalFillDateFrom(fill), 
                                Unit = fill.Unit,
                                CardholderPostalCode = fill.CardholderPostalCode,
                                CardholderState = fill.CardholderState,
                                PrescriberNPI = fill.PrescriberNationalProviderId, 
                                PrescriberPostalCode = fill.PrescriberZipcode,
                                PrescriberState = fill.PrescriberState,
                                TransmissionDate = fill.LookupTransmissionDate,
                                CompoundDrugName = fill.IsDrugCompound() ? fill.DispensedDrugName : string.Empty,
                                CompoundNDCDispensed1 = fill.IsDrugCompound() ? fill.CompoundNDCDispensed1 : fill.SubmittedNationalDrugCode,
                                CompoundNDCDispensed2 = fill.IsDrugCompound() ? fill.CompoundNDCDispensed2 : string.Empty,
                                CompoundNDCDispensed3 = fill.IsDrugCompound() ? fill.CompoundNDCDispensed3 : string.Empty,
                                CompoundNDCDispensed4 = fill.IsDrugCompound() ? fill.CompoundNDCDispensed4 : string.Empty,
                                CompoundNDCDispensed5 = fill.IsDrugCompound() ? fill.CompoundNDCDispensed5 : string.Empty,
                                CompoundNDCDispensed6 = fill.IsDrugCompound() ? fill.CompoundNDCDispensed6 : string.Empty,
                                DecimalPackSize = fill.ProductPackSize ?? fill.DrugPackSize, 
                                DispensedDrugName = fill.DispensedDrugName,
                                IsEmergencyFill = fill.IsEmergencyFill,
                                IsManualBill = (fill.ExternalBillingIndicator == "H").ToShortYesNo(), 
                                IsClientGeneric = ThirdPartyClaim.DefineIsClientGenericFrom(fill), 
                                IsMaintenanceDrug = ThirdPartyClaim.DefineIsMaintenanceDrugFrom(fill), 
                                IsPartialFill = ThirdPartyClaim.DefineIsPartialFillFrom(fill), 
                                IsPreferredDrug = fill.IsPreferredDrug,
                                PackSize = fill.ProductPackSize ?? fill.DrugPackSize,
                                QuantityWritten = fill.QuantityWritten ?? 0,
                                Strength = fill.Strength,
                                SubmittedDrugName = fill.DispensedDrugName,
                                SubmittedDrugNDC = fill.IsDrugCompound() ? string.Empty : fill.SubmittedNationalDrugCode, 
                                BasisOfReimbursement =
                                    fill.BasisOfReimbursement == null
                                        ? string.Empty
                                        : fill.BasisOfReimbursement.ToString(),
                                PatientAccountNumber = fill.PatientAccountNumber ?? 0,
                                PatientPostalCode = fill.CardholderPostalCode,
                                PatientState = fill.CardholderState, 
                                ProviderId = fill.ProviderId,
                                CompoundDispensedQuantity1 = fill.CompoundDispensedQuantity1 ?? 0,
                                CompoundDispensedQuantity2 = fill.CompoundDispensedQuantity2 ?? 0,
                                CompoundDispensedQuantity3 = fill.CompoundDispensedQuantity3 ?? 0,
                                CompoundDispensedQuantity4 = fill.CompoundDispensedQuantity4 ?? 0,
                                CompoundDispensedQuantity5 = fill.CompoundDispensedQuantity5 ?? 0,
                                CompoundDispensedQuantity6 = fill.CompoundDispensedQuantity6 ?? 0, 
                                DAW = fill.DispenseAsWritten, 
                                DispensedQuantity = fill.MetricQuantityDispensed ?? 0, 
                                FromFillDate = fill.FromFillDate,
                                HostTransactionNumber = fill.HostTransactionNumber ?? 0,
                                IsCompoundCode = fill.IsCompoundCode,
                                IsThirdParty = "N", 
                                NumberOfRefillsAuthorized = fill.NumberOfRefillsAuthorized ?? 0, 
                                NumberOfRefillsRemaining = fill.NumberOfRefillsRemaining ?? 0,
                                OriginCode = fill.OriginCode,
                                WrittenDate = fill.WrittenDate,
                                GenericCodeNumber = fill.GenericCodeNumber,
                                IsSenior = fill.IsSenior,
                                CentralFillStoreIndicator = fill.CentralFillStoreIndicator,
                                ThirdPartyPlanNumber = fill.ThirdPartyPlanNumber,
                                PrimaryInsurance = this.insuranceFactory.Create(fill),
                                SecondaryInsurance = this.insuranceFactory.Create(fills.Length > 1 && fills[1].SplitFillNumber == "2" ? fills[1] : null),
                                TertiaryInsurance = this.insuranceFactory.Create(fills.Length > 2 && fills[2].SplitFillNumber == "3" ? fills[2] : null),

                                PRS_PRESCRIBER_KEY = fill.PRS_PRESCRIBER_KEY,
                                PRS_PRESCRIBER_NUM = fill.PRS_PRESCRIBER_NUM,
                                PADR_KEY = fill.PADR_KEY
                            };

            SetValuesForNoCashTransaction(claim, fill);

            return claim;
        }
    }
}