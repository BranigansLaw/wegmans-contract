namespace RX.PharmacyBusiness.ETL.RXS578.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class LogileMatrixRecord : IRecord
    {
        public string Scenario { get; set; }
        public DateTime EffectiveAsOfDate { get; set; }
        public DateTime IneffectiveAsOfDate { get; set; }
        public string LOGILECode { get; set; }
        public string LOGILEDescription { get; set; }
        public int LOGILEEarnedSeconds { get; set; }
        public string CPSProgramName { get; set; }
        public string CPSKeyDesc { get; set; }
        public string CPSKeyValue { get; set; }
        public string CPSStatusName { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            var constraintViolations = new StringBuilder();

            if (!DateTime.TryParseExact(
                    fields.ElementAt(1),
                    "yyyyMMdd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime tempEffectiveAsOfDate))
                constraintViolations.AppendLine("Effective As Of Date is not a DateTime.");

            DateTime tempIneffectiveAsOfDate = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(fields.ElementAt(2)))
                if (!DateTime.TryParseExact(
                        fields.ElementAt(2),
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out tempIneffectiveAsOfDate))
                    constraintViolations.AppendLine("Ineffective As Of Date is not a DateTime.");

            if (!int.TryParse(fields.ElementAt(5), out int tempEarnedSeconds))
                constraintViolations.AppendLine("LOGILE Earned Seconds is not an integer.");

            if (constraintViolations.Length > 0)
            {
                throw new FormatException(constraintViolations.ToString());
            }
            else
            {
                this.Scenario = fields.ElementAt(0).Trim();
                this.EffectiveAsOfDate = tempEffectiveAsOfDate;
                this.IneffectiveAsOfDate = tempIneffectiveAsOfDate;
                this.LOGILECode = fields.ElementAt(3).Trim();
                this.LOGILEDescription = fields.ElementAt(4).Trim();
                this.LOGILEEarnedSeconds = tempEarnedSeconds;
                this.CPSProgramName = fields.ElementAt(6).Trim();
                this.CPSKeyDesc = fields.ElementAt(7).Trim();
                this.CPSKeyValue = fields.ElementAt(8).Trim();
                this.CPSStatusName = fields.ElementAt(9).Trim();
            }
        }
    }
}
