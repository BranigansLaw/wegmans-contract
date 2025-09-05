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

    // This class is intended for use with sending only the historical COVID 19 vaccines for NY to STC after the patient phone number was added to the record
    public class HistoricalDownloadForSTC : ETLBase
    {
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            DateTime startDate = (this.Arguments["-StartDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-StartDate"].ToString());
            this.fileManager = this.fileManager ?? new FileManager();
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();

            bool writeHeader = false;
            if (runDate == startDate)
            {
                writeHeader = true;
            }

            Log.LogInfo("Begin STC data feed with RunDate [{0}].", runDate.ToString("MM/dd/yyyy"));

            Log.LogInfo("Begin vaccination data from McKesson Oracle DW.");
            List<VaccinationRecordFromMcKessonOracleDW> vaccinationDispensedList = oracleHelper.DownloadQueryByRunDateToList<VaccinationRecordFromMcKessonOracleDW>(
                150,
                @"%BATCH_ROOT%\RXS582\bin\McKesson_Oracle_DW_COVID.sql",
                runDate,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Begin COVID questionnaire data from McKesson SqlServer in Azure cloud.");
            List<CpsRecord> cpsQuestionnaireList = sqlServerHelper.DownloadQueryByRunDateToList<CpsRecord>(
                @"%BATCH_ROOT%\RXS582\bin\McKesson_Azure_CPS.sql",
                runDate,
                "ENTERPRISE_RX_AZURE"
                );

            Log.LogInfo("Begin merging two data sources into one list.");
            //See CDC requirements document for translations of data values to codes.
            foreach (var cd in vaccinationDispensedList)
            {
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

                //ADMINISTRATION_SITE
                if (cd.STR_STATE == "PA")
                {
                    cd.ADMINISTRATION_SITE = cpsQuestionnaireList
                        .Where(cps =>
                            cps.PatientNum == cd.DEC_INTERNALPTNUM &&
                            cps.StoreNbr == cd.STR_FACILITY_ID &&
                            cps.RxNbr == cd.STR_RX_NUMBER &&
                            cps.RefillNbr == cd.refill_num &&
                            cps.KeyDesc == "Administration Site"
                        )
                        .Select(s =>
                            (s.KeyValue == "Right arm") ? "RD" :
                            (s.KeyValue == "Left arm") ? "LD" :
                            "UNK"
                        )
                        .FirstOrDefault();
                }
                else
                {
                    cd.ADMINISTRATION_SITE = cpsQuestionnaireList
                        .Where(cps =>
                            cps.PatientNum == cd.DEC_INTERNALPTNUM &&
                            cps.StoreNbr == cd.STR_FACILITY_ID &&
                            cps.RxNbr == cd.STR_RX_NUMBER &&
                            cps.RefillNbr == cd.refill_num &&
                            cps.KeyDesc == "Administration Site"
                        )
                        .Select(s =>
                            (s.KeyValue == "Right arm") ? "RA" :
                            (s.KeyValue == "Left arm") ? "LA" :
                            "UNK"
                        )
                        .FirstOrDefault();
                }

                //NY_PRIORITY_GROUP
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
                if (string.IsNullOrEmpty(cd.ADMINISTRATION_SITE)) cd.ADMINISTRATION_SITE = "UNK";
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
                    NY_PRIORITY_GROUP = s.NY_PRIORITY_GROUP,
                    PHONE_NUMBER = s.PHONE_NUMBER
                })
                .ToList();

            Log.LogInfo("Begin output of merged data to a pipe-delimited text file.");

            sqlServerHelper.WriteListToFile(
                stcList,
                @"%BATCH_ROOT%\RXS582\Output\STCdata_" + DateTime.Now.Date.ToString("yyyyMMdd") + "_HIST.txt",
                writeHeader,
                "|",
                string.Empty,
                true,
                true);

            Log.LogInfo("Completed making data feed for STC.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
