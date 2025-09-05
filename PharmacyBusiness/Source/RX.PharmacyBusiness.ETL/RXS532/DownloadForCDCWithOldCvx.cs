namespace RX.PharmacyBusiness.ETL.RXS532
{
    using RX.PharmacyBusiness.ETL.RXS532.Core;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    [JobNotes("RXS532", "Download COVID Vaccinations for CDC Reporting.", "KBA00045341", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGFVWBYIWVLKAQNFPJ7QMHJ6EKI0S")]
    public class DownloadForCDCWithOldCvx : ETLBase
    {
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string filenameModification = (this.Arguments["-FilenameModification"] == null) ? string.Empty : this.Arguments["-FilenameModification"].ToString();
            this.fileManager = this.fileManager ?? new FileManager();
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();

            Log.LogInfo("Download CDC data feed with RunDate [{0}] and filename modification [{1}].",
                runDate.ToString("MM/dd/yyyy"),
                filenameModification);

            //The following is only useful if we have one data source per output file, but we have to merge two data sources.
            //oracleHelper.DownloadQueryByRunDateToFile(
            //    150,
            //    @"%BATCH_ROOT%\RXS532\bin\McKesson_Oracle_DW.sql",
            //    @"%BATCH_ROOT%\RXS532\Output\" + runDate.ToString("yyyyMMdd") + "_" + twoCharacterRunNumber + "_NYA.txt",
            //    runDate,
            //    true,
            //    "\t", //TAB
            //    string.Empty,
            //    true,
            //    "ENTERPRISE_RX"
            //    );

            Log.LogInfo("Query COVID vaccination data from McKesson Oracle DW.");
            List<VaccinationRecordFromMcKessonOracleDW> covidDispensedList = oracleHelper.DownloadQueryByRunDateToList<VaccinationRecordFromMcKessonOracleDW>(
                150,
                @"%BATCH_ROOT%\RXS532\bin\McKesson_Oracle_DW_WithOldCvx.sql",
                runDate,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Query COVID questionnaire data from McKesson SqlServer in Azure cloud.");
            List<CpsRecord> cpsQuestionnaireList = sqlServerHelper.DownloadQueryByRunDateToList<CpsRecord>(
                @"%BATCH_ROOT%\RXS532\bin\McKesson_Azure_CPS.sql",
                runDate,
                "ENTERPRISE_RX_AZURE"
                );

            Log.LogInfo("Begin merging two data sources into one COVID list.");
            //See CDC requirements document for translations of data values to codes.
            foreach (var cd in covidDispensedList)
            {
                //RECIP_RACE_1
                cd.RECIP_RACE_1 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
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

                //RECIP_RACE_2
                cd.RECIP_RACE_2 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cd.RECIP_RACE_1 != "UNK" &&
                        ((cps.KeyDesc == "Native American or Alaska Native" && cd.RECIP_RACE_1 != "1002-5") ||
                         (cps.KeyDesc == "Asian" && cd.RECIP_RACE_1 != "2028-9") ||
                         (cps.KeyDesc == "Native Hawaiian or Other Pacific Islander" && cd.RECIP_RACE_1 != "2076-8") ||
                         (cps.KeyDesc == "Black or African American" && cd.RECIP_RACE_1 != "2054-5") ||
                         (cps.KeyDesc == "White" && cd.RECIP_RACE_1 != "2106-3") ||
                         (cps.KeyDesc == "Other" && cd.RECIP_RACE_1 != "2131-1")
                        )
                    )
                    .Select(s =>
                        (s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true") ? "1002-5" :
                        (s.KeyDesc == "Asian" && s.KeyValue == "true") ? "2028-9" :
                        (s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true") ? "2076-8" :
                        (s.KeyDesc == "Black or African American" && s.KeyValue == "true") ? "2054-5" :
                        (s.KeyDesc == "White" && s.KeyValue == "true") ? "2106-3" :
                        (s.KeyDesc == "Other" && s.KeyValue == "true") ? "2131-1" :
                        ""
                    )
                    .FirstOrDefault();

                //RECIP_RACE_3
                cd.RECIP_RACE_3 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cd.RECIP_RACE_1 != "UNK" &&
                        cd.RECIP_RACE_2 != "" &&
                        ((cps.KeyDesc == "Native American or Alaska Native" && cd.RECIP_RACE_1 != "1002-5" && cd.RECIP_RACE_2 != "1002-5") ||
                         (cps.KeyDesc == "Asian" && cd.RECIP_RACE_1 != "2028-9" && cd.RECIP_RACE_2 != "2028-9") ||
                         (cps.KeyDesc == "Native Hawaiian or Other Pacific Islander" && cd.RECIP_RACE_1 != "2076-8" && cd.RECIP_RACE_2 != "2076-8") ||
                         (cps.KeyDesc == "Black or African American" && cd.RECIP_RACE_1 != "2054-5" && cd.RECIP_RACE_2 != "2054-5") ||
                         (cps.KeyDesc == "White" && cd.RECIP_RACE_1 != "2106-3" && cd.RECIP_RACE_2 != "2106-3") ||
                         (cps.KeyDesc == "Other" && cd.RECIP_RACE_1 != "2131-1" && cd.RECIP_RACE_2 != "2131-1")
                        )
                    )
                    .Select(s =>
                        (s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true") ? "1002-5" :
                        (s.KeyDesc == "Asian" && s.KeyValue == "true") ? "2028-9" :
                        (s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true") ? "2076-8" :
                        (s.KeyDesc == "Black or African American" && s.KeyValue == "true") ? "2054-5" :
                        (s.KeyDesc == "White" && s.KeyValue == "true") ? "2106-3" :
                        (s.KeyDesc == "Other" && s.KeyValue == "true") ? "2131-1" :
                        ""
                    )
                    .FirstOrDefault();

                //RECIP_RACE_4
                cd.RECIP_RACE_4 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cd.RECIP_RACE_1 != "UNK" &&
                        cd.RECIP_RACE_2 != "" &&
                        cd.RECIP_RACE_3 != "" &&
                        ((cps.KeyDesc == "Native American or Alaska Native" && cd.RECIP_RACE_1 != "1002-5" && cd.RECIP_RACE_2 != "1002-5" && cd.RECIP_RACE_3 != "1002-5") ||
                         (cps.KeyDesc == "Asian" && cd.RECIP_RACE_1 != "2028-9" && cd.RECIP_RACE_2 != "2028-9" && cd.RECIP_RACE_3 != "2028-9") ||
                         (cps.KeyDesc == "Native Hawaiian or Other Pacific Islander" && cd.RECIP_RACE_1 != "2076-8" && cd.RECIP_RACE_2 != "2076-8" && cd.RECIP_RACE_3 != "2076-8") ||
                         (cps.KeyDesc == "Black or African American" && cd.RECIP_RACE_1 != "2054-5" && cd.RECIP_RACE_2 != "2054-5" && cd.RECIP_RACE_3 != "2054-5") ||
                         (cps.KeyDesc == "White" && cd.RECIP_RACE_1 != "2106-3" && cd.RECIP_RACE_2 != "2106-3" && cd.RECIP_RACE_3 != "2106-3") ||
                         (cps.KeyDesc == "Other" && cd.RECIP_RACE_1 != "2131-1" && cd.RECIP_RACE_2 != "2131-1" && cd.RECIP_RACE_3 != "2131-1")
                        )
                    )
                    .Select(s =>
                        (s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true") ? "1002-5" :
                        (s.KeyDesc == "Asian" && s.KeyValue == "true") ? "2028-9" :
                        (s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true") ? "2076-8" :
                        (s.KeyDesc == "Black or African American" && s.KeyValue == "true") ? "2054-5" :
                        (s.KeyDesc == "White" && s.KeyValue == "true") ? "2106-3" :
                        (s.KeyDesc == "Other" && s.KeyValue == "true") ? "2131-1" :
                        ""
                    )
                    .FirstOrDefault();

                //RECIP_RACE_5
                cd.RECIP_RACE_5 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cd.RECIP_RACE_1 != "UNK" &&
                        cd.RECIP_RACE_2 != "" &&
                        cd.RECIP_RACE_3 != "" &&
                        cd.RECIP_RACE_4 != "" &&
                        ((cps.KeyDesc == "Native American or Alaska Native" && cd.RECIP_RACE_1 != "1002-5" && cd.RECIP_RACE_2 != "1002-5" && cd.RECIP_RACE_3 != "1002-5" && cd.RECIP_RACE_4 != "1002-5") ||
                         (cps.KeyDesc == "Asian" && cd.RECIP_RACE_1 != "2028-9" && cd.RECIP_RACE_2 != "2028-9" && cd.RECIP_RACE_3 != "2028-9" && cd.RECIP_RACE_4 != "2028-9") ||
                         (cps.KeyDesc == "Native Hawaiian or Other Pacific Islander" && cd.RECIP_RACE_1 != "2076-8" && cd.RECIP_RACE_2 != "2076-8" && cd.RECIP_RACE_3 != "2076-8" && cd.RECIP_RACE_4 != "2076-8") ||
                         (cps.KeyDesc == "Black or African American" && cd.RECIP_RACE_1 != "2054-5" && cd.RECIP_RACE_2 != "2054-5" && cd.RECIP_RACE_3 != "2054-5" && cd.RECIP_RACE_4 != "2054-5") ||
                         (cps.KeyDesc == "White" && cd.RECIP_RACE_1 != "2106-3" && cd.RECIP_RACE_2 != "2106-3" && cd.RECIP_RACE_3 != "2106-3" && cd.RECIP_RACE_4 != "2106-3") ||
                         (cps.KeyDesc == "Other" && cd.RECIP_RACE_1 != "2131-1" && cd.RECIP_RACE_2 != "2131-1" && cd.RECIP_RACE_3 != "2131-1" && cd.RECIP_RACE_4 != "2131-1")
                        )
                    )
                    .Select(s =>
                        (s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true") ? "1002-5" :
                        (s.KeyDesc == "Asian" && s.KeyValue == "true") ? "2028-9" :
                        (s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true") ? "2076-8" :
                        (s.KeyDesc == "Black or African American" && s.KeyValue == "true") ? "2054-5" :
                        (s.KeyDesc == "White" && s.KeyValue == "true") ? "2106-3" :
                        (s.KeyDesc == "Other" && s.KeyValue == "true") ? "2131-1" :
                        ""
                    )
                    .FirstOrDefault();

                //RECIP_RACE_6
                cd.RECIP_RACE_6 = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cd.RECIP_RACE_1 != "UNK" &&
                        cd.RECIP_RACE_2 != "" &&
                        cd.RECIP_RACE_3 != "" &&
                        cd.RECIP_RACE_4 != "" &&
                        cd.RECIP_RACE_5 != "" &&
                        ((cps.KeyDesc == "Native American or Alaska Native" && cd.RECIP_RACE_1 != "1002-5" && cd.RECIP_RACE_2 != "1002-5" && cd.RECIP_RACE_3 != "1002-5" && cd.RECIP_RACE_4 != "1002-5" && cd.RECIP_RACE_5 != "1002-5") ||
                         (cps.KeyDesc == "Asian" && cd.RECIP_RACE_1 != "2028-9" && cd.RECIP_RACE_2 != "2028-9" && cd.RECIP_RACE_3 != "2028-9" && cd.RECIP_RACE_4 != "2028-9" && cd.RECIP_RACE_5 != "2028-9") ||
                         (cps.KeyDesc == "Native Hawaiian or Other Pacific Islander" && cd.RECIP_RACE_1 != "2076-8" && cd.RECIP_RACE_2 != "2076-8" && cd.RECIP_RACE_3 != "2076-8" && cd.RECIP_RACE_4 != "2076-8" && cd.RECIP_RACE_5 != "2076-8") ||
                         (cps.KeyDesc == "Black or African American" && cd.RECIP_RACE_1 != "2054-5" && cd.RECIP_RACE_2 != "2054-5" && cd.RECIP_RACE_3 != "2054-5" && cd.RECIP_RACE_4 != "2054-5" && cd.RECIP_RACE_5 != "2054-5") ||
                         (cps.KeyDesc == "White" && cd.RECIP_RACE_1 != "2106-3" && cd.RECIP_RACE_2 != "2106-3" && cd.RECIP_RACE_3 != "2106-3" && cd.RECIP_RACE_4 != "2106-3" && cd.RECIP_RACE_5 != "2106-3") ||
                         (cps.KeyDesc == "Other" && cd.RECIP_RACE_1 != "2131-1" && cd.RECIP_RACE_2 != "2131-1" && cd.RECIP_RACE_3 != "2131-1" && cd.RECIP_RACE_4 != "2131-1" && cd.RECIP_RACE_5 != "2131-1")
                        )
                    )
                    .Select(s =>
                        (s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true") ? "1002-5" :
                        (s.KeyDesc == "Asian" && s.KeyValue == "true") ? "2028-9" :
                        (s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true") ? "2076-8" :
                        (s.KeyDesc == "Black or African American" && s.KeyValue == "true") ? "2054-5" :
                        (s.KeyDesc == "White" && s.KeyValue == "true") ? "2106-3" :
                        (s.KeyDesc == "Other" && s.KeyValue == "true") ? "2131-1" :
                        ""
                    )
                    .FirstOrDefault();

                //RECIP_ETHNICITY
                cd.RECIP_ETHNICITY = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cps.KeyDesc == "Ethnicity"
                    )
                    .Select(s =>
                        (s.KeyValue == "Hispanic or Latino") ? "2135-2" :
                        (s.KeyValue == "Not Hispanic or Latino") ? "2186-5" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //VAX_ADMIN_SITE
                cd.VAX_ADMIN_SITE = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cps.KeyDesc == "Administration Site"
                    )
                    .Select(s =>
                        (s.KeyValue == "Right arm") ? "RA" :
                        (s.KeyValue == "Left arm") ? "LA" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //DOSE_NUM
                cd.DOSE_NUM = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        (cps.KeyDesc == "Previous dose received?" || cps.KeyDesc == "Previous vaccine dose received?")
                    )
                    .Select(s =>
                        (s.KeyValue == "First dose in series") ? "1" :
                        (s.KeyValue == "Second dose in series") ? "2" :
                        (s.KeyValue == "Third dose in series") ? "3" :
                        (s.KeyValue == "Fourth dose in series") ? "4" :
                        (s.KeyValue == "Fifth dose in series") ? "5" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //VAX_SERIES_COMPLETE
                cd.VAX_SERIES_COMPLETE = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        (cps.KeyDesc == "Previous dose received?" || cps.KeyDesc == "Previous vaccine dose received?")
                    )
                    .Select(s =>
                        (cd.MVX == "JSN" && s.KeyValue == "First dose in series") ? "YES" :  // J&J has only one dose in the series
                        (cd.MVX == "MOD" && s.KeyValue == "First dose in series") ? "NO" :
                        (cd.MVX == "PFR" && s.KeyValue == "First dose in series") ? "NO" :
                        (s.KeyValue == "Second dose in series") ? "YES" :
                        (s.KeyValue == "Third dose in series") ? "YES" :
                        (s.KeyValue == "Fourth dose in series") ? "YES" :
                        (s.KeyValue == "Fifth dose in series") ? "YES" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //CMORBID_STATUS
                cd.CMORBID_STATUS = cpsQuestionnaireList
                    .Where(cps =>
                        cps.PatientNum == cd.RECIP_ID &&
                        cps.StoreNbr == cd.fd_facility_id &&
                        cps.RxNbr == cd.rx_number &&
                        cps.RefillNbr == cd.refill_num &&
                        cps.KeyDesc == "Medical Conditions (At Increased Risk)"
                    )
                    .Select(s =>
                        (s.KeyValue == "No") ? "NO" :
                        (s.KeyValue == "Yes") ? "YES" :
                        "UNK"
                    )
                    .FirstOrDefault();

                //Enhancement requested by DC on 1/30/2021 to have any empty questionnaire strings set to "UNK".
                if (string.IsNullOrEmpty(cd.RECIP_RACE_1)) cd.RECIP_RACE_1 = "UNK";
                if (string.IsNullOrEmpty(cd.RECIP_ETHNICITY)) cd.RECIP_ETHNICITY = "UNK";
                if (string.IsNullOrEmpty(cd.VAX_ADMIN_SITE)) cd.VAX_ADMIN_SITE = "UNK";
                if (string.IsNullOrEmpty(cd.DOSE_NUM)) cd.DOSE_NUM = "UNK";
                if (string.IsNullOrEmpty(cd.VAX_SERIES_COMPLETE)) cd.VAX_SERIES_COMPLETE = "UNK";
                if (string.IsNullOrEmpty(cd.CMORBID_STATUS)) cd.CMORBID_STATUS = "UNK";
            }

            Log.LogInfo("Begin translatating merged data to only those data elements required by CDC.");
            List<VaccinationRecordForCDC> cdcList = covidDispensedList
                //.Where(w => w.ADMIN_ADDRESS_STATE != "NY") //Commented out on Mar 29, 2021 per request from Deanna Catenuto.
                .Select(s => new VaccinationRecordForCDC
                {
                    VAX_EVENT_ID = s.VAX_EVENT_ID,
                    EXT_TYPE = s.EXT_TYPE,
                    PPRL_ID = s.PPRL_ID,
                    RECIP_ID = s.RECIP_ID,
                    RECIP_FIRST_NAME = s.RECIP_FIRST_NAME,
                    RECIP_MIDDLE_NAME = s.RECIP_MIDDLE_NAME,
                    RECIP_LAST_NAME = s.RECIP_LAST_NAME,
                    RECIP_DOB = s.RECIP_DOB,
                    RECIP_SEX = s.RECIP_SEX,
                    RECIP_ADDRESS_STREET = s.RECIP_ADDRESS_STREET,
                    RECIP_ADDRESS_STREET_2 = s.RECIP_ADDRESS_STREET_2,
                    RECIP_ADDRESS_CITY = s.RECIP_ADDRESS_CITY,
                    RECIP_ADDRESS_COUNTY = s.RECIP_ADDRESS_COUNTY,
                    RECIP_ADDRESS_STATE = s.RECIP_ADDRESS_STATE,
                    RECIP_ADDRESS_ZIP = s.RECIP_ADDRESS_ZIP,
                    RECIP_RACE_1 = s.RECIP_RACE_1,
                    RECIP_RACE_2 = s.RECIP_RACE_2,
                    RECIP_RACE_3 = s.RECIP_RACE_3,
                    RECIP_RACE_4 = s.RECIP_RACE_4,
                    RECIP_RACE_5 = s.RECIP_RACE_5,
                    RECIP_RACE_6 = s.RECIP_RACE_6,
                    RECIP_ETHNICITY = s.RECIP_ETHNICITY,
                    ADMIN_DATE = s.ADMIN_DATE,
                    CVX = s.CVX,
                    NDC = s.NDC,
                    MVX = s.MVX,
                    LOT_NUMBER = s.LOT_NUMBER,
                    VAX_EXPIRATION = s.VAX_EXPIRATION,
                    VAX_ADMIN_SITE = s.VAX_ADMIN_SITE,
                    VAX_ROUTE = s.VAX_ROUTE,
                    DOSE_NUM = s.DOSE_NUM,
                    VAX_SERIES_COMPLETE = s.VAX_SERIES_COMPLETE,
                    RESPONSIBLE_ORG = s.RESPONSIBLE_ORG,
                    ADMIN_NAME = s.ADMIN_NAME,
                    VTRCKS_PROV_PIN = s.VTRCKS_PROV_PIN,
                    ADMIN_TYPE = s.ADMIN_TYPE,
                    ADMIN_ADDRESS_STREET = s.ADMIN_ADDRESS_STREET,
                    ADMIN_ADDRESS_STREET_2 = s.ADMIN_ADDRESS_STREET_2,
                    ADMIN_ADDRESS_CITY = s.ADMIN_ADDRESS_CITY,
                    ADMIN_ADDRESS_COUNTY = s.ADMIN_ADDRESS_COUNTY,
                    ADMIN_ADDRESS_STATE = s.ADMIN_ADDRESS_STATE,
                    ADMIN_ADDRESS_ZIP = s.ADMIN_ADDRESS_ZIP,
                    VAX_REFUSAL = s.VAX_REFUSAL,
                    CMORBID_STATUS = s.CMORBID_STATUS,
                    SEROLOGY = s.SEROLOGY
                })
                .ToList();

            Log.LogInfo("Begin output of merged data to a tab-delimited text file.");
            sqlServerHelper.WriteListToFile(
                cdcList,
                @"%BATCH_ROOT%\RXS532\Output\Wegmans_CovidVaccinations_" + runDate.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("hhmmss") + filenameModification + ".txt",
                true,
                "\t", //TAB
                string.Empty,
                true);

            Log.LogInfo("Completed making data feed for CDC.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
