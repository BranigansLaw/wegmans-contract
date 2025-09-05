

//employee_number,org_location,check_sequence,check_number,business_date,open_time,close_time,is_take_out,total_amount,food_amount,
////
namespace RX.PharmacyBusiness.ETL.GMR512_RX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class GMSRecord : IRecord
    {
        public int employee_number { get; set; }
        public string org_location { get; set; }
        public int check_sequence { get; set; }
        public int check_number { get; set; }
        public DateTime business_date { get; set; }
        public DateTime open_time { get; set; }
        public DateTime close_time { get; set; }
        public int is_take_out { get; set; }
        public decimal total_amount { get; set; }
        public decimal food_amount { get; set; }
        public decimal beer_amount { get; set; }
        public decimal wine_amount { get; set; }
        public decimal liquor_amount { get; set; }
        public decimal misc_amount { get; set; }
        public decimal charge_tip_amount { get; set; }
        public decimal service_charge_tip_amount { get; set; }
        public int external_revenue_center_id { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            var constraintViolations = new StringBuilder();

            if (!int.TryParse(fields.ElementAt(0), out int temp_employee_number))
                constraintViolations.AppendLine("employee_number is not an integer.");

            if (!int.TryParse(fields.ElementAt(2), out int temp_check_sequence))
                constraintViolations.AppendLine("check_sequence is not an integer.");

            if (!int.TryParse(fields.ElementAt(3), out int temp_check_number))
                constraintViolations.AppendLine("check_number is not an integer.");

            if (!DateTime.TryParseExact(
                    fields.ElementAt(4),
                    "M/dd/yyyy h:mm:ss tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime temp_business_date))
                constraintViolations.AppendLine("business_date is not a DateTime.");

            if (!DateTime.TryParseExact(
                    fields.ElementAt(5),
                    "M/dd/yyyy h:mm:ss tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime temp_open_time))
                constraintViolations.AppendLine("open_time is not a DateTime.");

            if (!DateTime.TryParseExact(
                    fields.ElementAt(6),
                    "M/dd/yyyy h:mm:ss tt",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime temp_close_time))
                constraintViolations.AppendLine("close_time is not a DateTime.");

            if (!int.TryParse(fields.ElementAt(7), out int temp_is_take_out))
                constraintViolations.AppendLine("is_take_out is not an integer.");

            if (!decimal.TryParse(fields.ElementAt(8), out decimal temp_total_amount))
                constraintViolations.AppendLine("total_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(9), out decimal temp_food_amount))
                constraintViolations.AppendLine("food_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(10), out decimal temp_beer_amount))
                constraintViolations.AppendLine("beer_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(11), out decimal temp_wine_amount))
                constraintViolations.AppendLine("wine_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(12), out decimal temp_liquor_amount))
                constraintViolations.AppendLine("liquor_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(13), out decimal temp_misc_amount))
                constraintViolations.AppendLine("misc_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(14), out decimal temp_charge_tip_amount))
                constraintViolations.AppendLine("charge_tip_amount is not a decimal.");

            if (!decimal.TryParse(fields.ElementAt(15), out decimal temp_service_charge_tip_amount))
                constraintViolations.AppendLine("service_charge_tip_amount is not a decimal.");

            if (!int.TryParse(fields.ElementAt(16), out int temp_external_revenue_center_id))
                constraintViolations.AppendLine("external_revenue_center_id is not an integer.");

            if (constraintViolations.Length > 0)
            {
                throw new FormatException(constraintViolations.ToString());
            }
            else
            {
                this.employee_number = temp_employee_number;
                this.org_location = fields.ElementAt(1);
                this.check_sequence = temp_check_sequence;
                this.check_number = temp_check_number;
                this.business_date = temp_business_date;
                this.open_time = temp_open_time;
                this.close_time = temp_close_time;
                this.is_take_out = temp_is_take_out;
                this.total_amount = temp_total_amount;
                this.food_amount = temp_food_amount;
                this.beer_amount = temp_beer_amount;
                this.wine_amount = temp_wine_amount;
                this.liquor_amount = temp_liquor_amount;
                this.misc_amount = temp_misc_amount;
                this.charge_tip_amount = temp_charge_tip_amount;
                this.service_charge_tip_amount = temp_service_charge_tip_amount;
                this.external_revenue_center_id = temp_external_revenue_center_id;

            }
        }
    }
}