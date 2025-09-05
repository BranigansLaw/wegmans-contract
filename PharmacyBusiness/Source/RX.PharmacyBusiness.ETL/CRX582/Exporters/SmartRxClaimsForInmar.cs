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

    public class SmartRxClaimsForInmar
    {
        private const string fileDelimiter = "|";
        private const string detalRecord = "D";
        private const string headerRecord = "H";
        private const string cientIdentifier = "7770";
        private string outputFileLocation;
        private IFileManager fileManager { get; set; }
        private ICollection<HipaaPortionOfClaim> hipaaRecords;
        private FileHelper fileHelper;

        public SmartRxClaimsForInmar(string outputFileLocation)
        {
            this.outputFileLocation = outputFileLocation;
            this.fileManager = this.fileManager ?? new FileManager();
            this.hipaaRecords = new List<HipaaPortionOfClaim>();
            this.fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public void Export(
            ICollection<ThirdPartyClaimRecord> records, 
            DateTime runDate,
            ICollection<HipaaPortionOfClaim> hipaaRecords)
        {
            //Inmar requires HIPAA data to process third party claims. Internal reporting does not need any HIPAA data, so HIPAA data is in that same temp table in the McKesson database.
            this.hipaaRecords = hipaaRecords;

            var output = new StringBuilder();
            output.AppendLine(
                CreateHeaderRecord(
                    records.Count,
                    records.Sum(r => ((r.CopayAmount_1 ?? 0) + (r.CopayAmount_2 ?? 0) + (r.CopayAmount_3 ?? 0))),
                    records.Sum(r => ((r.AdjudicatedAmount_1 ?? 0) + (r.AdjudicatedAmount_2 ?? 0) + (r.AdjudicatedAmount_3 ?? 0))),
                    runDate
                    ));

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
                output.AppendLine(CreateSmartRxRecordForInmar(record, 1));

                if (string.IsNullOrEmpty(record.CardholderId_2) == false)
                {
                    //Secondary insurance:
                    output.AppendLine(CreateSmartRxRecordForInmar(record, 2));
                }

                if (string.IsNullOrEmpty(record.CardholderId_3) == false)
                {
                    //Tertiary insurance:
                    output.AppendLine(CreateSmartRxRecordForInmar(record, 3));
                }
            }

            this.fileManager.WriteAllText(this.GetFileName(runDate), output.ToString());
            this.fileHelper.CopyFileToArchiveForQA(this.GetFileName(runDate));
        }

        private string CreateSmartRxRecordForInmar(ThirdPartyClaimRecord tpcRecord, int insurancePayer)
        {
            HipaaPortionOfClaim hipaaRecord = new HipaaPortionOfClaim();
            if (this.hipaaRecords
                .Where(e =>
                    e.ClaimDate.Date == tpcRecord.ClaimDate.Date &&
                    e.OriginatingStoreId == tpcRecord.OriginatingStoreId &&
                    e.PrescriptionNumber == tpcRecord.PrescriptionNumber &&
                    e.RefillNumber == tpcRecord.RefillNumber &&
                    (
                     (insurancePayer == 1 && (e.ERxSplitFillNumber == "0" || e.ERxSplitFillNumber == "1")) ||
                     (insurancePayer == 2 && e.ERxSplitFillNumber == "2") ||
                     (insurancePayer == 3 && e.ERxSplitFillNumber == "3")
                    ) &&
                    e.IsReversalTransaction() == tpcRecord.IsReversalTransaction()).Count() > 0)
            {
                hipaaRecord = this.hipaaRecords
                    .Where(e =>
                        e.ClaimDate.Date == tpcRecord.ClaimDate.Date &&
                        e.OriginatingStoreId == tpcRecord.OriginatingStoreId &&
                        e.PrescriptionNumber == tpcRecord.PrescriptionNumber &&
                        e.RefillNumber == tpcRecord.RefillNumber &&
                        (
                         (insurancePayer == 1 && (e.ERxSplitFillNumber == "0" || e.ERxSplitFillNumber == "1")) ||
                         (insurancePayer == 2 && e.ERxSplitFillNumber == "2") ||
                         (insurancePayer == 3 && e.ERxSplitFillNumber == "3")
                        ) &&
                        e.IsReversalTransaction() == tpcRecord.IsReversalTransaction())
                    .FirstOrDefault();
            }

            var fields = new Collection<string>();
            string otherPayerCardholderId = (insurancePayer == 1) ? tpcRecord.CardholderId_2 : (insurancePayer == 2) ?
                tpcRecord.CardholderId_3 : string.Empty;

            fields.Add(detalRecord);
            fields.Add(hipaaRecord.CardholderAddress1);
            fields.Add(hipaaRecord.CardholderAddress2);
            fields.Add((insurancePayer == 1) ? tpcRecord.CardholderId_1 : (insurancePayer == 2) ?
                tpcRecord.CardholderId_2 : tpcRecord.CardholderId_3); //CardholderId
            fields.Add(hipaaRecord.CardholderCity);
            fields.Add(string.Empty); //(record.CARDHOLDERCOUNTRYCODE);
            fields.Add(hipaaRecord.CardholderDateOfBirth.HasValue == true ? hipaaRecord.CardholderDateOfBirth.Value.ToString("yyyyMMdd") : string.Empty);
            fields.Add(hipaaRecord.CardholderFirstName);
            fields.Add(tpcRecord.CardholderGender);
            fields.Add(hipaaRecord.CardholderLastName);
            fields.Add(hipaaRecord.CardholderMiddleInitial);
            fields.Add(otherPayerCardholderId); //OtherPayerCardholderId
            if (string.IsNullOrEmpty(otherPayerCardholderId))
            {
                fields.Add(string.Empty);
                fields.Add(string.Empty);
                fields.Add(string.Empty);
                fields.Add(string.Empty);
            }
            else
            {
                fields.Add(hipaaRecord.OtherPayerDateOfBirth.HasValue == true ? hipaaRecord.OtherPayerDateOfBirth.Value.ToString("yyyyMMdd") : string.Empty);
                fields.Add(hipaaRecord.OtherPayerFirstName);
                fields.Add(hipaaRecord.OtherPayerLastName);
                fields.Add(hipaaRecord.OtherPayerMiddleInitial);
            }
            fields.Add(hipaaRecord.CardholderPhoneNumber);
            fields.Add(tpcRecord.CardholderPostalCode);
            fields.Add(string.Empty); //(record.SOCIALSECURITYNUMBER);
            fields.Add(tpcRecord.CardholderState);
            fields.Add(string.Empty); //(record.CONTRACTNUMBER);
            fields.Add(string.Empty); //(record.FORMULARYID);
            fields.Add((insurancePayer == 1) ? tpcRecord.NetworkId_1 : (insurancePayer == 2) ?
                tpcRecord.NetworkId_2 : tpcRecord.NetworkId_3); //NetworkId
            fields.Add(string.Empty); //(record.ALTERNATIVEPRODUCTIDQUALIFIER);
            fields.Add(string.Empty); //(record.ASSOCPRESSERVICEREFNBR);
            fields.Add(string.Empty); //(record.ASSOCPRESSERVICEREFNBRCOMMENT);
            fields.Add(tpcRecord.CompoundDrugName);
            fields.Add(string.Empty); //(record.COMPOUNDINGREDIENTCOMPONENTCNT);
            fields.Add(string.Empty); //(record.COMPOUNDINGREDIENTCOSTDET);
            fields.Add(string.Empty); //(record.COMPOUNDINGREDIENTMODIFIER);
            fields.Add(tpcRecord.CompoundNDCDispensed1);
            fields.Add(tpcRecord.CompoundNDCDispensed2);
            fields.Add(tpcRecord.CompoundNDCDispensed3);
            fields.Add(tpcRecord.CompoundNDCDispensed4);
            fields.Add(tpcRecord.CompoundNDCDispensed5);
            fields.Add(tpcRecord.CompoundNDCDispensed6);
            fields.Add(string.Empty); //(record.COMPOUNDPRODUCTIDQUALIFIER);
            fields.Add(string.Empty); //(record.COMPOUNDTYPE);
            fields.Add(string.Empty); //(record.CPTLEVEL1);
            fields.Add(string.Empty); //(record.CPTLEVEL2);
            fields.Add(string.Empty); //(record.CPTLEVEL3);
            fields.Add((tpcRecord.DecimalPackSize ?? 0M).ToString("0.00"));
            fields.Add(tpcRecord.DiagnosisCode1);
            fields.Add(string.Empty); //(record.DIAGNOSISCODE2);
            fields.Add(string.Empty); //(record.DIAGNOSISCODE3);
            fields.Add(string.Empty); //(record.DIAGNOSISCODE4);
            fields.Add(string.Empty); //(record.DIAGNOSISCODE5);
            fields.Add(string.Empty); //(record.DIAGNOSISCODE6);
            fields.Add(tpcRecord.DiagnosisCodeCount.ToString());
            fields.Add(tpcRecord.DiagnosisCodeQualifier1);
            fields.Add(string.Empty); //(record.DIAGNOSISCODEQUALIFIER2);
            fields.Add(string.Empty); //(record.DIAGNOSISCODEQUALIFIER3);
            fields.Add(string.Empty); //(record.DIAGNOSISCODEQUALIFIER4);
            fields.Add(string.Empty); //(record.DIAGNOSISCODEQUALIFIER5);
            fields.Add(string.Empty); //(record.DIAGNOSISCODEQUALIFIER6);
            fields.Add(tpcRecord.DispensedDrugName);
            fields.Add(string.Empty); //(record.DRUGCLASS);
            fields.Add(string.Empty); //(record.DRUGGROUP);
            fields.Add(string.Empty); //(record.DRUGSOURCECODE);
            fields.Add(string.Empty); //(record.GENERICEQUIVALENTPRODUCTID);
            fields.Add(string.Empty); //(record.GENERICEQUIVALENTPRODIDQUAL);
            fields.Add(tpcRecord.GenericCodeNumber);
            fields.Add(string.Empty); //(record.HCPCSCODE1);
            fields.Add(string.Empty); //(record.HCPCSCODE2);
            fields.Add(string.Empty); //(record.HCPCSCODE3);
            fields.Add(string.Empty); //(record.HCPCSCODE4);
            fields.Add(string.Empty); //(record.HCPCSCODE5);
            fields.Add(string.Empty); //(record.HCPCSCODE6);
            fields.Add(tpcRecord.IsClientGeneric);
            fields.Add(tpcRecord.IsEmergencyFill);
            fields.Add(tpcRecord.IsMaintenanceDrug);
            fields.Add(tpcRecord.IsPartialFill);
            fields.Add(tpcRecord.IsPreferredDrug);
            fields.Add(string.Empty); //(record.ISREPACKAGE);
            fields.Add(string.Empty); //(record.MEDICALCERTIFICATIONNUMBER);
            fields.Add((tpcRecord.MetricQuantityDispensed ?? 0M).ToString("0.00"));
            fields.Add(string.Empty); //(record.ORIGINALLYPRESCRIBEDPRODQUAL);
            fields.Add(string.Empty); //(record.ORIGINALLYPRESCRIBEDQUANTITY);
            fields.Add((tpcRecord.PackSize ?? 0M).ToString("0.00"));
            fields.Add(string.Empty); //(record.PREFERREDDRUGCODE);
            fields.Add(string.Empty); //(record.PREFERREDDRUGQUALIFIER);
            fields.Add(string.Empty); //(record.PROCEDUREMODIFIERCODE1);
            fields.Add(string.Empty); //(record.PROCEDUREMODIFIERCODE2);
            fields.Add(string.Empty); //(record.PROCEDUREMODIFIERCODE3);
            fields.Add(string.Empty); //(record.PROCEDUREMODIFIERCODE4);
            fields.Add(string.Empty); //(record.PROCEDUREMODIFIERCODE5);
            fields.Add(string.Empty); //(record.PROCEDUREMODIFIERCODE6);
            fields.Add(tpcRecord.ProcedureModifierCodeCount.ToString());
            fields.Add(string.Empty); //(record.PRODUCTCODEQUALIFIER);
            fields.Add(string.Empty); //(record.PRODUCTFORMULARYSTATUSCODE);
            fields.Add((tpcRecord.QuantityWritten ?? 0M).ToString("0.00"));
            fields.Add(string.Empty); //(record.REASONFORSERVICECODE);
            fields.Add(string.Empty); //(record.REGION);
            fields.Add(string.Empty); //(record.RESULTOFSERVICECODE);
            fields.Add(tpcRecord.Schedule);
            fields.Add(string.Empty); //(record.SOURCEPRODUCTIDQUALIFIER);
            fields.Add(string.Empty); //(record.STEPDRUGPRODUCTIDQUALIFIER);
            fields.Add(tpcRecord.Strength);
            fields.Add(string.Empty); //(record.SUBMISSIONCLARIFICATIONCODE);
            fields.Add(tpcRecord.SubmittedDrugName);
            fields.Add(tpcRecord.SubmittedDrugNDC);
            fields.Add(tpcRecord.Unit);
            fields.Add(string.Empty); //(record.UNITDOSEINDICATOR);
            fields.Add(string.Empty); //(record.UPC1);
            fields.Add(string.Empty); //(record.UPC2);
            fields.Add(string.Empty); //(record.UPC3);
            fields.Add(string.Empty); //(record.UPC4);
            fields.Add(string.Empty); //(record.UPC5);
            fields.Add(string.Empty); //(record.UPC6);
            fields.Add(string.Empty); //(record.EMPLOYERADDRESS1);
            fields.Add(string.Empty); //(record.EMPLOYERADDRESS2);
            fields.Add(string.Empty); //(record.EMPLOYERCITY);
            fields.Add(string.Empty); //(record.EMPLOYERCOUNTRYCODE);
            fields.Add(string.Empty); //(record.EMPLOYERFEDERALTAXID);
            fields.Add(string.Empty); //(record.EMPLOYERNAME);
            fields.Add(string.Empty); //(record.EMPLOYEROTHERPAYEREMPLOYERNAME);
            fields.Add(string.Empty); //(record.EMPLOYERPHONENUMBER);
            fields.Add(string.Empty); //(record.EMPLOYERPOSTALCODE);
            fields.Add(string.Empty); //(record.EMPLOYERSTATE);
            fields.Add(string.Empty); //(record.BILLINGFORMAT);
            fields.Add(string.Empty); //(record.BILLINGTEXT);
            fields.Add(string.Empty); //(record.CURRENTBILLEDDATE);
            fields.Add(string.Empty); //(record.INITIALBILLEDDATE);
            fields.Add(tpcRecord.IsManualBill);
            fields.Add(string.Empty); //(record.ISPATIENTBILL);
            fields.Add(string.Empty); //(record.MANUALBILLINGTYPE);
            fields.Add(string.Empty); //(record.NUMBEROFCOPIESTOPRINT);
            fields.Add(tpcRecord.AcquisitionCost.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? (tpcRecord.AdjudicatedAmount_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.AdjudicatedAmount_2 ?? 0M).ToString("0.00") : (tpcRecord.AdjudicatedAmount_3 ?? 0M).ToString("0.00")); //AdjudicatedAmount
            fields.Add(tpcRecord.AWPCost.ToString("0.00"));
            fields.Add(string.Empty); //(record.BALANCEDUE);
            fields.Add(string.Empty); //(record.BASISOFCALCULATIONFLATSALESTAX);
            fields.Add(string.Empty); //(record.BASISOFCALCULATIONPERCSALESTAX);
            fields.Add((string.IsNullOrEmpty(tpcRecord.BasisOfReimbursement) ? string.Empty : tpcRecord.BasisOfReimbursement).PadLeft(2, '0'));
            fields.Add(string.Empty); //(record.CALCULATEDACQUISITIONCOST1);
            fields.Add(string.Empty); //(record.CALCULATEDACQUISITIONCOST2);
            fields.Add(string.Empty); //(record.CALCULATEDACQUISITIONCOST3);
            fields.Add(string.Empty); //(record.CALCULATEDACQUISITIONCOST4);
            fields.Add(string.Empty); //(record.CALCULATEDACQUISITIONCOST5);
            fields.Add(string.Empty); //(record.CALCULATEDACQUISITIONCOST6);
            fields.Add(string.Empty); //(record.COMPOUNDAWPCOST1);
            fields.Add(string.Empty); //(record.COMPOUNDAWPCOST2);
            fields.Add(string.Empty); //(record.COMPOUNDAWPCOST3);
            fields.Add(string.Empty); //(record.COMPOUNDAWPCOST4);
            fields.Add(string.Empty); //(record.COMPOUNDAWPCOST5);
            fields.Add(string.Empty); //(record.COMPOUNDAWPCOST6);
            fields.Add(tpcRecord.CompoundIngredientDrugCost1.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost2.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost3.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost4.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost5.ToString("0.00"));
            fields.Add(tpcRecord.CompoundIngredientDrugCost6.ToString("0.00"));
            fields.Add((insurancePayer == 1) ? (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.CopayAmount_2 ?? 0M).ToString("0.00") : (tpcRecord.CopayAmount_3 ?? 0M).ToString("0.00")); //CopayAmount
            fields.Add(string.Empty); //(record.COPAYPAID);
            fields.Add(string.Empty); //(record.COSTBASISRESPONSE);
            fields.Add(string.Empty); //(record.DEDUCTIONAMOUNT);
            fields.Add(string.Empty); //(record.DISCOUNTAMOUNT);
            fields.Add((insurancePayer == 1) ? (tpcRecord.DispensingFeePaid_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.DispensingFeePaid_2 ?? 0M).ToString("0.00") : (tpcRecord.DispensingFeePaid_3 ?? 0M).ToString("0.00")); //DispensingFeePaid
            fields.Add(string.Empty); //(record.DISPENSINGFEEREIMBURSABLEAMT);
            fields.Add(string.Empty); //(record.DISPENSINGFEESUBMITTED);
            fields.Add(string.Empty); //(record.FLATSALESTAXPAID);
            fields.Add(string.Empty); //(record.FLATSALESTAXSUBMITTED);
            fields.Add(string.Empty); //(record.GROSSAMOUNTDUE);
            fields.Add(tpcRecord.IncentiveAmountPaid.ToString("0.00"));
            fields.Add(string.Empty); //(record.INCENTIVEAMOUNTSUBMITTED);
            fields.Add((insurancePayer == 1) ? (tpcRecord.IngredientCostPaid_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.IngredientCostPaid_2 ?? 0M).ToString("0.00") : (tpcRecord.IngredientCostPaid_3 ?? 0M).ToString("0.00")); //IngredientCostPaid
            fields.Add(string.Empty); //(record.INGREDIENTCOSTREIMBURSABLEAMT);
            fields.Add(tpcRecord.IngredientCostSubmitted.ToString("0.00"));
            fields.Add(string.Empty); //(record.MAXIMUMALLOWABLECOST);
            fields.Add((insurancePayer == 1) ? (0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00") : (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00")); //OtherAmountClaimSubmitted
            fields.Add((insurancePayer == 1) ? (0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.AdjudicatedAmount_1 ?? 0M).ToString("0.00") : (tpcRecord.AdjudicatedAmount_1 ?? 0M).ToString("0.00")); //OtherPayerAmountPaid
            fields.Add((insurancePayer == 1) ? (0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00") : (tpcRecord.CopayAmount_1 ?? 0M).ToString("0.00")); //OtherPPRespAmount
            fields.Add(string.Empty); //(record.OTHERPPRESPAMOUNTCOUNT);
            fields.Add(string.Empty); //(record.OTHERPPRESPAMOUNTQUALIFIER);
            fields.Add(string.Empty); //(record.PATIENTPAIDAMOUNTSUBMITTED);
            fields.Add((insurancePayer == 1) ? (tpcRecord.PatientPayAmount_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.PatientPayAmount_2 ?? 0M).ToString("0.00") : (tpcRecord.PatientPayAmount_3 ?? 0M).ToString("0.00")); //PatientPayAmount
            fields.Add(string.Empty); //(record.PATIENTSALESTAXAMOUNT);
            fields.Add(string.Empty); //(record.PERCENTSALESTAXPAID);
            fields.Add(string.Empty); //(record.PERCENTSALESTAXRATEPAID);
            fields.Add(string.Empty); //(record.PERCENTSALESTAXRATESUBMITTED);
            fields.Add(string.Empty); //(record.PERCENTSALESTAXSUBMITTED);
            fields.Add(string.Empty); //(record.PROFESSIONALSERVICEFEEPAID);
            fields.Add(string.Empty); //(record.PROFESSIONALSERVICEFEESUBD);
            fields.Add(string.Empty); //(record.PROMOTIONALAMOUNT);
            fields.Add(string.Empty); //(record.SALESTAXBASISPAID);
            fields.Add(string.Empty); //(record.SALESTAXBASISSUBMITTED);
            fields.Add(string.Empty); //(record.SECONDARYAMOUNTPAID);
            fields.Add(tpcRecord.TransactionPrice.ToString("0.00"));
            fields.Add(tpcRecord.UsualAndCustomary.ToString("0.00"));
            fields.Add(string.Empty); //(record.AIDTODEPENDENTCHILD);
            fields.Add(string.Empty); //(record.ALLERGYOVERRIDE);
            fields.Add(string.Empty); //(record.CARDCOVERAGEEFFECTIVEDATE);
            fields.Add(string.Empty); //(record.CLAIMSOURCENAME);
            fields.Add(string.Empty); //(record.CLAIMTYPE);
            fields.Add(string.Empty); //(record.FAMILYSERIESCODE);
            fields.Add(string.Empty); //(record.HCFA10D);
            fields.Add(string.Empty); //(record.HCFA19);
            fields.Add(string.Empty); //(record.HOLDTRANSACTIONFLAG);
            fields.Add(string.Empty); //(record.INTERACTIONOVERRIDE);
            fields.Add(string.Empty); //(record.ISAUTOFILLELIGIBLE);
            fields.Add(string.Empty); //(record.ISDISEASEOVERRIDE);
            fields.Add(string.Empty); //(record.ISDUPLICATETHERAPYOVERRIDE);
            fields.Add(string.Empty); //(record.ISRESUBMISSION);
            fields.Add(string.Empty); //(record.NUMBEROFTIMESCLAIMREJECTED);
            fields.Add(string.Empty); //(record.NUMBEROFTIMESCLAIMSUBMITTED);
            fields.Add(string.Empty); //(record.OTHERPAYERREJECTIONCODE);
            fields.Add(string.Empty); //(record.PATIENTCLINICNUMBER);
            fields.Add(string.Empty); //(record.PRICECODE);
            fields.Add(string.Empty); //(record.PRICEGROUP);
            fields.Add(string.Empty); //(record.PRICEOVERRIDE);
            fields.Add(string.Empty); //(record.PRICEREGION);
            fields.Add(string.Empty); //(record.PRICEZONE);
            fields.Add(string.Empty); //(record.REBILLPROCESSINGFEE);
            fields.Add(string.Empty); //(record.REJECTIONCODE);
            fields.Add(string.Empty); //(record.TARFLAG);
            fields.Add((insurancePayer == 1) ? tpcRecord.ThirdPartySubmitType_1 : (insurancePayer == 2) ?
                tpcRecord.ThirdPartySubmitType_2 : tpcRecord.ThirdPartySubmitType_3); //ThirdPartySubmitType
            fields.Add(string.Empty); //(record.THIRDPARTYTRANSACTIONBATCHNBR);
            fields.Add(string.Empty); //(record.TOLERANCEEXCEEDAMOUNT);
            fields.Add(string.Empty); //(record.TOLERANCEEXCEEDPERCENTAGE);
            fields.Add(string.Empty); //(record.TRANSACTIONCHARGEDTOAR);
            fields.Add(string.Empty); //(record.TRANSACTIONMASTERENTEREDDATE);
            fields.Add(string.Empty); //(record.WELFARECUSTOMERLOCATION);
            fields.Add(string.Empty); //(record.WHOLESALEORDIRECTPURCHASE);
            fields.Add(string.Empty); //(record.WORKERSCOMPENSATIONCARRIERID);
            fields.Add(tpcRecord.PatientAccountNumber.ToString());
            fields.Add(hipaaRecord.PatientAddress1);
            fields.Add(hipaaRecord.PatientAddress2);
            fields.Add(hipaaRecord.PatientCity);
            fields.Add(string.Empty); //(record.PATIENTCOUNTRYCODE);
            fields.Add(hipaaRecord.PatientDateOfBirth.HasValue == true ? hipaaRecord.PatientDateOfBirth.Value.ToString("yyyyMMdd") : string.Empty);
            fields.Add(string.Empty); //(record.PATIENTDEPENDENTNUMBER);
            fields.Add(string.Empty); //(record.PATIENTELIGIBILITYCLARIFNCODE);
            fields.Add(string.Empty); //(record.PATIENTEMAIL);
            fields.Add(hipaaRecord.PatientFirstName);
            fields.Add(string.Empty); //(record.PATIENTFREQUENTSHOPPERID);
            fields.Add(tpcRecord.PatientGender);
            fields.Add(string.Empty); //(record.PATIENTISEMPLOYED);
            fields.Add(string.Empty); //(record.PATIENTISPREGNANT);
            fields.Add(string.Empty); //(record.PATIENTISSENIORCITIZEN);
            fields.Add(hipaaRecord.PatientLastName);
            fields.Add(string.Empty); //(record.PATIENTMARTIALSTATUS);
            fields.Add(string.Empty); //(record.PATIENTMEDICALRECORDNUMBER);
            fields.Add(tpcRecord.PatientMedicarePartDCoverage);
            fields.Add(tpcRecord.PatientMedigapId);
            fields.Add(hipaaRecord.PatientMiddleInitial);
            fields.Add(string.Empty); //(record.PATIENTOTHERPAYERCARDHOLDERID);
            fields.Add(string.Empty); //(record.PATIENTOTHERPAYERDATEOFBIRTH);
            fields.Add(string.Empty); //(record.PATIENTCODE);
            fields.Add(string.Empty); //(record.PATIENTID);
            fields.Add(string.Empty); //(record.PATIENTIDQUALIFIER);
            fields.Add(string.Empty); //(record.PATIENTPERSONCODE);
            fields.Add(hipaaRecord.PatientPhoneNumber);
            fields.Add(tpcRecord.PatientPostalCode);
            fields.Add((insurancePayer == 1) ? (tpcRecord.PatientRelationshipCode_1 ?? 0M).ToString("0.00") : (insurancePayer == 2) ?
                (tpcRecord.PatientRelationshipCode_2 ?? 0M).ToString("0.00") : (tpcRecord.PatientRelationshipCode_3 ?? 0M).ToString("0.00")); //PatientRelationshipCode
            fields.Add(string.Empty); //(record.PATIENTSOCIALSECURITYNUMBER);
            fields.Add(tpcRecord.PatientState);
            fields.Add(string.Empty); //(record.PATIENTSTUDENTTYPE);
            fields.Add((insurancePayer == 1) ? tpcRecord.BIN_1 : (insurancePayer == 2) ?
                tpcRecord.BIN_2 : tpcRecord.BIN_3); //BIN
            fields.Add(string.Empty); //(record.INTERNALCONTROLNUMBER);
            fields.Add(string.Empty); //(record.N102);
            fields.Add(string.Empty); //(record.N103);
            fields.Add(string.Empty); //(record.N104);
            fields.Add(string.Empty); //(record.N301);
            fields.Add(string.Empty); //(record.N302);
            fields.Add(string.Empty); //(record.N401);
            fields.Add(string.Empty); //(record.N402);
            fields.Add(string.Empty); //(record.N403);
            fields.Add(string.Empty); //(record.N404);
            fields.Add(string.Empty); //(record.N407);
            fields.Add((insurancePayer == 1) ? tpcRecord.OtherCoverageCode_1 : (insurancePayer == 2) ?
                tpcRecord.OtherCoverageCode_2 : tpcRecord.OtherCoverageCode_3); //OtherCoverageCode
            fields.Add((insurancePayer == 1) ? tpcRecord.PayerId_2 : (insurancePayer == 2) ?
                tpcRecord.PayerId_3 : string.Empty); //OtherPayerId
            fields.Add(string.Empty); //(record.OTHERPAYERIDQUALIFIER);
            fields.Add((insurancePayer == 1) ? tpcRecord.PayerId_1 : (insurancePayer == 2) ?
                tpcRecord.PayerId_2 : tpcRecord.PayerId_3); //PayerId
            fields.Add(string.Empty); //(record.PAYERTYPE);
            fields.Add((insurancePayer == 1) ? tpcRecord.ProcessorControlNumber_1 : (insurancePayer == 2) ?
                tpcRecord.ProcessorControlNumber_2 : tpcRecord.ProcessorControlNumber_3); //ProcessorControlNumber
            fields.Add((insurancePayer == 1) ? tpcRecord.BIN_2 : (insurancePayer == 2) ?
                tpcRecord.BIN_3 : string.Empty); //SecondaryBIN
            fields.Add(string.Empty); //SecondaryId
            fields.Add(tpcRecord.OriginatingStoreId);
            fields.Add(string.Empty); //(record.PHARMACISTINITIALS);
            fields.Add(string.Empty); //(record.PHARMACYTYPE);
            fields.Add(string.Empty); //(record.PROVIDERACCEPTASSIGNMENT);
            fields.Add(tpcRecord.ProviderId);
            fields.Add(tpcRecord.ProviderIdQualifier);
            fields.Add(string.Empty); //(record.STOREFAXNUMBER);
            fields.Add(string.Empty); //(record.STOREVERSIONNUMBER);
            fields.Add(string.Empty); //(record.AGECOVERAGEEXPIRES);
            fields.Add((insurancePayer == 1) ? tpcRecord.GroupInsuranceNumber_1 : (insurancePayer == 2) ?
                tpcRecord.GroupInsuranceNumber_2 : tpcRecord.GroupInsuranceNumber_3); //GroupInsuranceNumber
            fields.Add(string.Empty); //(record.HOMEPLAN);
            fields.Add(string.Empty); //(record.HOSTINSURANCEPLAN);
            fields.Add((insurancePayer == 1) ? tpcRecord.InsurancePlan_1 : (insurancePayer == 2) ?
                tpcRecord.InsurancePlan_2 : tpcRecord.InsurancePlan_3); //InsurancePlan
            fields.Add(string.Empty); //(record.OTHERPAYERGROUPID);
            fields.Add(string.Empty); //(record.OTHERPAYERPLANNAME);
            fields.Add(string.Empty); //(record.PATIENTGROUPNAME);
            fields.Add(string.Empty); //(record.PAYERINSURANCEPLAN);
            fields.Add(string.Empty); //(record.PHARMACYPLANNAME);
            fields.Add(string.Empty); //(record.ACCIDENTTYPE);
            fields.Add((insurancePayer == 1) ? tpcRecord.AuthorizationNumber_1 : (insurancePayer == 2) ?
                tpcRecord.AuthorizationNumber_2 : tpcRecord.AuthorizationNumber_3); //AuthorizationNumber
            fields.Add(tpcRecord.CompoundDispensedQuantity1.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity2.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity3.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity4.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity5.ToString());
            fields.Add(tpcRecord.CompoundDispensedQuantity6.ToString());
            fields.Add(tpcRecord.DAW);
            fields.Add(tpcRecord.DaysSupply.ToString());
            fields.Add(string.Empty); //(record.DIAGNOSISPOINTER1);
            fields.Add(string.Empty); //(record.DIRECTIONSFORUSE);
            fields.Add((tpcRecord.DispensedQuantity ?? 0M).ToString("0.00"));
            fields.Add(string.Empty); //(record.DOSESPERDAY);
            fields.Add(tpcRecord.FromFillDate.HasValue == true ? tpcRecord.FromFillDate.Value.ToString("yyyyMMdd") : string.Empty);
            fields.Add(string.Empty); //(record.HEADERRESPONSESTATUS);
            fields.Add(string.Empty); //(record.HOSPITALIZATIONENDDATE);
            fields.Add(string.Empty); //(record.HOSPITALIZATIONSTARTDATE);
            fields.Add(tpcRecord.HostTransactionNumber.ToString());
            fields.Add((insurancePayer == 1) ? tpcRecord.HowRecordSubmitted_1 : (insurancePayer == 2) ?
                tpcRecord.HowRecordSubmitted_2 : tpcRecord.HowRecordSubmitted_3); //HowRecordSubmitted
            fields.Add(tpcRecord.InitialFillDate.HasValue == true ? tpcRecord.InitialFillDate.Value.ToString("yyyyMMdd") : string.Empty);
            fields.Add(string.Empty); //(record.INJURYDATE);
            fields.Add(tpcRecord.IsCompoundCode);
            fields.Add(string.Empty); //(record.ISCOPAYASSIST);
            fields.Add(string.Empty); //(record.ISCOPAYCARD);
            fields.Add(string.Empty); //(record.ISDUROVERRIDE);
            fields.Add(string.Empty); //(record.ISMAC);
            fields.Add(string.Empty); //(record.ISOTHERINSURANCE);
            fields.Add(string.Empty); //(record.ISOUTSIDELAB);
            fields.Add(tpcRecord.IsThirdParty);
            fields.Add(string.Empty); //(record.LABFEES);
            fields.Add(string.Empty); //(record.LEVELOFSERVICE);
            fields.Add(string.Empty); //(record.MEDICAIDORIGINALREFERENCENBR);
            fields.Add(string.Empty); //(record.MEDICAIDRESUBMISSIONCODE);
            fields.Add(string.Empty); //(record.MEDICATIONPOSSESIONRATIO);
            fields.Add(tpcRecord.NumberOfRefillsAuthorized.ToString());
            fields.Add(tpcRecord.NumberOfRefillsRemaining.ToString());
            fields.Add(string.Empty); //(record.ORIGINALDATEOFSAMEILLNESS);
            fields.Add(string.Empty); //(record.ORIGINALLYPRESCRIBEDNDC);
            fields.Add(string.Empty); //(record.ORIGINALSUBMISSIONDATE);
            fields.Add(string.Empty); //(record.OTHERPAYERDATE);
            fields.Add(string.Empty); //(record.OUTOFWORKENDDATE);
            fields.Add(string.Empty); //(record.OUTOFWORKSTARTDATE);
            fields.Add(string.Empty); //(record.PHARMACISTID);
            fields.Add(string.Empty); //(record.PRESCRIPTIONDENIALCLARIFN);
            fields.Add(tpcRecord.PrescriptionNumber);
            fields.Add(string.Empty); //(record.PRIORAUTHORIZATIONNUMBER);
            fields.Add(string.Empty); //(record.PRIORAUTHORIZATIONTYPECODE);
            fields.Add(string.Empty); //(record.PROFESSIONALSERVICECODE);
            fields.Add(string.Empty); //(record.PROPORTIONOFDAYSCOVERED);
            fields.Add(tpcRecord.RefillNumber.ToString());
            fields.Add(string.Empty); //(record.SOLDDATE);
            fields.Add((insurancePayer == 1) ? tpcRecord.ERxSplitFillNumber_1 : (insurancePayer == 2) ?
                tpcRecord.ERxSplitFillNumber_2 : tpcRecord.ERxSplitFillNumber_3); //SplitFillNumber
            fields.Add(string.Empty); //(record.SUBMISSIONOFPAYMENTDATE);
            fields.Add(string.Empty); //(record.TOFILLDATE);
            fields.Add(tpcRecord.TransactionCode);
            fields.Add(string.Empty); //(record.TRANSACTIONRESPONSESTATUS);
            fields.Add(string.Empty); //(record.TRANSACTIONTYPECODE);
            fields.Add(tpcRecord.TransmissionDate.HasValue == true ? tpcRecord.TransmissionDate.Value.ToString("yyyyMMdd") : string.Empty);
            fields.Add(string.Empty); //(record.TRANSMISSIONTIME);
            fields.Add(string.Empty); //(record.TREATMENTAUTHORIZATIONREQUEST);
            fields.Add(string.Empty); //(record.UNITSPERDAY);
            fields.Add(string.Empty); //(record.UNITSPERDOSE);
            fields.Add(string.Empty); //(record.WORKERSCOMPENSATIONCLAIMID);
            fields.Add(tpcRecord.WrittenDate.HasValue == true ? tpcRecord.WrittenDate.Value.ToString("yyyyMMdd") : string.Empty);
            fields.Add(string.Empty); //(record.FORMSIGNEDDATE);
            fields.Add(string.Empty); //(record.MEDICAIDAGENCYID);
            fields.Add(string.Empty); //(record.MEDICAIDID);
            fields.Add(tpcRecord.OriginCode);
            fields.Add(string.Empty); //(record.PLACEOFSERVICE);
            fields.Add(hipaaRecord.PrescriberAddress1);
            fields.Add(hipaaRecord.PrescriberAddress2);
            fields.Add(string.Empty); //(record.PRESCRIBERALTERNATEID);
            fields.Add(hipaaRecord.PrescriberCity);
            fields.Add(string.Empty); //(record.PRESCRIBERCOUNTRYCODE);
            fields.Add(hipaaRecord.PrescriberDEA);
            fields.Add(string.Empty); //(record.PRESCRIBEREMAIL);
            fields.Add(string.Empty); //(record.PRESCRIBERFAXNUMBER);
            fields.Add(hipaaRecord.PrescriberFirstName);
            fields.Add(string.Empty); //(record.PRESCRIBERFULLNAME);
            fields.Add((hipaaRecord.PrescriberId ?? 0M).ToString("0"));
            fields.Add(hipaaRecord.PrescriberIdQualifier);
            fields.Add(hipaaRecord.PrescriberLastName);
            fields.Add(string.Empty); //(record.PRESCRIBERLICENSENUMBER);
            fields.Add(tpcRecord.PrescriberNPI);
            fields.Add(string.Empty); //(record.PRESCRIBERPHONENUMBER);
            fields.Add(tpcRecord.PrescriberPostalCode);
            fields.Add(string.Empty); //(record.PRESCRIBERSPECIALTYCODE);
            fields.Add(tpcRecord.PrescriberState);
            fields.Add(string.Empty); //(record.PRESCRIBERSTATEASSIGNEDIDNBR);
            fields.Add(string.Empty); //(record.PRESCRIBERTYPE);
            fields.Add(string.Empty); //(record.PRIMECAREPROVIDERFIRSTNAME);
            fields.Add(string.Empty); //(record.PRIMECAREPROVIDERID);
            fields.Add(string.Empty); //(record.PRIMECAREPROVIDERIDQUALIFIER);
            fields.Add(string.Empty); //(record.PRIMECAREPROVIDERLASTNAME);
            fields.Add(string.Empty); //(record.REFERRINGPRESCRIBERNAME);
            fields.Add(string.Empty); //(record.REFERRINGPRESCRIBERNPI);
            fields.Add(string.Empty); //(record.REFERRINGPRESCRIBERUPIN);
            fields.Add(string.Empty); //(record.SERVICEPROVIDERID);
            fields.Add(string.Empty); //(record.SERVICEPROVIDERIDQUALIFIER);

            fields.Add(string.Empty); //(record.PreferredProductCount);
            fields.Add(string.Empty); //(record.PreferredProductDescription);
            fields.Add(string.Empty); //(record.AccumulatedDeductibleAmount);
            fields.Add(string.Empty); //(record.AmountAppliedToPeriodicDeductible);
            fields.Add(string.Empty); //(record.AmountAttributedToCoverageGap);
            fields.Add(string.Empty); //(record.AmountAttributedToProcessorFee);
            fields.Add(string.Empty); //(record.AmountAttributedToProductSelectionBrandDrug);
            fields.Add(string.Empty); //(record.AmountAttributedToProductSelectionBrandNonPreferredFormularySelection);
            fields.Add(string.Empty); //(record.AmountAttributedToProductSelectionNonPreferredFormularySelection);
            fields.Add(string.Empty); //(record.AmountAttributedToProviderNetworkSelection);
            fields.Add(string.Empty); //(record.AmountAttributedToSalesTax);
            fields.Add(string.Empty); //(record.AmountExceedingPeriodicBenefitMaximum);
            fields.Add(string.Empty); //(record.AmountOfCoinsurance);
            fields.Add(string.Empty); //(record.BasisOfCalculationCoinsurance);
            fields.Add(string.Empty); //(record.BasisOfCalculationCoPay);
            fields.Add(string.Empty); //(record.BasisOfCalculationDispensingFee);
            fields.Add(string.Empty); //(record.BenefitStageAmount);
            fields.Add(string.Empty); //(record.EstimatedGenericSavings);
            fields.Add(string.Empty); //(record.OtherPayerAmountPaidQualifier);
            fields.Add(string.Empty); //(record.OtherPayerAmountRecognized);
            fields.Add(string.Empty); //(record.PlanSalesTaxAmount);
            fields.Add(string.Empty); //(record.PreferredProductCostShareIncentive);
            fields.Add(string.Empty); //(record.PreferredProductIncentive);
            fields.Add(string.Empty); //(record.RemainingBenefitAmount);
            fields.Add(string.Empty); //(record.RemainingDeductibleAmount);
            fields.Add(string.Empty); //(record.TaxExemptIndicator);
            fields.Add(string.Empty); //(record.AdditionalDocumentationTypeID);
            fields.Add(string.Empty); //(record.AdditionalMessageInformation);
            fields.Add(string.Empty); //(record.AdditionalMessageInformationContinuity);
            fields.Add(string.Empty); //(record.AdditionalMessageInformationCount);
            fields.Add(string.Empty); //(record.AdditionalMessageInformationQualifier);
            fields.Add(string.Empty); //(record.ApprovedMessageCode);
            fields.Add(string.Empty); //(record.ApprovedMessageCodeCount);
            fields.Add(string.Empty); //(record.BenefitStageCount);
            fields.Add(string.Empty); //(record.BenefitStageQualifier);
            fields.Add(string.Empty); //(record.ClinicalInformationCounter);
            fields.Add(string.Empty); //(record.HealthPlanFundedAssistanceAmount);
            fields.Add(string.Empty); //(record.MeasurementDate);
            fields.Add(string.Empty); //(record.MeasurementDimension);
            fields.Add(string.Empty); //(record.MeasurementTime);
            fields.Add(string.Empty); //(record.MeasurementUnit);
            fields.Add(string.Empty); //(record.MeasurementValue);
            fields.Add(string.Empty); //(record.Message);
            fields.Add(string.Empty); //(record.QuestionAlphanumericResponse);
            fields.Add(string.Empty); //(record.QuestionDateResponse);
            fields.Add(string.Empty); //(record.QuestionDollarAmountResponse);
            fields.Add(string.Empty); //(record.QuestionNumberLetter);
            fields.Add(string.Empty); //(record.QuestionNumberLetterCount);
            fields.Add(string.Empty); //(record.QuestionNumericResponse);
            fields.Add(string.Empty); //(record.QuestionPercentResponse);
            fields.Add(string.Empty); //(record.RequestPeriodBeginDate);
            fields.Add(string.Empty); //(record.RequestPeriodRecertRevisedDate);
            fields.Add(string.Empty); //(record.RequestStatus);
            fields.Add(string.Empty); //(record.SpendingAccountAmountRemaining);
            fields.Add(string.Empty); //(record.SupportingDocumentation);
            fields.Add(string.Empty); //(record.PatientResidence);
            fields.Add(string.Empty); //(record.OtherPayerCoverageType);
            fields.Add(string.Empty); //(record.OtherPayerIDCount);
            fields.Add(string.Empty); //(record.OtherPayerProcessorControlNumber);
            fields.Add(string.Empty); //(record.PayerIDQualifier);
            fields.Add(string.Empty); //(record.ExtendedHostTransactionNumber);
            fields.Add(string.Empty); //(record.LengthOfNeed);
            fields.Add(string.Empty); //(record.LengthOfNeedQualifier);
            fields.Add(string.Empty); //(record.PharmacyServiceType);
            fields.Add(string.Empty); //(record.SwitchTimeDateStamp);
            fields.Add(string.Empty); //(record.ExtendedPrescriberEmail);
            fields.Add(string.Empty); //(record.Tags);
            fields.Add(string.Empty); //(record.ExtendedPriorAuthorizationNumber);
            fields.Add(string.Empty); //(record.PatientFullName);
            fields.Add(string.Empty); //(record.PrimecareProviderFullName);
            fields.Add(string.Empty); //(record.CardholderFullName);
            fields.Add(string.Empty); //(record.OtherPayerFullName);
            fields.Add(string.Empty); //(record.eVoucher);
            fields.Add(string.Empty); //(record.CentralFillClaim);

            return string.Join(fileDelimiter, fields.ToArray());
        }

        private string CreateHeaderRecord(int recordCount, decimal copayTotal, decimal adjudicatedTotal, DateTime runDate)
        {
            bool inmarHeader = true;

            if (inmarHeader == true)
            {
                return
                    string.Format(
                            "{1}{0}{2:0}{0}{3:0.00}{0}{4:0.00}{0}{5:yyyyMMdd}{0}{6}",
                            fileDelimiter,
                            headerRecord,
                            recordCount,
                            copayTotal,
                            adjudicatedTotal,
                            runDate,
                            cientIdentifier
                        );
            }
            else
            {
                var header = new Collection<string>();

                header.Add("RECORDTYPE");
                header.Add("CARDHOLDERADDRESS1");
                header.Add("CARDHOLDERADDRESS2");
                header.Add("CARDHOLDERID");
                header.Add("CARDHOLDERCITY");
                header.Add("CARDHOLDERCOUNTRYCODE");
                header.Add("CARDHOLDERDATEOFBIRTH");
                header.Add("CARDHOLDERFIRSTNAME");
                header.Add("CARDHOLDERGENDER");
                header.Add("CARDHOLDERLASTNAME");
                header.Add("CARDHOLDERMIDDLEINITIAL");
                header.Add("OTHERPAYERCARDHOLDERID");
                header.Add("OTHERPAYERDATEOFBIRTH");
                header.Add("OTHERPAYERFIRSTNAME");
                header.Add("OTHERPAYERLASTNAME");
                header.Add("OTHERPAYERMIDDLEINITIAL");
                header.Add("CARDHOLDERPHONENUMBER");
                header.Add("CARDHOLDERPOSTALCODE");
                header.Add("SOCIALSECURITYNUMBER");
                header.Add("CARDHOLDERSTATE");
                header.Add("CONTRACTNUMBER");
                header.Add("FORMULARYID");
                header.Add("NETWORKID");
                header.Add("ALTERNATIVEPRODUCTIDQUALIFIER");
                header.Add("ASSOCPRESSERVICEREFNBR");
                header.Add("ASSOCPRESSERVICEREFNBRCOMMENT");
                header.Add("COMPOUNDDRUGNAME");
                header.Add("COMPOUNDINGREDIENTCOMPONENTCNT");
                header.Add("COMPOUNDINGREDIENTCOSTDET");
                header.Add("COMPOUNDINGREDIENTMODIFIER");
                header.Add("COMPOUNDNDCDISPENSED1");
                header.Add("COMPOUNDNDCDISPENSED2");
                header.Add("COMPOUNDNDCDISPENSED3");
                header.Add("COMPOUNDNDCDISPENSED4");
                header.Add("COMPOUNDNDCDISPENSED5");
                header.Add("COMPOUNDNDCDISPENSED6");
                header.Add("COMPOUNDPRODUCTIDQUALIFIER");
                header.Add("COMPOUNDTYPE");
                header.Add("CPTLEVEL1");
                header.Add("CPTLEVEL2");
                header.Add("CPTLEVEL3");
                header.Add("DECIMALPACKSIZE");
                header.Add("DIAGNOSISCODE1");
                header.Add("DIAGNOSISCODE2");
                header.Add("DIAGNOSISCODE3");
                header.Add("DIAGNOSISCODE4");
                header.Add("DIAGNOSISCODE5");
                header.Add("DIAGNOSISCODE6");
                header.Add("DIAGNOSISCODECOUNT");
                header.Add("DIAGNOSISCODEQUALIFIER1");
                header.Add("DIAGNOSISCODEQUALIFIER2");
                header.Add("DIAGNOSISCODEQUALIFIER3");
                header.Add("DIAGNOSISCODEQUALIFIER4");
                header.Add("DIAGNOSISCODEQUALIFIER5");
                header.Add("DIAGNOSISCODEQUALIFIER6");
                header.Add("DISPENSEDDRUGNAME");
                header.Add("DRUGCLASS");
                header.Add("DRUGGROUP");
                header.Add("DRUGSOURCECODE");
                header.Add("GENERICEQUIVALENTPRODUCTID");
                header.Add("GENERICEQUIVALENTPRODIDQUAL");
                header.Add("GCN");
                header.Add("HCPCSCODE1");
                header.Add("HCPCSCODE2");
                header.Add("HCPCSCODE3");
                header.Add("HCPCSCODE4");
                header.Add("HCPCSCODE5");
                header.Add("HCPCSCODE6");
                header.Add("ISCLIENTGENERIC");
                header.Add("ISEMERGENCYFILL");
                header.Add("ISMAINTENANCEDRUG");
                header.Add("ISPARTIALFILL");
                header.Add("ISPREFERREDDRUG");
                header.Add("ISREPACKAGE");
                header.Add("MEDICALCERTIFICATIONNUMBER");
                header.Add("METRICQUANTITYDISPENSED");
                header.Add("ORIGINALLYPRESCRIBEDPRODQUAL");
                header.Add("ORIGINALLYPRESCRIBEDQUANTITY");
                header.Add("PACKSIZE");
                header.Add("PREFERREDDRUGCODE");
                header.Add("PREFERREDDRUGQUALIFIER");
                header.Add("PROCEDUREMODIFIERCODE1");
                header.Add("PROCEDUREMODIFIERCODE2");
                header.Add("PROCEDUREMODIFIERCODE3");
                header.Add("PROCEDUREMODIFIERCODE4");
                header.Add("PROCEDUREMODIFIERCODE5");
                header.Add("PROCEDUREMODIFIERCODE6");
                header.Add("PROCEDUREMODIFIERCODECOUNT");
                header.Add("PRODUCTCODEQUALIFIER");
                header.Add("PRODUCTFORMULARYSTATUSCODE");
                header.Add("QUANTITYWRITTEN");
                header.Add("REASONFORSERVICECODE");
                header.Add("REGION");
                header.Add("RESULTOFSERVICECODE");
                header.Add("SCHEDULE");
                header.Add("SOURCEPRODUCTIDQUALIFIER");
                header.Add("STEPDRUGPRODUCTIDQUALIFIER");
                header.Add("STRENGTH");
                header.Add("SUBMISSIONCLARIFICATIONCODE");
                header.Add("SUBMITTEDDRUGNAME");
                header.Add("SUBMITTEDDRUGNDC");
                header.Add("UNIT");
                header.Add("UNITDOSEINDICATOR");
                header.Add("UPC1");
                header.Add("UPC2");
                header.Add("UPC3");
                header.Add("UPC4");
                header.Add("UPC5");
                header.Add("UPC6");
                header.Add("EMPLOYERADDRESS1");
                header.Add("EMPLOYERADDRESS2");
                header.Add("EMPLOYERCITY");
                header.Add("EMPLOYERCOUNTRYCODE");
                header.Add("EMPLOYERFEDERALTAXID");
                header.Add("EMPLOYERNAME");
                header.Add("EMPLOYEROTHERPAYEREMPLOYERNAME");
                header.Add("EMPLOYERPHONENUMBER");
                header.Add("EMPLOYERPOSTALCODE");
                header.Add("EMPLOYERSTATE");
                header.Add("BILLINGFORMAT");
                header.Add("BILLINGTEXT");
                header.Add("CURRENTBILLEDDATE");
                header.Add("INITIALBILLEDDATE");
                header.Add("ISMANUALBILL");
                header.Add("ISPATIENTBILL");
                header.Add("MANUALBILLINGTYPE");
                header.Add("NUMBEROFCOPIESTOPRINT");
                header.Add("ACQUISITIONCOST");
                header.Add("ADJUDICATEDAMOUNT");
                header.Add("AWPCOST");
                header.Add("BALANCEDUE");
                header.Add("BASISOFCALCULATIONFLATSALESTAX");
                header.Add("BASISOFCALCULATIONPERCSALESTAX");
                header.Add("BASISOFREIMBURSEMENT");
                header.Add("CALCULATEDACQUISITIONCOST1");
                header.Add("CALCULATEDACQUISITIONCOST2");
                header.Add("CALCULATEDACQUISITIONCOST3");
                header.Add("CALCULATEDACQUISITIONCOST4");
                header.Add("CALCULATEDACQUISITIONCOST5");
                header.Add("CALCULATEDACQUISITIONCOST6");
                header.Add("COMPOUNDAWPCOST1");
                header.Add("COMPOUNDAWPCOST2");
                header.Add("COMPOUNDAWPCOST3");
                header.Add("COMPOUNDAWPCOST4");
                header.Add("COMPOUNDAWPCOST5");
                header.Add("COMPOUNDAWPCOST6");
                header.Add("COMPOUNDINGREDIENTDRUGCOST1");
                header.Add("COMPOUNDINGREDIENTDRUGCOST2");
                header.Add("COMPOUNDINGREDIENTDRUGCOST3");
                header.Add("COMPOUNDINGREDIENTDRUGCOST4");
                header.Add("COMPOUNDINGREDIENTDRUGCOST5");
                header.Add("COMPOUNDINGREDIENTDRUGCOST6");
                header.Add("COPAYAMOUNT");
                header.Add("COPAYPAID");
                header.Add("COSTBASISRESPONSE");
                header.Add("DEDUCTIONAMOUNT");
                header.Add("DISCOUNTAMOUNT");
                header.Add("DISPENSINGFEEPAID");
                header.Add("DISPENSINGFEEREIMBURSABLEAMT");
                header.Add("DISPENSINGFEESUBMITTED");
                header.Add("FLATSALESTAXPAID");
                header.Add("FLATSALESTAXSUBMITTED");
                header.Add("GROSSAMOUNTDUE");
                header.Add("INCENTIVEAMOUNTPAID");
                header.Add("INCENTIVEAMOUNTSUBMITTED");
                header.Add("INGREDIENTCOSTPAID");
                header.Add("INGREDIENTCOSTREIMBURSABLEAMT");
                header.Add("INGREDIENTCOSTSUBMITTED");
                header.Add("MAXIMUMALLOWABLECOST");
                header.Add("OTHERAMOUNTCLAIMSUBMITTED");
                header.Add("OTHERPAYERAMOUNTPAID");
                header.Add("OTHERPPRESPAMOUNT");
                header.Add("OTHERPPRESPAMOUNTCOUNT");
                header.Add("OTHERPPRESPAMOUNTQUALIFIER");
                header.Add("PATIENTPAIDAMOUNTSUBMITTED");
                header.Add("PATIENTPAYAMOUNT");
                header.Add("PATIENTSALESTAXAMOUNT");
                header.Add("PERCENTSALESTAXPAID");
                header.Add("PERCENTSALESTAXRATEPAID");
                header.Add("PERCENTSALESTAXRATESUBMITTED");
                header.Add("PERCENTSALESTAXSUBMITTED");
                header.Add("PROFESSIONALSERVICEFEEPAID");
                header.Add("PROFESSIONALSERVICEFEESUBD");
                header.Add("PROMOTIONALAMOUNT");
                header.Add("SALESTAXBASISPAID");
                header.Add("SALESTAXBASISSUBMITTED");
                header.Add("SECONDARYAMOUNTPAID");
                header.Add("TRANSACTIONPRICE");
                header.Add("USUALANDCUSTOMARY");
                header.Add("AIDTODEPENDENTCHILD");
                header.Add("ALLERGYOVERRIDE");
                header.Add("CARDCOVERAGEEFFECTIVEDATE");
                header.Add("CLAIMSOURCENAME");
                header.Add("CLAIMTYPE");
                header.Add("FAMILYSERIESCODE");
                header.Add("HCFA10D");
                header.Add("HCFA19");
                header.Add("HOLDTRANSACTIONFLAG");
                header.Add("INTERACTIONOVERRIDE");
                header.Add("ISAUTOFILLELIGIBLE");
                header.Add("ISDISEASEOVERRIDE");
                header.Add("ISDUPLICATETHERAPYOVERRIDE");
                header.Add("ISRESUBMISSION");
                header.Add("NUMBEROFTIMESCLAIMREJECTED");
                header.Add("NUMBEROFTIMESCLAIMSUBMITTED");
                header.Add("OTHERPAYERREJECTIONCODE");
                header.Add("PATIENTCLINICNUMBER");
                header.Add("PRICECODE");
                header.Add("PRICEGROUP");
                header.Add("PRICEOVERRIDE");
                header.Add("PRICEREGION");
                header.Add("PRICEZONE");
                header.Add("REBILLPROCESSINGFEE");
                header.Add("REJECTIONCODE");
                header.Add("TARFLAG");
                header.Add("THIRDPARTYSUBMITTYPE");
                header.Add("THIRDPARTYTRANSACTIONBATCHNBR");
                header.Add("TOLERANCEEXCEEDAMOUNT");
                header.Add("TOLERANCEEXCEEDPERCENTAGE");
                header.Add("TRANSACTIONCHARGEDTOAR");
                header.Add("TRANSACTIONMASTERENTEREDDATE");
                header.Add("WELFARECUSTOMERLOCATION");
                header.Add("WHOLESALEORDIRECTPURCHASE");
                header.Add("WORKERSCOMPENSATIONCARRIERID");
                header.Add("PATIENTACCOUNTNUMBER");
                header.Add("PATIENTADDRESS1");
                header.Add("PATIENTADDRESS2");
                header.Add("PATIENTCITY");
                header.Add("PATIENTCOUNTRYCODE");
                header.Add("PATIENTDATEOFBIRTH");
                header.Add("PATIENTDEPENDENTNUMBER");
                header.Add("PATIENTELIGIBILITYCLARIFNCODE");
                header.Add("PATIENTEMAIL");
                header.Add("PATIENTFIRSTNAME");
                header.Add("PATIENTFREQUENTSHOPPERID");
                header.Add("PATIENTGENDER");
                header.Add("PATIENTISEMPLOYED");
                header.Add("PATIENTISPREGNANT");
                header.Add("PATIENTISSENIORCITIZEN");
                header.Add("PATIENTLASTNAME");
                header.Add("PATIENTMARTIALSTATUS");
                header.Add("PATIENTMEDICALRECORDNUMBER");
                header.Add("PATIENTMEDICAREPARTDCOVERAGE");
                header.Add("PATIENTMEDIGAPID");
                header.Add("PATIENTMIDDLEINITIAL");
                header.Add("PATIENTOTHERPAYERCARDHOLDERID");
                header.Add("PATIENTOTHERPAYERDATEOFBIRTH");
                header.Add("PATIENTCODE");
                header.Add("PATIENTID");
                header.Add("PATIENTIDQUALIFIER");
                header.Add("PATIENTPERSONCODE");
                header.Add("PATIENTPHONENUMBER");
                header.Add("PATIENTPOSTALCODE");
                header.Add("PATIENTRELATIONSHIPCODE");
                header.Add("PATIENTSOCIALSECURITYNUMBER");
                header.Add("PATIENTSTATE");
                header.Add("PATIENTSTUDENTTYPE");
                header.Add("BIN");
                header.Add("INTERNALCONTROLNUMBER");
                header.Add("N102");
                header.Add("N103");
                header.Add("N104");
                header.Add("N301");
                header.Add("N302");
                header.Add("N401");
                header.Add("N402");
                header.Add("N403");
                header.Add("N404");
                header.Add("N407");
                header.Add("OTHERCOVERAGECODE");
                header.Add("OTHERPAYERID");
                header.Add("OTHERPAYERIDQUALIFIER");
                header.Add("PAYERID");
                header.Add("PAYERTYPE");
                header.Add("PROCESSORCONTROLNUMBER");
                header.Add("SECONDARYBIN");
                header.Add("SECONDARYID");
                header.Add("ORIGINATINGSTOREID");
                header.Add("PHARMACISTINITIALS");
                header.Add("PHARMACYTYPE");
                header.Add("PROVIDERACCEPTASSIGNMENT");
                header.Add("PROVIDERID");
                header.Add("PROVIDERIDQUALIFIER");
                header.Add("STOREFAXNUMBER");
                header.Add("STOREVERSIONNUMBER");
                header.Add("AGECOVERAGEEXPIRES");
                header.Add("GROUPINSURANCENUMBER");
                header.Add("HOMEPLAN");
                header.Add("HOSTINSURANCEPLAN");
                header.Add("INSURANCEPLAN");
                header.Add("OTHERPAYERGROUPID");
                header.Add("OTHERPAYERPLANNAME");
                header.Add("PATIENTGROUPNAME");
                header.Add("PAYERINSURANCEPLAN");
                header.Add("PHARMACYPLANNAME");
                header.Add("ACCIDENTTYPE");
                header.Add("AUTHORIZATIONNUMBER");
                header.Add("COMPOUNDDISPENSEDQUANTITY1");
                header.Add("COMPOUNDDISPENSEDQUANTITY2");
                header.Add("COMPOUNDDISPENSEDQUANTITY3");
                header.Add("COMPOUNDDISPENSEDQUANTITY4");
                header.Add("COMPOUNDDISPENSEDQUANTITY5");
                header.Add("COMPOUNDDISPENSEDQUANTITY6");
                header.Add("DAW");
                header.Add("DAYSSUPPLY");
                header.Add("DIAGNOSISPOINTER1");
                header.Add("DIRECTIONSFORUSE");
                header.Add("DISPENSEDQUANTITY");
                header.Add("DOSESPERDAY");
                header.Add("FROMFILLDATE");
                header.Add("HEADERRESPONSESTATUS");
                header.Add("HOSPITALIZATIONENDDATE");
                header.Add("HOSPITALIZATIONSTARTDATE");
                header.Add("HOSTTRANSACTIONNUMBER");
                header.Add("HOWRECORDSUBMITTED");
                header.Add("INITIALFILLDATE");
                header.Add("INJURYDATE");
                header.Add("ISCOMPOUNDCODE");
                header.Add("ISCOPAYASSIST");
                header.Add("ISCOPAYCARD");
                header.Add("ISDUROVERRIDE");
                header.Add("ISMAC");
                header.Add("ISOTHERINSURANCE");
                header.Add("ISOUTSIDELAB");
                header.Add("ISTHIRDPARTY");
                header.Add("LABFEES");
                header.Add("LEVELOFSERVICE");
                header.Add("MEDICAIDORIGINALREFERENCENBR");
                header.Add("MEDICAIDRESUBMISSIONCODE");
                header.Add("MEDICATIONPOSSESIONRATIO");
                header.Add("NUMBEROFREFILLSAUTHORIZED");
                header.Add("NUMBEROFREFILLSREMAINING");
                header.Add("ORIGINALDATEOFSAMEILLNESS");
                header.Add("ORIGINALLYPRESCRIBEDNDC");
                header.Add("ORIGINALSUBMISSIONDATE");
                header.Add("OTHERPAYERDATE");
                header.Add("OUTOFWORKENDDATE");
                header.Add("OUTOFWORKSTARTDATE");
                header.Add("PHARMACISTID");
                header.Add("PRESCRIPTIONDENIALCLARIFN");
                header.Add("PRESCRIPTIONNUMBER");
                header.Add("PRIORAUTHORIZATIONNUMBER");
                header.Add("PRIORAUTHORIZATIONTYPECODE");
                header.Add("PROFESSIONALSERVICECODE");
                header.Add("PROPORTIONOFDAYSCOVERED");
                header.Add("REFILLNUMBER");
                header.Add("SOLDDATE");
                header.Add("SPLITFILLNUMBER");
                header.Add("SUBMISSIONOFPAYMENTDATE");
                header.Add("TOFILLDATE");
                header.Add("TRANSACTIONCODE");
                header.Add("TRANSACTIONRESPONSESTATUS");
                header.Add("TRANSACTIONTYPECODE");
                header.Add("TRANSMISSIONDATE");
                header.Add("TRANSMISSIONTIME");
                header.Add("TREATMENTAUTHORIZATIONREQUEST");
                header.Add("UNITSPERDAY");
                header.Add("UNITSPERDOSE");
                header.Add("WORKERSCOMPENSATIONCLAIMID");
                header.Add("WRITTENDATE");
                header.Add("FORMSIGNEDDATE");
                header.Add("MEDICAIDAGENCYID");
                header.Add("MEDICAIDID");
                header.Add("ORIGINCODE");
                header.Add("PLACEOFSERVICE");
                header.Add("PRESCRIBERADDRESS1");
                header.Add("PRESCRIBERADDRESS2");
                header.Add("PRESCRIBERALTERNATEID");
                header.Add("PRESCRIBERCITY");
                header.Add("PRESCRIBERCOUNTRYCODE");
                header.Add("PRESCRIBERDEA");
                header.Add("PRESCRIBEREMAIL");
                header.Add("PRESCRIBERFAXNUMBER");
                header.Add("PRESCRIBERFIRSTNAME");
                header.Add("PRESCRIBERFULLNAME");
                header.Add("PRESCRIBERID");
                header.Add("PRESCRIBERIDQUALIFIER");
                header.Add("PRESCRIBERLASTNAME");
                header.Add("PRESCRIBERLICENSENUMBER");
                header.Add("PRESCRIBERNPI");
                header.Add("PRESCRIBERPHONENUMBER");
                header.Add("PRESCRIBERPOSTALCODE");
                header.Add("PRESCRIBERSPECIALTYCODE");
                header.Add("PRESCRIBERSTATE");
                header.Add("PRESCRIBERSTATEASSIGNEDIDNBR");
                header.Add("PRESCRIBERTYPE");
                header.Add("PRIMECAREPROVIDERFIRSTNAME");
                header.Add("PRIMECAREPROVIDERID");
                header.Add("PRIMECAREPROVIDERIDQUALIFIER");
                header.Add("PRIMECAREPROVIDERLASTNAME");
                header.Add("REFERRINGPRESCRIBERNAME");
                header.Add("REFERRINGPRESCRIBERNPI");
                header.Add("REFERRINGPRESCRIBERUPIN");
                header.Add("SERVICEPROVIDERID");
                header.Add("SERVICEPROVIDERIDQUALIFIER");

                return string.Join(fileDelimiter, header.ToArray());
            }
        }

        private string GetFileName(DateTime runDate)
        {
            return Path.Combine(this.outputFileLocation, string.Format("SmartRxCF_WFM_{0:yyyyMMdd}.txt", runDate));
        }
    }
}
