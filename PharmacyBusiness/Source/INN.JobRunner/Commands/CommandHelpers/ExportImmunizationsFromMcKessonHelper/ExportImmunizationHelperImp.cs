using Amazon.S3.Model.Internal.MarshallTransformations;
using Library.McKessonCPSInterface.DataModel;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportImmunizationsFromMcKessonHelper
{
    public class ExportImmunizationHelperImp : IExportImmunizationHelper
    {
        public string? GetStoreProductNumberWhereNdcwoEqualsStrProductNdc(IEnumerable<NdcConversionRow> ndcConversionRows, ImmunizationRow immunizationRow)
        {
            return ndcConversionRows
                        .Where(r => r.NdcWo == immunizationRow.StrProductNdc)
                        .Select(s => s.DrugNameConversion)
                        .FirstOrDefault();
        }

        public string? GetStoreProductNdcWhereNdcwoEqualsStrProductNdc(IEnumerable<NdcConversionRow> ndcConversionRows, ImmunizationRow immunizationRow)
        {
            return ndcConversionRows
                        .Where(r => r.NdcWo == immunizationRow.StrProductNdc)
                        .Select(s => s.NdcConversionWo)
                        .FirstOrDefault();
        }

        public string? GetRaceRecipOneWherePatientNumberEqualsDecInternalPtNum(IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows, ImmunizationRow immunizationRow)
        {
            return immunizationQuestionnaireRows
                    .Where(cps =>
                        cps.PatientNum == immunizationRow.DecInternalPtNum &&
                        cps.StoreNbr == immunizationRow.StrFacilityId &&
                        cps.RxNbr == immunizationRow.StrRxNumber &&
                        cps.RefillNbr == immunizationRow.RefillNumber &&
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
                        s.KeyDesc == "Native American or Alaska Native" && s.KeyValue == "true" ? "1002-5" :
                        s.KeyDesc == "Asian" && s.KeyValue == "true" ? "2028-9" :
                        s.KeyDesc == "Native Hawaiian or Other Pacific Islander" && s.KeyValue == "true" ? "2076-8" :
                        s.KeyDesc == "Black or African American" && s.KeyValue == "true" ? "2054-5" :
                        s.KeyDesc == "White" && s.KeyValue == "true" ? "2106-3" :
                        s.KeyDesc == "Other" && s.KeyValue == "true" ? "2131-1" :
                        "UNK"
                    )
                    .FirstOrDefault();
        }

        public string? GetRecipEthnicityWherePatientNumberEqualsDecInternalPtNum(IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows, ImmunizationRow immunizationRow)
        {
            return immunizationQuestionnaireRows
                    .Where(cps =>
                        cps.PatientNum == immunizationRow.DecInternalPtNum &&
                        cps.StoreNbr == immunizationRow.StrFacilityId &&
                        cps.RxNbr == immunizationRow.StrRxNumber &&
                        cps.RefillNbr == immunizationRow.RefillNumber &&
                        cps.KeyDesc == "Ethnicity"
                    )
                    .Select(s =>
                        s.KeyValue == "Hispanic or Latino" ? "2135-2" :
                        s.KeyValue == "Not Hispanic or Latino" ? "2186-5" :
                        "UNK"
                    )
                    .FirstOrDefault();
        }

        public string? GetNyPriorityGroupWherePatientNumberEqualsDecInternalPtNum(IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows, ImmunizationRow immunizationRow)
        {
            return immunizationQuestionnaireRows
                        .Where(cps =>
                            cps.PatientNum == immunizationRow.DecInternalPtNum &&
                            cps.StoreNbr == immunizationRow.StrFacilityId &&
                            cps.RxNbr == immunizationRow.StrRxNumber &&
                            cps.RefillNbr == immunizationRow.RefillNumber &&
                            cps.KeyDesc == "COVID Priority Group - NY Stores ONLY"
                        )
                        .Select(s =>
                            s.KeyValue == "Age" ? "AGE" :
                            s.KeyValue == "Age 65-74" ? "65-74" :
                            s.KeyValue == "Age 75 or above" ? "75+" :
                            s.KeyValue == "Healthcare Provider - Other" ? "HCPOTHER" :
                            s.KeyValue == "Frontline Worker (agriculture, USPS, grocery, education, public transportation, manufacturing)" ? "FRONTLINE" :
                            s.KeyValue == "Healthcare - Ambulatory Staff" ? "HCPAMB" :
                            s.KeyValue == "EMS Healthcare worker" ? "HCPEMS" :
                            s.KeyValue == "Healthcare Professional - hospital staff" ? "HCPHOSP" :
                            s.KeyValue == "Medical examiner, coroner, mortician" ? "HCPME" :
                            s.KeyValue == "Long Term Care - Facility Staff" ? "LTCHCP" :
                            s.KeyValue == "Long Term Care - Resident" ? "LTCRES" :
                            s.KeyValue == "Other Essential Worker" ? "OTHESSENTIAL" :
                            s.KeyValue == "Public Safety - fire, police, corrections" ? "PUBSAFE" :
                            s.KeyValue == "Congregate Care Resident (not Long Term Care)" ? "RESCONG" :
                            s.KeyValue == "Under age 65 with health conditions" ? "U65HEALTH" :
                            s.KeyValue == "3rd Dose - Health Reason" ? "3HEALTH" :
                            ""
                        )
                        .FirstOrDefault();
        }

        public string? GetPrimaryLanguage(ImmunizationRow immunizationRow)
        {
            return immunizationRow.PrimaryLanguage == "ENGLISH" ? "ENG" : "UNK";
        }

        public string? GetRecipEthnicity(ImmunizationRow immunizationRow)
        {
            return  string.IsNullOrEmpty(immunizationRow.RecipEthnicity)? "UNK" : immunizationRow.RecipEthnicity;
        }

        public string? GetRecipRaceOne(ImmunizationRow immunizationRow)
        {
            return string.IsNullOrEmpty(immunizationRow.RecipRaceOne) ? "UNK" : immunizationRow.RecipRaceOne;
        }
    }
}
