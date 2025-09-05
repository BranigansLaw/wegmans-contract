namespace RX.PharmacyBusiness.ETL.CRX582
{
    using Oracle.ManagedDataAccess.Client;
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using RX.PharmacyBusiness.ETL.CRX582.Business;
    using RX.PharmacyBusiness.ETL.CRX582.Contracts;
    using RX.PharmacyBusiness.ETL.CRX582.Core;
    using RX.PharmacyBusiness.ETL.CRX582.Exporters;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX582", "Download Third Party Claims.", "KBA00023012", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAO5XDAO6M9SSN2HDY")]
    public class DownloadThirdPartyClaims : ETLBase
    {
        private IInsurancePayerFactory insuranceFactory;

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string targetRecipients = (this.Arguments["-TargetRecipients"] == null) ? "ALL" : this.Arguments["-TargetRecipients"].ToString();
            string outputLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX582\Output\");
            this.insuranceFactory = new InsurancePayerFactory();
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
            SmartRxVolumeSummary volumeSummary = new SmartRxVolumeSummary(outputLocation);
            SmartRxClaimsForInmar smartRxInmar = new SmartRxClaimsForInmar(outputLocation);
            SmartRxClaimsFor1010data smartRx1010 = new SmartRxClaimsFor1010data(outputLocation, targetRecipients);

            Log.LogInfo("Begin Third Party Claims with RunDate [{0}].", runDate.ToString("MM/dd/yyyy"));

            Log.LogInfo("Step 1 of 6: Call procedure that loads a temp table in McKesson.");
            OracleParameter[] procedureParamsLoad = new OracleParameter[1];
            procedureParamsLoad[0] = new OracleParameter("in_RUN_DATE", OracleDbType.Date, ParameterDirection.Input);
            procedureParamsLoad[0].Value = runDate.Date;
            oracleHelper.CallNonQueryProcedure(
                "ENTERPRISE_RX",
                "Wegmans.THIRD_PARTY_CLAIMS_PKG.Load",
                procedureParamsLoad);

            Log.LogInfo("Step 2 of 6: Get raw data from that temp table.");
            List<FillFact> fillFactList = oracleHelper.DownloadQueryByRunDateToList<FillFact>(
                150,
                @"%BATCH_ROOT%\CRX582\bin\GetFillFacts.sql",
                runDate.Date,
                "ENTERPRISE_RX"
                );
            fileHelper.WriteListToFile<FillFact>(
                fillFactList,
                @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\GetFillFacts_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                false,
                false);

            Log.LogInfo("Step 3 of 6: Transform raw data into Claim data.");
            ThirdPartyClaim[] claims = fillFactList
                .GroupBy(r => new
                {
                    r.FillFactFillStateTimestamp.Date,
                    r.OriginatingStoreId,
                    r.PrescriptionNumber,
                    r.RefillNumber,
                    r.FillFactFillStateCode
                })
                .Select(g =>
                    this.CreateClaim(
                        g.OrderBy(x => x.SplitFillNumber)
                        .ThenBy(x => x.FillFactFillStatePriceNumber)
                        .ThenBy(x => x.FillFactFillStateCode)
                        .ToArray()))
                .ToArray();

            List<ThirdPartyClaimRecord> records = claims
                .Select(claim => new ThirdPartyClaimRecord
                {
                    // Claim key fields:
                    ClaimDate = claim.ClaimDate,
                    OriginatingStoreId = claim.OriginatingStoreId,
                    PrescriptionNumber = claim.PrescriptionNumber,
                    RefillNumber = claim.RefillNumber,
                    TransactionCode = claim.TransactionCode,

                    // Calculated and business rules fields:
                    MetricQuantityDispensed = claim.MetricQuantityDispensed,
                    DaysSupply = claim.DaysSupply,
                    DiagnosisCode1 = claim.DiagnosisCode1,
                    DiagnosisCodeCount = claim.DiagnosisCodeCount,
                    DiagnosisCodeQualifier1 = claim.DiagnosisCodeQualifier1,
                    TransactionPrice = claim.TransactionPrice,
                    UsualAndCustomary = claim.UsualAndCustomary,
                    IngredientCostSubmitted = claim.IngredientCostSubmitted,
                    AcquisitionCost = claim.AcquisitionCost,
                    AWPCost = claim.AWPCost,
                    CompoundIngredientDrugCost1 = claim.CompoundIngredientDrugCost1,
                    CompoundIngredientDrugCost2 = claim.CompoundIngredientDrugCost2,
                    CompoundIngredientDrugCost3 = claim.CompoundIngredientDrugCost3,
                    CompoundIngredientDrugCost4 = claim.CompoundIngredientDrugCost4,
                    CompoundIngredientDrugCost5 = claim.CompoundIngredientDrugCost5,
                    CompoundIngredientDrugCost6 = claim.CompoundIngredientDrugCost6,
                    IncentiveAmountPaid = claim.IncentiveAmountPaid,
                    Schedule = claim.Schedule,
                    PatientGender = claim.PatientGender,

                    // Remaining fields:
                    Unit = claim.Unit,
                    IsManualBill = claim.IsManualBill,
                    CardholderGender = claim.CardholderGender,
                    CardholderPostalCode = claim.CardholderPostalCode,
                    CardholderState = claim.CardholderState,
                    CompoundDrugName = claim.CompoundDrugName,
                    CompoundNDCDispensed1 = claim.CompoundNDCDispensed1,
                    CompoundNDCDispensed2 = claim.CompoundNDCDispensed2,
                    CompoundNDCDispensed3 = claim.CompoundNDCDispensed3,
                    CompoundNDCDispensed4 = claim.CompoundNDCDispensed4,
                    CompoundNDCDispensed5 = claim.CompoundNDCDispensed5,
                    CompoundNDCDispensed6 = claim.CompoundNDCDispensed6,
                    DecimalPackSize = claim.DecimalPackSize,
                    DispensedDrugName = claim.DispensedDrugName,
                    IsClientGeneric = claim.IsClientGeneric,
                    IsEmergencyFill = claim.IsEmergencyFill,
                    IsMaintenanceDrug = claim.IsMaintenanceDrug,
                    IsPartialFill = claim.IsPartialFill,
                    IsPreferredDrug = claim.IsPreferredDrug,
                    PackSize = claim.PackSize,
                    QuantityWritten = claim.QuantityWritten,
                    Strength = claim.Strength,
                    SubmittedDrugName = claim.SubmittedDrugName,
                    SubmittedDrugNDC = claim.SubmittedDrugNDC,
                    BasisOfReimbursement = claim.BasisOfReimbursement,
                    PatientAccountNumber = claim.PatientAccountNumber,
                    PatientMedicarePartDCoverage = claim.PatientMedicarePartDCoverage,
                    PatientMedigapId = claim.PatientMedigapId,
                    PatientPostalCode = claim.PatientPostalCode,
                    PatientState = claim.PatientState,
                    ProviderId = claim.ProviderId,
                    ProviderIdQualifier = claim.ProviderIdQualifier,
                    CompoundDispensedQuantity1 = claim.CompoundDispensedQuantity1,
                    CompoundDispensedQuantity2 = claim.CompoundDispensedQuantity2,
                    CompoundDispensedQuantity3 = claim.CompoundDispensedQuantity3,
                    CompoundDispensedQuantity4 = claim.CompoundDispensedQuantity4,
                    CompoundDispensedQuantity5 = claim.CompoundDispensedQuantity5,
                    CompoundDispensedQuantity6 = claim.CompoundDispensedQuantity6,
                    DAW= claim.DAW,
                    DispensedQuantity = claim.DispensedQuantity,
                    FromFillDate = claim.FromFillDate,
                    HostTransactionNumber = claim.HostTransactionNumber,
                    InitialFillDate = claim.InitialFillDate,
                    IsCompoundCode = claim.IsCompoundCode,
                    IsThirdParty = claim.IsThirdParty,
                    NumberOfRefillsAuthorized = claim.NumberOfRefillsAuthorized,
                    NumberOfRefillsRemaining = claim.NumberOfRefillsRemaining,
                    TransmissionDate = claim.TransmissionDate,
                    WrittenDate = claim.WrittenDate,
                    PrescriberNPI = claim.PrescriberNPI,
                    PrescriberPostalCode = claim.PrescriberPostalCode,
                    PrescriberState = claim.PrescriberState,
                    GenericCodeNumber = claim.GenericCodeNumber,
                    OriginCode = claim.OriginCode,
                    ProcedureModifierCodeCount = claim.ProcedureModifierCodeCount,
                    //IsSenior = claim.IsSenior,
                    //CentralFillStoreIndicator = claim.CentralFillStoreIndicator,
                    //ThirdPartyPlanNumber = claim.ThirdPartyPlanNumber,

                    // Primary Insurance            
                    ERxSplitFillNumber_1 = claim.PrimaryInsurance.ERxSplitFillNumber,
                    CardholderId_1 = claim.PrimaryInsurance.CardholderId,
                    AdjudicatedAmount_1 = claim.PrimaryInsurance.AdjudicatedAmount,
                    CopayAmount_1 = claim.PrimaryInsurance.CopayAmount,
                    DispensingFeePaid_1 = claim.PrimaryInsurance.DispensingFeePaid,
                    IngredientCostPaid_1 = claim.PrimaryInsurance.IngredientCostPaid,
                    PatientPayAmount_1 = claim.PrimaryInsurance.PatientPayAmount,
                    BIN_1 = claim.PrimaryInsurance.BIN,
                    InsurancePlan_1 = claim.PrimaryInsurance.InsurancePlan,
                    HowRecordSubmitted_1 = claim.PrimaryInsurance.HowRecordSubmitted,
                    ProcessorControlNumber_1 = claim.PrimaryInsurance.ProcessorControlNumber,
                    GroupInsuranceNumber_1 = claim.PrimaryInsurance.GroupInsuranceNumber,
                    AuthorizationNumber_1 = claim.PrimaryInsurance.AuthorizationNumber,
                    PatientRelationshipCode_1 = claim.PrimaryInsurance.PatientRelationshipCode,
                    PayerId_1 = claim.PrimaryInsurance.PayerId,
                    NetworkId_1 = claim.PrimaryInsurance.NetworkId,
                    ThirdPartySubmitType_1 = claim.PrimaryInsurance.ThirdPartySubmitType,
                    OtherCoverageCode_1 = claim.PrimaryInsurance.OtherCoverageCode,

                    // Secondary Insurance           
                    ERxSplitFillNumber_2 = claim.SecondaryInsurance.ERxSplitFillNumber,
                    CardholderId_2 = claim.SecondaryInsurance.CardholderId,
                    AdjudicatedAmount_2 = claim.SecondaryInsurance.AdjudicatedAmount,
                    CopayAmount_2 = claim.SecondaryInsurance.CopayAmount,
                    DispensingFeePaid_2 = claim.SecondaryInsurance.DispensingFeePaid,
                    IngredientCostPaid_2 = claim.SecondaryInsurance.IngredientCostPaid,
                    PatientPayAmount_2 = claim.SecondaryInsurance.PatientPayAmount,
                    BIN_2 = claim.SecondaryInsurance.BIN,
                    InsurancePlan_2 = claim.SecondaryInsurance.InsurancePlan,
                    HowRecordSubmitted_2 = claim.SecondaryInsurance.HowRecordSubmitted,
                    ProcessorControlNumber_2 = claim.SecondaryInsurance.ProcessorControlNumber,
                    GroupInsuranceNumber_2 = claim.SecondaryInsurance.GroupInsuranceNumber,
                    AuthorizationNumber_2 = claim.SecondaryInsurance.AuthorizationNumber,
                    PatientRelationshipCode_2 = claim.SecondaryInsurance.PatientRelationshipCode,
                    PayerId_2 = claim.SecondaryInsurance.PayerId,
                    NetworkId_2 = claim.SecondaryInsurance.NetworkId,
                    ThirdPartySubmitType_2 = claim.SecondaryInsurance.ThirdPartySubmitType,
                    OtherCoverageCode_2 = claim.SecondaryInsurance.OtherCoverageCode,

                    // Tertiary Insurance           
                    ERxSplitFillNumber_3 = claim.TertiaryInsurance.ERxSplitFillNumber,
                    CardholderId_3 = claim.TertiaryInsurance.CardholderId,
                    AdjudicatedAmount_3 = claim.TertiaryInsurance.AdjudicatedAmount,
                    CopayAmount_3 = claim.TertiaryInsurance.CopayAmount,
                    DispensingFeePaid_3 = claim.TertiaryInsurance.DispensingFeePaid,
                    IngredientCostPaid_3 = claim.TertiaryInsurance.IngredientCostPaid,
                    PatientPayAmount_3 = claim.TertiaryInsurance.PatientPayAmount,
                    BIN_3 = claim.TertiaryInsurance.BIN,
                    InsurancePlan_3 = claim.TertiaryInsurance.InsurancePlan,
                    HowRecordSubmitted_3 = claim.TertiaryInsurance.HowRecordSubmitted,
                    ProcessorControlNumber_3 = claim.TertiaryInsurance.ProcessorControlNumber,
                    GroupInsuranceNumber_3 = claim.TertiaryInsurance.GroupInsuranceNumber,
                    AuthorizationNumber_3 = claim.TertiaryInsurance.AuthorizationNumber,
                    PatientRelationshipCode_3 = claim.TertiaryInsurance.PatientRelationshipCode,
                    PayerId_3 = claim.TertiaryInsurance.PayerId,
                    NetworkId_3 = claim.TertiaryInsurance.NetworkId,
                    ThirdPartySubmitType_3 = claim.TertiaryInsurance.ThirdPartySubmitType,
                    OtherCoverageCode_3 = claim.TertiaryInsurance.OtherCoverageCode,

                    PRS_PRESCRIBER_KEY = claim.PRS_PRESCRIBER_KEY,
                    PRS_PRESCRIBER_NUM = claim.PRS_PRESCRIBER_NUM,
                    PADR_KEY = claim.PADR_KEY
                }).ToList();

            if (targetRecipients == "ALL" || targetRecipients == "INMAR")
            { 
                Log.LogInfo("Step 4 of 6: Creating volume summary file for vendor Inmar.");
                volumeSummary.Export(records, runDate);
            }
            else
            {
                Log.LogInfo("Step 4 of 6: Skipped because INMAR export not going to targeted recipients.");
            }

            if (targetRecipients == "ALL" || targetRecipients == "INMAR")
            {
                Log.LogInfo("Step 5 of 6: Creating claim detail file for vendor Inmar.");
                List<HipaaPortionOfClaim> hipaaRecords = oracleHelper.DownloadQueryByRunDateToList<HipaaPortionOfClaim>(
                    150,
                    @"%BATCH_ROOT%\CRX582\bin\GettHipaaClaimElements.sql",
                    runDate.Date,
                    "ENTERPRISE_RX"
                    );
                fileHelper.WriteListToFile<HipaaPortionOfClaim>(
                    hipaaRecords,
                    @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\GettHipaaClaimElements_" + runDate.ToString("yyyyMMdd") + ".txt",
                    true,
                    "|",
                    string.Empty,
                    true,
                    false,
                    false);
                smartRxInmar.Export(records, runDate, hipaaRecords);
            }
            else
            {
                Log.LogInfo("Step 5 of 6: Skipped because INMAR export not going to targeted recipients.");
            }

            if (targetRecipients == "ALL" || targetRecipients == "TENTEN_SFTP")
            {
                Log.LogInfo("Step 6 of 6: Creating SmartRx detail file for SFTP to vendor 1010data.");
                smartRx1010.Export(records, runDate);
            }

            if (targetRecipients == "TENTEN_TENUP")
            {
                Log.LogInfo("Step 6 of 6: Creating SmartRx detail file for TenUp to vendor 1010data.");
                smartRx1010.Export(records, runDate);
            }

            if (targetRecipients == "TENTEN_TENUP_ERXCERT")
            {
                Log.LogInfo("Step 6 of 6: Creating ERXCERT SmartRx detail file for TenUp to vendor 1010data.");
                smartRx1010.Export(records, runDate);
            }

            Log.LogInfo("Completed making data feeds for Third Party Claims.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
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
