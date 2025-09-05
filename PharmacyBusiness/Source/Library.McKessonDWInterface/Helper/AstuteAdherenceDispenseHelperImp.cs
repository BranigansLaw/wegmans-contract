using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;
using System.Globalization;

namespace Library.McKessonDWInterface.Helper
{
    public class AstuteAdherenceDispenseHelperImp : IAstuteAdherenceDispenseHelper
    {
        // Action Type Codes
        private const string MULTIPLE_CASES_FOUND = "Dispense Applied To Multiple Cases";
        private const string PATIENT_NOT_FOUND = "patient not found";
        private const string ID_NOT_FOUND = "id not found";
        private const string DOB_NOT_FOUND = "DOB Not Found";
        private const string NAME_DOB_NOT_FOUND = "Patient Name DOB Not Found";
        private const string PROGRAM_HEADER_MISMATCH = "Program Header Mismatch";

        // Program Types
        private const string XARELTO_PROGRAM_TYPE = "Janssen Select - Xarelto";
        private const string TRADITIONAL_PROGRAM_TYPE = "Traditional Specialty";
        private const string JPAP_PROGRAM_TYPE = "JPAP";
        private const string ONCOLOGY_PROGRAM_TYPE = "Janssen Oncology Voucher";
        private const string DELAY_AND_DENIAL_PROGRAM_TYPE = "Janssen Oncology Delay & Denial";

        private readonly string[] facility198OnlyPrograms = new string[] { XARELTO_PROGRAM_TYPE, JPAP_PROGRAM_TYPE, ONCOLOGY_PROGRAM_TYPE, DELAY_AND_DENIAL_PROGRAM_TYPE };
        private readonly string[] setNewReferralStatusPrograms = new string[] { JPAP_PROGRAM_TYPE, DELAY_AND_DENIAL_PROGRAM_TYPE };
        private readonly string[] setAddressPrograms = new string[] { XARELTO_PROGRAM_TYPE, JPAP_PROGRAM_TYPE, ONCOLOGY_PROGRAM_TYPE, DELAY_AND_DENIAL_PROGRAM_TYPE };

        // Patient ID Types
        private const string XARELTO_PATIENT_ID_TYPE = "Janssen Select ID";
        private const string JPAP_PATIENT_ID_TYPE = "JPAP CarePath Patient ID";
        private const string ONCOLOGY_PATIENT_ID_TYPE = "Oncology CarePath Patient ID";
        private const string DELAY_AND_DENIAL_PATIENT_ID_TYPE = "ONC Delay Denial CarePath Patient ID";

        private const string DISPENSE_LOGIC_MISMATCH = "Dispense Logic Mismatch";
        private const string PATIENT_ADDRESS_TYPE = "PATIENT";
        private const string CASE_TEXT_DESCRIPTION = "2";
        private const string ISSUE_SOURCE = "Dispense (Erx)";
        private const string NO_DISPENSE = "NO DISPENSE";
        private const string ADMIN_QUEUE_USER = "adminqueue";
        private const string CLOSED = "C";
        private const string OPEN = "O";
        private const string REP_NOTES_TYPE = "Rep Notes";
        private const string CLINICAL_QUESTION_DECLINED = "Clinical/Qol - Declined";
        private const string NEW_REFERRAL_PATIENT_STATUS = "P1=New Referral";
        private const string SHIPPED_PATIENT_STATUS = "A1=Shipped";
        private const string SHIPPED_WORKFLOW_STATUS = "Call Complete-Shipped";

        private readonly string[] programHeadersToExclude = new string[] { "RPh Only", "Patient Outreach" };
        private readonly string[] reshipPlanCodesToExclude = new string[] { "OTHJVR", "OTHJLR", "OTHJSR", "OTHORN", "OTHORR", "OTHZYR", "OTHOVR" };
        private readonly string[] jpapReshipPlanCodes = new string[] { "OTHPAR" };

