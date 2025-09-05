namespace RX.PharmacyBusiness.ETL.RXS516.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class NotificationsSent : IRecord
    {
        public int store_num { get; set; }
        public int pos_exceptions_count { get; set; }
        public string datetime_sent { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            var constraintViolations = new StringBuilder();

            if (!int.TryParse(fields.ElementAt(0), out int temp_store_num))
                constraintViolations.AppendLine("store_num is not an integer.");

            if (!int.TryParse(fields.ElementAt(1), out int temp_pos_exceptions_count))
                constraintViolations.AppendLine("pos_exceptions_count is not an integer.");

            if (constraintViolations.Length > 0)
            {
                throw new FormatException(constraintViolations.ToString());
            }
            else
            {
                this.store_num = temp_store_num;
                this.pos_exceptions_count = temp_pos_exceptions_count;
                this.datetime_sent = fields.ElementAt(2).ToString();
            }
        }
    }
}
