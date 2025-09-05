using XXXDeveloperTools.NfmPlansAndFilesets.DataModel;

namespace XXXDeveloperTools.NfmPlansAndFilesets.Helper
{
    public interface IPopulateCollectionsHelper
    {
        /// <summary>
        /// Returns a collection of ControlMRow objects
        /// </summary>
        /// <returns></returns>
        IEnumerable<ControlMRow> PopulateControlM();

        /// <summary>
        /// Returns a collection of NfmPlanRow objects
        /// </summary>
        /// <returns></returns>
        IEnumerable<NfmPlanRow> PopulateNfmPlan();

        /// <summary>
        /// Returns a collection of NfmFilesetRow objects
        /// </summary>
        /// <returns></returns>
        IEnumerable<NfmFilesetRow> PopulateNfmFileset();

        /// <summary>
        /// Returns a collection of PgpLogRow objects
        /// </summary>
        /// <returns></returns>
        IEnumerable<PgpLogRow> PopulatePgpLog();
    }
}