        private readonly string[] traditionalNdcExclusions = new string[] {
            "57894016001",
            "57894003001",
            "57894035001",
            "57894005427",
            "50458056001",
            "50458056101",
            "50458056201",
            "50458056301",
            "50458056401",
            "50458060601",
            "50458060701",
            "50458060801",
            "50458060901",
            "50458061101",
            "50458061201",
            "50458002802",
            "50458002803",
            "50458070714",
            "50458072030",
            "50458058451",
            "50458057989",
            "59676060012",
            "59676060430",
            "57894005060",
            "57894010060",
            "59676003084",
            "59676004056",
            "59676003056",
            "59676005028",
            "59676004028",
            "57894050205",
            "57894050220",
            "57894050301",
            "59676027801",
            "50458009801",
            "59676057001",
            "59676057101",
            "59676057201",
            "50458054360",
            "50458054260",
            "50458054160",
            "50458054060",
            "50458094001",
            "50458094101",
            "50458094201",
            "50458094301",
            "50458014030",
            "50458014090",
            "50458014130",
            "50458014190",
            "59676057530",
            "59676056501",
            "59676056301",
            "59676056401",
            "59676056630",
            "59676056201",
            "57894050101",
            "59676070101",
            "59676070260",
            "59676080030",
            "57894046901",
            "57894047001",
            "57894044901",
            "57894045001",
            "50458057830",
            "50458057930",
            "50458058030",
            "50458057760",
            "59676061001",
            "57894015012",
            "57894019506",
            "57894024030",
            "57894008060",
            "50458030911",
            "50458030611",
            "50458030711",
            "50458030811",
            "57894050505",
            "57894050520"
        };

        private const string JPAP = "JPAP";
        private readonly string[] jpapPlanCodes = new string[] { "OTHEPAON", "OTHEPAOR", "OTHPAR" };
        private readonly string[] delayAndDenialPlanCodes = new string[] { "OTHTREMLIN" };
        private readonly string[] jpapOneDayFollowupBaseProductNames = new string[] { "SPRAVATO", "TECVAYLI", "TALVEY" };
        private readonly string[] oncologyBaseProductNames = new string[] { "ERLEADA", "AKEEGA", "ZYTIGA" };
        private readonly int[] mailStores = new int[] { 198, 199 };

        /// <inheritdoc />
        public int DeriveStoreNumber(
            string facilityId)
        {
            return int.Parse(facilityId);
        }

        /// <inheritdoc />
        public DateTime? DeriveFillDate(
            string programType,
            string baseProductName,
            DateTime soldDate,
            double daysSupply)
        {
            if (programType == JPAP_PROGRAM_TYPE)
            {
                if (jpapOneDayFollowupBaseProductNames.Contains(baseProductName, StringComparer.OrdinalIgnoreCase))
                {
                    return (soldDate.AddDays(daysSupply));
                }

                return (soldDate.AddDays(daysSupply - 6));
            }

            return default;
        }

        /// <inheritdoc />
        public string DeriveProgramHeader(
            string programType,
            string drugNdc,
            string baseProductName,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows)
        {
            if (programType == JPAP_PROGRAM_TYPE)
            {
                //Remove hard-coded NDCs below.
                //Compare NDC from McKesson and compare to Complete Specialty Item List, and get Program Header from that.
                CompleteSpecialtyItemRow? completeSpecialtyItemRow = completeSpecialtyItemRows.Where(r => r.NdcWo == drugNdc).FirstOrDefault();
                if (completeSpecialtyItemRow?.ProgramHeader is not null)
                {
                    return completeSpecialtyItemRow.ProgramHeader;
                }
            }
            return baseProductName;
        }

        /// <inheritdoc />
        public string DeriveProgramType(
            string planCode,
            string baseProductName,
            int storeNumber)
        {
            if (jpapPlanCodes.Contains(planCode, StringComparer.OrdinalIgnoreCase))
                return JPAP_PROGRAM_TYPE;

            if (delayAndDenialPlanCodes.Contains(planCode, StringComparer.OrdinalIgnoreCase))
                return DELAY_AND_DENIAL_PROGRAM_TYPE;

            if (baseProductName == "XARELTO" && mailStores.Contains(storeNumber))
                return XARELTO_PROGRAM_TYPE;

            if (oncologyBaseProductNames.Contains(baseProductName, StringComparer.OrdinalIgnoreCase) && storeNumber == 198)
                return ONCOLOGY_PROGRAM_TYPE;

            return TRADITIONAL_PROGRAM_TYPE;
        }

        /// <inheritdoc />
        public string DeriveBaseProductName(
            string productName)
        {
            if (productName.IndexOf(" ") >= 0)
                return productName.Substring(0, productName.IndexOf(" "));

            return productName;
        }

        /// <inheritdoc />
        public bool DeriveTitrationDoseFlag(
            string patientGroupName,
            string drugNdc,
            string programHeader,
            double daysSupply)
        {
            string[] validNdcs = ["50458070714", "57894050205", "57894050505", "57894050220", "57894050301", "57894050101"];
            bool meetsNdcCriteria = !string.IsNullOrEmpty(drugNdc) && validNdcs.Contains(drugNdc);

            if (meetsNdcCriteria && patientGroupName.Contains(JPAP))
                return false;

            if (meetsNdcCriteria && (string.IsNullOrEmpty(patientGroupName) || !patientGroupName.Contains(JPAP)))
                return true;

            return false;
        }

