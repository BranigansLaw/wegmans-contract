namespace RX.PharmacyBusiness.ETL.RXS516.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class UnresolvedPosExceptionRecord : IRecord
    {
        public int exception_date { get; set; }
        public int store_num { get; set; }
        public int rx_num { get; set; }
        public int refill_num { get; set; }
        public int part_seq_num { get; set; }
        public int classification_code { get; set; }
        public string in_wcb { get; set; }
        public DateTime sold_datetime { get; set; }
        public decimal copay { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            var constraintViolations = new StringBuilder();
            //Oracle data type NUMBER(9,2) where 9 is precision (total number of digits) and scale is 2 (number of digits to the right of the decimal point).
            decimal constraintPrecision9Scale2 = 9999999.99M;

            if (!int.TryParse(fields.ElementAt(0), out int temp_exception_date))
                constraintViolations.AppendLine("exception_date is not an integer.");

            if (!int.TryParse(fields.ElementAt(1), out int temp_store_num))
                constraintViolations.AppendLine("store_num is not an integer.");

            if (!int.TryParse(fields.ElementAt(2), out int temp_rx_num))
                constraintViolations.AppendLine("rx_num is not an integer.");

            if (!int.TryParse(fields.ElementAt(3), out int temp_refill_num))
                constraintViolations.AppendLine("refill_num is not an integer.");

            if (!int.TryParse(fields.ElementAt(4), out int temp_part_seq_num))
                constraintViolations.AppendLine("part_seq_num is not an integer.");

            if (!int.TryParse(fields.ElementAt(5), out int temp_classification_code))
                constraintViolations.AppendLine("classification_code is not an integer.");

            if (!string.IsNullOrEmpty(fields.ElementAt(6)) && fields.ElementAt(6).Length > 1)
                constraintViolations.AppendLine("in_wcb is longer than the max 1 characters.");

            if (!DateTime.TryParseExact(
                    fields.ElementAt(7),
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime temp_sold_datetime))
                constraintViolations.AppendLine("sold_datetime is not a DateTime.");

            if (!decimal.TryParse(fields.ElementAt(8), out decimal temp_copay))
                constraintViolations.AppendLine("copay is not a decimal.");
            else
            {
                if (Math.Abs(temp_copay) > constraintPrecision9Scale2)
                    constraintViolations.AppendLine(string.Format("copay value is greater than limit of {0}.", constraintPrecision9Scale2));

                decimal scale = 100 * (temp_copay - Math.Floor(temp_copay));
                if (scale - Math.Floor(scale) > 0)
                    constraintViolations.AppendLine("copay cannot have more than two decimal places for the cents portion of the dollar value.");
            }

            if (constraintViolations.Length > 0)
            {
                throw new FormatException(constraintViolations.ToString());
            }
            else
            {
                this.exception_date = temp_exception_date;
                this.store_num = temp_store_num;
                this.rx_num = temp_rx_num;
                this.refill_num = temp_refill_num;
                this.part_seq_num = temp_part_seq_num;
                this.classification_code = temp_classification_code;
                this.in_wcb = fields.ElementAt(6);
                this.sold_datetime = temp_sold_datetime;
                this.copay = temp_copay;
            }
        }
    }
}
