namespace RX.PharmacyBusiness.ETL.CRX582.Exporters
{
    using RX.PharmacyBusiness.ETL.CRX582.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class SmartRxClaimsFor1010data
    {
        private const string fileDelimiter = "|";
        private string outputFileLocation;
        private IFileManager fileManager { get; set; }
        private string targetRecipients { get; set; }
        private bool isFormattedForTenUp { get; set; }
        private FileHelper fileHelper;

        public SmartRxClaimsFor1010data(string outputFileLocation, string targetRecipients)
        {
            this.outputFileLocation = outputFileLocation;
            this.fileManager = this.fileManager ?? new FileManager();
            this.targetRecipients = targetRecipients;
            this.isFormattedForTenUp = (targetRecipients == "TENTEN_TENUP" || targetRecipients == "TENTEN_TENUP_ERXCERT");
            this.fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public void Export(ICollection<ThirdPartyClaimRecord> records, DateTime runDate)
        {
            if (!records.Any())
            {
                throw new Exception("There is no SmartRx data available today, so check timing of predecessor jobs.");
            }

            var output = new StringBuilder();
            output.AppendLine(CreateHeaderRecord());

            //Each insurance level per claim has its own row of data for SmartRx requirements.
            foreach (var record in records
                                    .OrderBy(x => x.ClaimDate)
                                    .ThenBy(x => x.OriginatingStoreId)
                                    .ThenBy(x => x.PrescriptionNumber)
                                    .ThenBy(x => x.RefillNumber)
                                    .ThenBy(x => x.TransmissionDate)
                                    .ThenBy(x => x.TransactionCode))
            {
                //Primary insurance:
                output.AppendLine(CreateSmartRxRecordFor1010data(record, 1));

                if (string.IsNullOrEmpty(record.CardholderId_2) == false)
                {
                    //Secondary insurance:
                    output.AppendLine(CreateSmartRxRecordFor1010data(record, 2));
                }

                if (string.IsNullOrEmpty(record.CardholderId_3) == false)
                {
                    //Tertiary insurance:
                    output.AppendLine(CreateSmartRxRecordFor1010data(record, 3));
                }
            }

            this.fileManager.WriteAllText(this.GetFileName(runDate), output.ToString());
            this.fileHelper.CopyFileToArchiveForQA(this.GetFileName(runDate));
        }

        private string CreateSmartRxRecordFor1010data(ThirdPartyClaimRecord tpcRecord, int insurancePayer)
        {
            var fields = new Collection<string>();

            fields.Add(tpcRecord.OriginatingStoreId);
            fields.Add(tpcRecord.PrescriptionNumber);
            fields.Add(tpcRecord.RefillNumber.ToString());

            fields.Add(tpcRecord.FromFillDate.HasValue == true ?
                ((this.isFormattedForTenUp)? 
                    TenTenHelper.FormatDateWithoutTimeForTenUp(tpcRecord.FromFillDate.Value) :
                    tpcRecord.FromFillDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                ) : 
                string.Empty);

            fields.Add((insurancePayer == 1) ? tpcRecord.CardholderId_1 : (insurancePayer == 2) ?
                tpcRecord.CardholderId_2 : tpcRecord.CardholderId_3); //CardholderId
            fields.Add(tpcRecord.CardholderGender);
            fields.Add((insurancePayer == 1) ? tpcRecord.CardholderId_2 : (insurancePayer == 2) ?
                tpcRecord.CardholderId_3 : string.Empty); //OtherPayerCardholderId
            fields.Add(tpcRecord.CardholderPostalCode);
            fields.Add(tpcRecord.CardholderState);
            fields.Add((insurancePayer == 1) ? tpcRecord.NetworkId_1 : (insurancePayer == 2) ?
                tpcRecord.NetworkId_2 : tpcRecord.NetworkId_3); //NetworkId
            fields.Add(TenTenHelper.CleanStringForTenUp(tpcRecord.CompoundDrugName));
            fields.Add(tpcRecord.CompoundNDCDispensed1);
            fields.Add(tpcRecord.CompoundNDCDispensed2);
            fields.Add(tpcRecord.CompoundNDCDispensed3);
            fields.Add(tpcRecord.CompoundNDCDispensed4);
            fields.Add((tpcRecord.DecimalPackSize ?? 0M).ToString("0.00"));
            fields.Add(tpcRecord.DiagnosisCode1);
            fields.Add(tpcRecord.DiagnosisCodeCount.ToString());
            fields.Add(tpcRecord.DiagnosisCodeQualifier1);
            fields.Add(TenTenHelper.CleanStringForTenUp(tpcRecord.DispensedDrugName));
            fields.Add(tpcRecord.IsClientGeneric);
            fields.Add(tpcRecord.IsEmergencyFill);
            fields.Add(tpcRecord.IsMaintenanceDrug);
            fields.Add(tpcRecord.IsPartialFill);
            fields.Add(tpcRecord.IsPreferredDrug);
            fields.Add((tpcRecord.MetricQuantityDispensed ?? 0M).ToString("0.00"));
            fields.Add((tpcRecord.PackSize ?? 0M).ToString("0.00"));
            fields.Add((tpcRecord.QuantityWritten ?? 0M).ToString("0.00"));
            fields.Add(tpcRecord.Schedule);
            fields.Add(tpcRecord.Strength);
            fields.Add(TenTenHelper.CleanStringForTenUp(tpcRecord.SubmittedDrugName));
            fields.Add(tpcRecord.SubmittedDrugNDC);
            fields.Add(TenTenHelper.CleanStringForTenUp(tpcRecord.Unit));
            fields.Add(tpcRecord.IsManualBill);
            fields.Add(tpcRecord.AcquisitionCost.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? (tpcRecord.AdjudicatedAmount_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.AdjudicatedAmount_2 ?? 0M).ToString("0.00") : (tpcRecord.AdjudicatedAmount_3 ?? 0M).ToString("0.00")); //AdjudicatedAmount
            fields.Add(tpcRecord.AWPCost.ToString("0.00"));
            fields.Add((string.IsNullOrEmpty(tpcRecord.BasisOfReimbursement) ? string.Empty : tpcRecord.BasisOfReimbursement).PadLeft(2, '0'));
            fields.Add(tpcRecord.CompoundIngredientDrugCost1.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost2.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost3.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost4.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.CopayAmount_2 ?? 0M).ToString("0.00") : (tpcRecord.CopayAmount_3 ?? 0M).ToString("0.00")); //CopayAmount
            fields.Add((insurancePayer == 1) ? (tpcRecord.DispensingFeePaid_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.DispensingFeePaid_2 ?? 0M).ToString("0.00") : (tpcRecord.DispensingFeePaid_3 ?? 0M).ToString("0.00")); //DispensingFeePaid
            fields.Add(tpcRecord.IncentiveAmountPaid.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? (tpcRecord.IngredientCostPaid_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.IngredientCostPaid_2 ?? 0M).ToString("0.00") : (tpcRecord.IngredientCostPaid_3 ?? 0M).ToString("0.00")); //IngredientCostPaid
            fields.Add(tpcRecord.IngredientCostSubmitted.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? (0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00") : (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00")); //OtherAmountClaimSubmitted
            fields.Add((insurancePayer == 1) ? (0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00") : (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00")); //OtherPPRespAmount
            fields.Add((insurancePayer == 1) ? (tpcRecord.PatientPayAmount_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.PatientPayAmount_2 ?? 0M).ToString("0.00") : (tpcRecord.PatientPayAmount_3 ?? 0M).ToString("0.00")); //PatientPayAmount
            fields.Add(tpcRecord.TransactionPrice.ToString("0.00"));
            fields.Add(tpcRecord.UsualAndCustomary.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? tpcRecord.ThirdPartySubmitType_1 : (insurancePayer == 2) ?
                tpcRecord.ThirdPartySubmitType_2 : tpcRecord.ThirdPartySubmitType_3); //ThirdPartySubmitType
            fields.Add(tpcRecord.PatientAccountNumber.ToString());
            fields.Add(tpcRecord.PatientGender);
            fields.Add(tpcRecord.PatientMedicarePartDCoverage);
            fields.Add(tpcRecord.PatientMedigapId);
            fields.Add(tpcRecord.PatientPostalCode);
            fields.Add((insurancePayer == 1) ? (tpcRecord.PatientRelationshipCode_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.PatientRelationshipCode_2 ?? 0M).ToString("0.00") : (tpcRecord.PatientRelationshipCode_3 ?? 0M).ToString("0.00")); //PatientRelationshipCode
            fields.Add(tpcRecord.PatientState);
            fields.Add((insurancePayer == 1) ? tpcRecord.BIN_1 : (insurancePayer == 2) ?
                tpcRecord.BIN_2 : tpcRecord.BIN_3); //BIN
            fields.Add((insurancePayer == 1) ? tpcRecord.OtherCoverageCode_1 : (insurancePayer == 2) ?
                tpcRecord.OtherCoverageCode_2 : tpcRecord.OtherCoverageCode_3); //OtherCoverageCode
            fields.Add((insurancePayer == 1) ? tpcRecord.PayerId_2 : (insurancePayer == 2) ?
                tpcRecord.PayerId_3 : string.Empty); //OtherPayerId
            fields.Add((insurancePayer == 1) ? tpcRecord.PayerId_1 : (insurancePayer == 2) ?
                tpcRecord.PayerId_2 : tpcRecord.PayerId_3); //PayerId
            fields.Add((insurancePayer == 1) ? tpcRecord.ProcessorControlNumber_1 : (insurancePayer == 2) ?
                tpcRecord.ProcessorControlNumber_2 : tpcRecord.ProcessorControlNumber_3); //ProcessorControlNumber
            fields.Add((insurancePayer == 1) ? tpcRecord.BIN_2 : (insurancePayer == 2) ?
                tpcRecord.BIN_3 : string.Empty); //SecondaryBIN
            fields.Add(string.Empty); //SecondaryId
            fields.Add(tpcRecord.ProviderId);
            fields.Add(tpcRecord.ProviderIdQualifier);
            fields.Add((insurancePayer == 1) ? tpcRecord.GroupInsuranceNumber_1 : (insurancePayer == 2) ?
                tpcRecord.GroupInsuranceNumber_2 : tpcRecord.GroupInsuranceNumber_3); //GroupInsuranceNumber
            fields.Add((insurancePayer == 1) ? tpcRecord.InsurancePlan_1 : (insurancePayer == 2) ?
                tpcRecord.InsurancePlan_2 : tpcRecord.InsurancePlan_3); //InsurancePlan
            fields.Add((insurancePayer == 1) ? tpcRecord.AuthorizationNumber_1 : (insurancePayer == 2) ?
                tpcRecord.AuthorizationNumber_2 : tpcRecord.AuthorizationNumber_3); //AuthorizationNumber
            fields.Add(tpcRecord.CompoundDispensedQuantity1.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity2.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity3.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity4.ToString());
            fields.Add(tpcRecord.DAW);
            fields.Add(tpcRecord.DaysSupply.ToString());
            fields.Add((tpcRecord.DispensedQuantity ?? 0M).ToString("0.00"));
            fields.Add(tpcRecord.HostTransactionNumber.ToString());
            fields.Add((insurancePayer == 1) ? tpcRecord.HowRecordSubmitted_1 : (insurancePayer == 2) ?
                tpcRecord.HowRecordSubmitted_2 : tpcRecord.HowRecordSubmitted_3); //HowRecordSubmitted

            fields.Add((tpcRecord.InitialFillDate.HasValue == true) ?
                ((this.isFormattedForTenUp) ?
                    TenTenHelper.FormatDateWithoutTimeForTenUp(tpcRecord.InitialFillDate.Value) :
                    tpcRecord.InitialFillDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                ) : 
                string.Empty);

            fields.Add(tpcRecord.IsCompoundCode);
            fields.Add(tpcRecord.IsThirdParty);
            fields.Add(tpcRecord.NumberOfRefillsAuthorized.ToString());
            fields.Add(tpcRecord.NumberOfRefillsRemaining.ToString());
            fields.Add((insurancePayer == 1) ? tpcRecord.ERxSplitFillNumber_1 : (insurancePayer == 2) ?
                tpcRecord.ERxSplitFillNumber_2 : tpcRecord.ERxSplitFillNumber_3); //SplitFillNumber
            fields.Add(tpcRecord.TransactionCode);

            fields.Add((tpcRecord.TransmissionDate.HasValue == true) ?
                ((this.isFormattedForTenUp) ?
                    TenTenHelper.FormatDateWithoutTimeForTenUp(tpcRecord.TransmissionDate.Value) :
                    tpcRecord.TransmissionDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                ) : 
                string.Empty);

            fields.Add((tpcRecord.WrittenDate.HasValue == true) ?
                ((this.isFormattedForTenUp) ?
                    TenTenHelper.FormatDateWithoutTimeForTenUp(tpcRecord.WrittenDate.Value) :
                    tpcRecord.WrittenDate.Value.ToString("yyyy-MM-dd HH:mm:ss")
                ) : 
                string.Empty);

            fields.Add(tpcRecord.PrescriberNPI);
            fields.Add(tpcRecord.PrescriberPostalCode);
            fields.Add(tpcRecord.PrescriberState);

            fields.Add((tpcRecord.PRS_PRESCRIBER_KEY.HasValue == true) ? tpcRecord.PRS_PRESCRIBER_KEY.Value.ToString() : string.Empty);
            fields.Add((tpcRecord.PRS_PRESCRIBER_NUM.HasValue == true) ? tpcRecord.PRS_PRESCRIBER_NUM.Value.ToString() : string.Empty);
            fields.Add((tpcRecord.PADR_KEY.HasValue == true) ? tpcRecord.PADR_KEY.Value.ToString() : string.Empty);

            return string.Join(fileDelimiter, fields.ToArray());
        }

        private static string CreateHeaderRecord()
        {
            return
                string.Format(
                    "ORIGINATINGSTOREID{0}PRESCRIPTIONNUMBER{0}REFILLNUMBER{0}FROMFILLDATE{0}CARDHOLDERID{0}CARDHOLDERGENDER{0}OTHERPAYERCARDHOLDERID{0}CARDHOLDERPOSTALCODE{0}CARDHOLDERSTATE{0}NETWORKID{0}COMPOUNDDRUGNAME{0}COMPOUNDNDCDISPENSED1{0}COMPOUNDNDCDISPENSED2{0}COMPOUNDNDCDISPENSED3{0}COMPOUNDNDCDISPENSED4{0}DECIMALPACKSIZE{0}DIAGNOSISCODE1{0}DIAGNOSISCODECOUNT{0}DIAGNOSISCODEQUALIFIER1{0}DISPENSEDDRUGNAME{0}ISCLIENTGENERIC{0}ISEMERGENCYFILL{0}ISMAINTENANCEDRUG{0}ISPARTIALFILL{0}ISPREFERREDDRUG{0}METRICQUANTITYDISPENSED{0}PACKSIZE{0}QUANTITYWRITTEN{0}SCHEDULE{0}STRENGTH{0}SUBMITTEDDRUGNAME{0}SUBMITTEDDRUGNDC{0}UNIT{0}ISMANUALBILL{0}ACQUISITIONCOST{0}ADJUDICATEDAMOUNT{0}AWPCOST{0}BASISOFREIMBURSEMENT{0}COMPOUNDINGREDIENTDRUGCOST1{0}COMPOUNDINGREDIENTDRUGCOST2{0}COMPOUNDINGREDIENTDRUGCOST3{0}COMPOUNDINGREDIENTDRUGCOST4{0}COPAYAMOUNT{0}DISPENSINGFEEPAID{0}INCENTIVEAMOUNTPAID{0}INGREDIENTCOSTPAID{0}INGREDIENTCOSTSUBMITTED{0}OTHERAMOUNTCLAIMSUBMITTED{0}OTHERPPRESPAMOUNT{0}PATIENTPAYAMOUNT{0}TRANSACTIONPRICE{0}USUALANDCUSTOMARY{0}THIRDPARTYSUBMITTYPE{0}PATIENTACCOUNTNUMBER{0}PATIENTGENDER{0}PATIENTMEDICAREPARTDCOVERAGE{0}PATIENTMEDIGAPID{0}PATIENTPOSTALCODE{0}PATIENTRELATIONSHIPCODE{0}PATIENTSTATE{0}BIN{0}OTHERCOVERAGECODE{0}OTHERPAYERID{0}PAYERID{0}PROCESSORCONTROLNUMBER{0}SECONDARYBIN{0}SECONDARYID{0}PROVIDERID{0}PROVIDERIDQUALIFIER{0}GROUPINSURANCENUMBER{0}INSURANCEPLAN{0}AUTHORIZATIONNUMBER{0}COMPOUNDDISPENSEDQUANTITY1{0}COMPOUNDDISPENSEDQUANTITY2{0}COMPOUNDDISPENSEDQUANTITY3{0}COMPOUNDDISPENSEDQUANTITY4{0}DAW{0}DAYSSUPPLY{0}DISPENSEDQUANTITY{0}HOSTTRANSACTIONNUMBER{0}HOWRECORDSUBMITTED{0}INITIALFILLDATE{0}ISCOMPOUNDCODE{0}ISTHIRDPARTY{0}NUMBEROFREFILLSAUTHORIZED{0}NUMBEROFREFILLSREMAINING{0}SPLITFILLNUMBER{0}TRANSACTIONCODE{0}TRANSMISSIONDATE{0}WRITTENDATE{0}PRESCRIBERNPI{0}PRESCRIBERPOSTALCODE{0}PRESCRIBERSTATE{0}PRS_PRESCRIBER_KEY{0}PRS_PRESCRIBER_NUM{0}PADR_KEY",
                    fileDelimiter);
        }

        private string GetFileName(DateTime runDate)
        {
            string filenamePrefix = this.targetRecipients == "TENTEN_TENUP_ERXCERT" ? "ERXCERT_" : string.Empty;
            return Path.Combine(this.outputFileLocation, string.Format("{1}SmartRxClaim_{0:yyyyMMdd}.txt", runDate, filenamePrefix));
        }
    }
}

