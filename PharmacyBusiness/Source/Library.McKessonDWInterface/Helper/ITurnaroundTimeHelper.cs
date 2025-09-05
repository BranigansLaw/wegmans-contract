namespace Library.McKessonDWInterface.Helper
{
    public interface ITurnaroundTimeHelper
    {
        /// <summary>
        /// This method calculates a value for derived column IsIntervention.
        /// </summary>
        /// <param name="wfsdDescription"></param>
        /// <param name="tatTarget"></param>
        /// <returns></returns>
        bool DeriveIsIntervention(string wfsdDescription, string tatTarget);

        /// <summary>
        /// This method calculates a value for derived column IsException.
        /// </summary>
        /// <param name="wfsdDescription"></param>
        /// <returns></returns>
        bool DeriveIsException(string wfsdDescription);

        /// <summary>
        /// This method calculates a value for derived column DaysInStep.
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="dateOut"></param>
        /// <returns></returns>
        decimal DeriveDaysInStep(DateTime dateIn, DateTime dateOut);

        /// <summary>
        /// This method calculates a value for derived column DaysOffHours.
        /// </summary>
        /// <param name="dateIn"></param>
        /// <param name="dateOut"></param>
        /// <returns></returns>
        decimal DeriveDaysOffHours(DateTime dateIn, DateTime dateOut);
    }
}
