namespace RX.PharmacyBusiness.ETL.RXS582
{
    using RX.PharmacyBusiness.ETL.RXS582.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    [JobNotes("RXS582", "Download STC Immunization reporting to CDC.", "KBA00024401", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAO73L12FUDLI2ZRCB")]
    public class DownloadForSTC : ETLBase
    {
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string rerunName = (this.Arguments["-RerunName"] == null) ? string.Empty : this.Arguments["-RerunName"].ToString();
            this.fileManager = this.fileManager ?? new FileManager();
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();

            string inputLocation = @"%BATCH_ROOT%\RXS582\Input\";
            string outputLocation = @"%BATCH_ROOT%\RXS582\Output\";
            string archiveLocation = @"%BATCH_ROOT%\RXS582\Archive\";
            string rejectLocation = @"%BATCH_ROOT%\RXS582\Reject\";
            FileHelper fileHelper = new FileHelper(inputLocation, outputLocation, archiveLocation, rejectLocation);

            string sqlMckessonOracleDW = @"%BATCH_ROOT%\RXS582\bin\McKesson_Oracle_DW.sql";
            string sqlMckessonAzureCPS = @"%BATCH_ROOT%\RXS582\bin\McKesson_Azure_CPS.sql";
            string outputFilename = @"%BATCH_ROOT%\RXS582\Output\STCdata_" + runDate.ToString("yyyyMMdd") + ".txt";

            if (rerunName == "AFFLURIA2022")
            {
                sqlMckessonOracleDW = @"%BATCH_ROOT%\RXS582\bin\McKesson_Oracle_DW_Affluria_20220810_thru_20220930.sql";
                sqlMckessonAzureCPS = @"%BATCH_ROOT%\RXS582\bin\McKesson_Azure_CPS_Affluria_20220810_thru_20220930.sql";
                outputFilename = @"%BATCH_ROOT%\RXS582\Output\STCdata_Affluria_20220810_thru_20220930.txt";
            }

            if (rerunName == "NEWYORK2023")
            {
                sqlMckessonOracleDW = @"%BATCH_ROOT%\RXS582\bin\McKesson_Oracle_DW_NewYork_20220801_thru_20230920.sql";
                sqlMckessonAzureCPS = @"%BATCH_ROOT%\RXS582\bin\McKesson_Azure_CPS_NewYork_20220801_thru_20230920.sql";
                outputFilename = @"%BATCH_ROOT%\RXS582\Output\STCdata_NewYork_20220801_thru_20230920.txt";
            }

            if (rerunName == "FLUVAX2023")
            {
                sqlMckessonOracleDW = @"%BATCH_ROOT%\RXS582\bin\McKesson_Oracle_DW_FluVax_20230801_thru_20230919.sql";
                sqlMckessonAzureCPS = @"%BATCH_ROOT%\RXS582\bin\McKesson_Azure_CPS_FluVax_20230801_thru_20230919.sql";
                outputFilename = @"%BATCH_ROOT%\RXS582\Output\STCdata_FluVax_20230801_thru_20230919.txt";
            }

            Log.LogInfo("Begin STC data feed with RunDate [{0}] and RerunName [{0}].", runDate.ToString("MM/dd/yyyy"), rerunName);

            //The following is only useful if we have one data source per output file, but we have to merge two data sources.
            //oracleHelper.DownloadQueryByRunDateToFile(
            //    150,
            //    @"%BATCH_ROOT%\RXS582\bin\STCdata_YYYYMMDD.sql",
            //    @"%BATCH_ROOT%\RXS582\Output\STCdata_" + runDate.ToString("yyyyMMdd") + ".txt",
            //    runDate,
            //    true,
            //    "|",
            //    string.Empty,
            //    true,
            //    "ENTERPRISE_RX"
            //    );

            Log.LogInfo("Get vaccination data from McKesson Oracle DW.");
            List<VaccinationRecordFromMcKessonOracleDW> vaccinationDispensedList = oracleHelper.DownloadQueryByRunDateToList<VaccinationRecordFromMcKessonOracleDW>(
                150,
                sqlMckessonOracleDW,
                runDate,
                "ENTERPRISE_RX"
                );
            fileHelper.WriteListToFile<VaccinationRecordFromMcKessonOracleDW>(
                vaccinationDispensedList,
                @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\RawData_VaccinationDispensedList_" + runDate.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                false,
                false);

            Log.LogInfo("Get COVID questionnaire data from McKesson SqlServer in Azure cloud.");
            List<CpsRecord> cpsQuestionnaireList = sqlServerHelper.DownloadQueryByRunDateToList<CpsRecord>(
                sqlMckessonAzureCPS,
                runDate,
                "ENTERPRISE_RX_AZURE"
                );

            Log.LogInfo("Get NDC Conversions.");
            DelimitedStreamReaderOptions ndcConversionFileOptions = new DelimitedStreamReaderOptions(
                Constants.CharPipe,
                new Nullable<char>(),
                true,
                false,
                3,
                1
            );

            List<NdcConversionRecord> ndcConversionRecords = fileHelper.ReadFilesToList<NdcConversionRecord>(
                "NDC_Conversions.txt",
                ndcConversionFileOptions,
                false);

            if (!ndcConversionRecords.Any())
                throw new Exception("No records found in NDC_Conversions.txt that was created by job RXS518.");

            Log.LogInfo("Merge data sources into one list.");
            //See CDC requirements document for translations of data values to codes.
            foreach (var cd in vaccinationDispensedList)
            {
                //NDC Conversions
                if (ndcConversionRecords.Where(r => r.DrugOriginalNdc == cd.STR_PRODUCT_NDC).Count() > 0)
                {
                    //Note: Change Drug Name first and Drug NDC last so that the linq statement works.

                    cd.STR_PRODUCT_NAME = ndcConversionRecords
                        .Where(r => r.DrugOriginalNdc == cd.STR_PRODUCT_NDC)
                        .Select(s => s.DrugNameConversion)
                        .FirstOrDefault();

                    cd.STR_PRODUCT_NDC = ndcConversionRecords
                        .Where(r => r.DrugOriginalNdc == cd.STR_PRODUCT_NDC)
                        .Select(s => s.DrugNdcConversion)
                        .FirstOrDefault();
                }

                //RECIP_RACE_1
                cd.RECIP_RACE_1 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.DEC_INTERNALPTNUM &&
                        cps.StoreNbr == cd.STR_FACILITY_ID &&
                        cps.RxNbr == cd.STR_RX_NUMBER &&
                        cps.RefillNbr == cd.refill_num &&
                        (cps.KeyDesc == "Native American or Alaska Native" ||
                         cps.KeyDesc == "Asian" ||
                         cps.KeyDesc == "Native Hawaiian or Other Pacific Islander" ||
                         cps.KeyDesc == "Black or African American" ||
                         cps.KeyDesc == "White" ||
                         cps.KeyDesc == "Other" ||
                         cps.KeyDesc == "Unknown"
                        )
                    )
                    .Select(s =>
                        (s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true") ? "1002-5" :
                        (s.KeyDesc == "Asian" && s.KeyValue == "true") ? "2028-9" :
                        (s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true") ? "2076-8" :
                        (s.KeyDesc == "Black or African American" && s.KeyValue == "true") ? "2054-5" :
                        (s.KeyDesc == "White" && s.KeyValue == "true") ? "2106-3" :
                        (s.KeyDesc == "Other" && s.KeyValue == "true") ? "2131-1" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //RECIP_ETHNICITY
                cd.RECIP_ETHNICITY = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.DEC_INTERNALPTNUM &&
                        cps.StoreNbr == cd.STR_FACILITY_ID &&
                        cps.RxNbr == cd.STR_RX_NUMBER &&
                        cps.RefillNbr == cd.refill_num &&
                        cps.KeyDesc == "Ethnicity"
                    )
                    .Select(s =>
                        (s.KeyValue == "Hispanic or Latino") ? "2135-2" :
                        (s.KeyValue == "Not Hispanic or Latino") ? "2186-5" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //NY_PRIORITY_GROUP - on 8/1 STC confirmed that they want this field blank now, so we left this code and modified output record
                cd.NY_PRIORITY_GROUP = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.DEC_INTERNALPTNUM &&
                        cps.StoreNbr == cd.STR_FACILITY_ID &&
                        cps.RxNbr == cd.STR_RX_NUMBER &&
                        cps.RefillNbr == cd.refill_num &&
                        cps.KeyDesc == "COVID Priority Group - NY Stores ONLY"
                    )
                    .Select(s =>
                        (s.KeyValue == "Age") ? "AGE" :
                        (s.KeyValue == "Age 65-74") ? "65-74" :
                        (s.KeyValue == "Age 75 or above") ? "75+" :
                        (s.KeyValue == "Healthcare Provider - Other") ? "HCPOTHER" :
                        (s.KeyValue == "Frontline Worker (agriculture, USPS, grocery, education, public transportation, manufacturing)") ? "FRONTLINE" :
                        (s.KeyValue == "Healthcare - Ambulatory Staff") ? "HCPAMB" :
                        (s.KeyValue == "EMS Healthcare worker") ? "HCPEMS" :
                        (s.KeyValue == "Healthcare Professional - hospital staff") ? "HCPHOSP" :
                        (s.KeyValue == "Medical examiner, coroner, mortician") ? "HCPME" :
                        (s.KeyValue == "Long Term Care - Facility Staff") ? "LTCHCP" :
                        (s.KeyValue == "Long Term Care - Resident") ? "LTCRES" :
                        (s.KeyValue == "Other Essential Worker") ? "OTHESSENTIAL" :
                        (s.KeyValue == "Public Safety - fire, police, corrections") ? "PUBSAFE" :
                        (s.KeyValue == "Congregate Care Resident (not Long Term Care)") ? "RESCONG" :
                        (s.KeyValue == "Under age 65 with health conditions") ? "U65HEALTH" :
                        (s.KeyValue == "3rd Dose - Health Reason") ? "3HEALTH" :
                        ""
                    )
                    .FirstOrDefault();

                //Enhancement requested by DC on 1/30/2021 to have any empty questionnaire strings set to "UNK".
                if (string.IsNullOrEmpty(cd.RECIP_RACE_1)) cd.RECIP_RACE_1 = "UNK";
                if (string.IsNullOrEmpty(cd.RECIP_ETHNICITY)) cd.RECIP_ETHNICITY = "UNK";
            }

            Log.LogInfo("Begin translatating merged data to only those data elements required by CDC.");
            List<VaccinationRecordForSTC> stcList = vaccinationDispensedList
                .Select(s => new VaccinationRecordForSTC
                {
                    STR_FIRST_NAME = s.STR_FIRST_NAME,
                    STR_MIDDLE_NAME = s.STR_MIDDLE_NAME,
                    STR_LAST_NAME = s.STR_LAST_NAME,
                    DT_BIRTH_DATE = s.DT_BIRTH_DATE,
                    C_GENDER = s.C_GENDER,
                    STR_ADDRESS_ONE = s.STR_ADDRESS_ONE,
                    STR_ADDRESS_TWO = s.STR_ADDRESS_TWO,
                    STR_CITY = s.STR_CITY,
                    STR_STATE = s.STR_STATE,
                    STR_ZIP = s.STR_ZIP,
                    STR_PRODUCT_NAME = s.STR_PRODUCT_NAME,
                    STR_PRODUCT_NDC = s.STR_PRODUCT_NDC,
                    DEC_DISPENSED_QTY = s.DEC_DISPENSED_QTY,
                    C_GENERIC_IDENTIFIER = s.C_GENERIC_IDENTIFIER,
                    STR_VERIFIED_RPH_FIRST = s.STR_VERIFIED_RPH_FIRST,
                    STR_VERIFIED_RPH_LAST = s.STR_VERIFIED_RPH_LAST,
                    STR_FACILITY_ID = s.STR_FACILITY_ID,
                    STR_RX_NUMBER = s.STR_RX_NUMBER,
                    STR_DOSAGE_FORM = s.STR_DOSAGE_FORM,
                    STR_STRENGTH = s.STR_STRENGTH,
                    DT_SOLD_DATE = s.DT_SOLD_DATE,
                    DT_CANCELLED_DATE = s.DT_CANCELLED_DATE,
                    C_ACTION_CODE = s.C_ACTION_CODE,
                    STR_DEA = s.STR_DEA,
                    STR_NPI = s.STR_NPI,
                    DEC_INTERNALPTNUM = s.DEC_INTERNALPTNUM,
                    LOT_NUMBER = s.LOT_NUMBER,
                    EXP_DATE = s.EXP_DATE,
                    STR_PATIENTEMAIL = s.STR_PATIENTEMAIL,
                    VIS_PRESENTED_DATE = s.VIS_PRESENTED_DATE,
                    ADMINISTRATION_SITE = s.ADMINISTRATION_SITE,
                    PROTECTION_INDICATOR = s.PROTECTION_INDICATOR,
                    RECIP_RACE_1 = s.RECIP_RACE_1,
                    RECIP_ETHNICITY = s.RECIP_ETHNICITY,
                    VFC_STATUS = s.VFC_STATUS,
                    NY_PRIORITY_GROUP = "", //on 8/1 STC confirmed that they want this field blank now
                    PHONE_NUMBER = s.PHONE_NUMBER,
                    PRESCRIBED_BY_ID = s.PRESCRIBED_BY_ID
                })
                .ToList();

            Log.LogInfo("Begin output of merged data to a pipe-delimited text file.");
            sqlServerHelper.WriteListToFile(
                stcList,
                outputFilename,
                true,
                "|",
                string.Empty,
                true);
            fileHelper.CopyFileToArchiveForQA(outputFilename);

            Log.LogInfo("Completed making data feed for STC.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
