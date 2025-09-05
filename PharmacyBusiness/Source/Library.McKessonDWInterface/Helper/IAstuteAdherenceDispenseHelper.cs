using Library.TenTenInterface.DataModel;

namespace Library.McKessonDWInterface.Helper
{
    public interface IAstuteAdherenceDispenseHelper
    {
        /// <summary>
        /// Derive the Store Number for the Astute Adherence Dispense
        /// </summary>
        /// <param name="facilityId"></param>
        /// <returns></returns>
        int DeriveStoreNumber(
            string facilityId);

        /// <summary>
        /// Derive the Fill Date for the Astute Adherence Dispense
        /// </summary>
        /// <param name="programType"></param>
        /// <param name="baseProductName"></param>
        /// <param name="soldDate"></param>
        /// <param name="daysSupply"></param>
        /// <returns></returns>
        DateTime? DeriveFillDate(
            string programType,
            string baseProductName,
            DateTime soldDate,
            double daysSupply);

        /// <summary>
        /// Derive the Program Header for the Astute Adherence Dispense
        /// </summary>
        /// <param name="programType"></param>
        /// <param name="drugNdc"></param>
        /// <param name="baseProductName"></param>
        /// <returns></returns>
        string DeriveProgramHeader(
            string programType,
            string drugNdc,
            string baseProductName,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);

        /// <summary>
        /// Derive the Program Type for the Astute Adherence Dispense
        /// </summary>
        /// <param name="planCode"></param>
        /// <param name="baseProductName"></param>
        /// <param name="storeNumber"></param>
        /// <returns></returns>
        string DeriveProgramType(
            string planCode,
            string baseProductName,
            int storeNumber);

        /// <summary>
        /// Derive the Base Product Name for the Astute Adherence Dispense
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        string DeriveBaseProductName(
            string productName);

        /// <summary>
        /// Derive the Titration Dose Flag for the Astute Adherence Dispense
        /// </summary>
        /// <param name="patientGroupName"></param>
        /// <param name="drugNdc"></param>
        /// <param name="programHeader"></param>
        /// <param name="daysSupply"></param>
        /// <returns></returns>
        bool DeriveTitrationDoseFlag(
            string patientGroupName,
            string drugNdc,
            string programHeader,
            double daysSupply);

        /// <summary>
        /// Derive the Call Date for the Astute Adherence Dispense
        /// </summary>
        /// <param name="soldDateTime"></param>
        /// <param name="daysSupply"></param>
        /// <param name="programType"></param>
        /// <param name="baseProductName"></param>
        /// <param name="titrationDoseFlag"></param>
        /// <param name="totalRefillsRemaining"></param>
        /// <returns></returns>
        DateTime DeriveCallDate(
            DateTime soldDateTime,
            double daysSupply,
            string programType,
            string baseProductName,
            bool titrationDoseFlag,
            decimal totalRefillsRemaining);

        /// <summary>
        /// Derive the Patient Id Type for the Astute Adherence Dispense.
        /// </summary>
        /// <param name="programType"></param>
        /// <returns></returns>
        string DerivePatientIdType(
            string programType);

        /// <summary>
        /// Derive the Fill Number for the Astute Adherence Dispense.
        /// </summary>
        /// <param name="refillNumber"></param>
        /// <returns></returns>
        int DeriveFillNumber(
            int refillNumber);

        /// <summary>
        /// Derive the Sold Date Time for the Astute Adherence Dispense.
        /// </summary>
        /// <param name="soldDate"></param>
        /// <param name="soldTime"></param>
        /// <returns></returns>
        DateTime DeriveSoldDateTime(
            DateTime soldDate,
            string? soldTime);

        /// <summary>
        /// Derive the Crm Product Name for the Astute Adherence Dispense.
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="baseProductName"></param>
        /// <returns></returns>
        string DeriveCrmProductName(
            string productName,
            string baseProductName);

        /// <summary>
        /// Derive the Next Workflow Status for the Astute Adherence Dispense.
        /// </summary>
        /// <param name="programType"></param>
        /// <param name="titrationDoseFlag"></param>
        /// <param name="daysSupply"></param>
        /// <param name="baseProductName"></param>
        /// <param name="totalRefillsRemaining"></param>
        /// <returns></returns>
        string DeriveNextWorkflowStatus(
            string programType,
            bool titrationDoseFlag,
            double daysSupply,
            string baseProductName,
            decimal totalRefillsRemaining);

        /// <summary>
        /// Determine if the Astute Adherence Dispense should be skipped.
        /// </summary>
        /// <param name="programType"></param>
        /// <param name="storeNumber"></param>
        /// <param name="patientPricePaid"></param>
        /// <param name="planCode"></param>
        /// <param name="drugNdc"></param>
        /// <returns></returns>
        bool ShouldSkipDispense(
            string programType,
            int storeNumber,
            decimal patientPricePaid,
            string planCode,
            string drugNdc,
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows);
    }
}
