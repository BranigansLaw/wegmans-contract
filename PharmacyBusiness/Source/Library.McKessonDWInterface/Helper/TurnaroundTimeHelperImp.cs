namespace Library.McKessonDWInterface.Helper
{
    public class TurnaroundTimeHelperImp : ITurnaroundTimeHelper
    {
        public const string TAT_Target_SPECIALTY = "SPECIALTY";
        public const string TAT_Target_EXCELLUS = "EXCELLUS";
        public const string TAT_Target_IHA = "IHA";

        /// <inheritdoc />
        public bool DeriveIsIntervention(string wfsdDescription, string tatTarget)
        {
            if (wfsdDescription == "Adjudication" ||
                wfsdDescription == "Fill on Arrival" ||
                wfsdDescription == "General Exception" ||
                wfsdDescription == "Payment Complete Exception")
                return true;

            if (wfsdDescription == "Contact Manager" &&
                (tatTarget == TAT_Target_EXCELLUS ||
                 tatTarget == TAT_Target_IHA))
                return true;

            return false;
        }

        /// <inheritdoc />
        public bool DeriveIsException(string wfsdDescription)
        {
            if (wfsdDescription == "Contact Manager")
                return true;

            return false;
        }

        /// <inheritdoc />
        public decimal DeriveDaysInStep(DateTime dateIn, DateTime dateOut)
        {
            return decimal.Round((decimal)(dateOut - dateIn).TotalDays, 6, MidpointRounding.AwayFromZero);
        }

        /// <inheritdoc />
        public decimal DeriveDaysOffHours(DateTime dateIn, DateTime dateOut)
        {
            DateTime tempDate = dateIn;
            DateTime tempDateBeginTime;
            DateTime tempDateEndTime;
            decimal tempSpanDays;
            decimal spanDaysOffHours = 0;
            decimal spanDaysWorkingHours = 0;

            while (tempDate < dateOut)
            {
                tempDateBeginTime = dateIn >= tempDate ? dateIn : tempDate.Date;
                tempDateEndTime = dateOut <= tempDate.Date.AddDays(1) ? dateOut : tempDate.Date.AddDays(1);
                tempSpanDays = (decimal)(tempDateEndTime - tempDateBeginTime).TotalDays;

                if (tempDate.DayOfWeek == DayOfWeek.Saturday ||
                    tempDate.DayOfWeek == DayOfWeek.Sunday ||
                    (tempDate.Month == 1 && tempDate.Day == 1) ||
                    (tempDate.Month == 7 && tempDate.Day == 4) ||
                    (tempDate.Month == 12 && tempDate.Day == 25))
                {
                    //Post Office closed on weekends and on holidays.
                    //Three holidays have fixed dates, and others can be added upon request.
                    spanDaysOffHours += tempSpanDays;
                }
                else
                {
                    //Regular Working Hours
                    spanDaysWorkingHours += tempSpanDays;
                }

                tempDate = tempDateEndTime;
            }

            return decimal.Round(spanDaysOffHours, 6, MidpointRounding.AwayFromZero);
        }
    }
}