        /// <inheritdoc />
        public DateTime DeriveCallDate(
            DateTime soldDateTime, 
            double daysSupply, 
            string programType, 
            string baseProductName,
            bool titrationDoseFlag,
            decimal totalRefillsRemaining)
        {
            if (programType == XARELTO_PROGRAM_TYPE)
            {
                return soldDateTime.AddDays(daysSupply - 6);
            }
            if (programType == JPAP_PROGRAM_TYPE)
            {
                if (!string.IsNullOrEmpty(baseProductName) && jpapOneDayFollowupBaseProductNames.Contains(baseProductName, StringComparer.OrdinalIgnoreCase))
                {
                    return soldDateTime.AddDays(1); // JPAP dispenses for these drugs need a follow-up 1 day after the ship date
                }
                if (titrationDoseFlag) //OLD: || daysSupply % 7 != 0)
                {
                    return soldDateTime.AddDays(7);
                }
                if (totalRefillsRemaining == 0)
                {
                    return soldDateTime.AddDays(daysSupply - 15);
                }

                return (soldDateTime.AddDays(daysSupply - 6));
            }

            return soldDateTime.AddDays(daysSupply - 7);
        }

        /// <inheritdoc />
        public string DerivePatientIdType(
            string programType)
        {
            string returnPatientIdType = string.Empty;

            // Determine patient ID type, if any, for the specialty program
            switch (programType)
            {
                case XARELTO_PROGRAM_TYPE:
                    returnPatientIdType = XARELTO_PATIENT_ID_TYPE;
                    break;
                case JPAP_PROGRAM_TYPE:
                    returnPatientIdType = JPAP_PATIENT_ID_TYPE;
                    break;
                case ONCOLOGY_PROGRAM_TYPE:
                    returnPatientIdType = ONCOLOGY_PATIENT_ID_TYPE;
                    break;
                case DELAY_AND_DENIAL_PROGRAM_TYPE:
                    returnPatientIdType = DELAY_AND_DENIAL_PATIENT_ID_TYPE;
                    break;
                default:
                    break;
            }

            return returnPatientIdType;
        }

        /// <inheritdoc />
        public int DeriveFillNumber(
            int refillNumber)
        {
            return refillNumber + 1;
        }

        /// <inheritdoc />
        public DateTime DeriveSoldDateTime(
            DateTime soldDate,
            string? soldTime)
        {
            if (!string.IsNullOrEmpty(soldTime))
            {
                string dateString = soldDate.ToString("MMddyyyy") + soldTime;
                if (DateTime.TryParseExact(dateString, "MMddyyyyHH:mm:ss",
                    CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }

            }

            return soldDate;
        }

        /// <inheritdoc />
        public string DeriveCrmProductName(
            string productName,
            string baseProductName)
        {
            if (productName.Contains(NO_DISPENSE))
            {
                return string.Format("{0} {1}", baseProductName, NO_DISPENSE);
            }

            return productName;
        }

        /// <inheritdoc />
        public string DeriveNextWorkflowStatus(
            string programType,
            bool titrationDoseFlag,
            double daysSupply,
            string baseProductName,
            decimal totalRefillsRemaining)
        {
            if (programType == JPAP)
            {
                if (titrationDoseFlag)
                {
                    return "Titration Dose Review Required";
                }
    
                if (totalRefillsRemaining == 0)
                {
                    return "Active CM";
                }
                return "In Process/Workflow";
            }
            if (programType == DELAY_AND_DENIAL_PROGRAM_TYPE && totalRefillsRemaining == 0)
            {
                return "Active CM";
            }

            return "Patient Follow-Up";
        }

        /// <inheritdoc />
        public bool ShouldSkipDispense(
            string programType,
            int storeNumber,
            decimal patientPricePaid,
            string planCode,
            string drugNdc,
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows)
        {
            // Needs Verification
            if (specialtyDispenseExclusionRows.Any(r => (r.NdcWo == drugNdc)) && programType == TRADITIONAL_PROGRAM_TYPE)
                return true;

            // Its specialty program is only dispensed out of facility 198, but it was not dispensed there
            if (facility198OnlyPrograms.Contains(programType, StringComparer.OrdinalIgnoreCase) && storeNumber != 198)
                return true;

            // It's a Xarelto drug with patient_price_paid = 0, which indicates a re-shipment (this is an alternative to using secondary plan code)
            if (programType == XARELTO_PROGRAM_TYPE && patientPricePaid == 0)
                return true;

            // Its primary plan code is in the re-shipment exclusion list
            if (!string.IsNullOrEmpty(planCode) && reshipPlanCodesToExclude.Contains(planCode, StringComparer.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
