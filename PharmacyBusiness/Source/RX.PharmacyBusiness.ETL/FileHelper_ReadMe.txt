
FileHelper README
---------------------

(1) Instantiate the FileHelper like this

            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string inputLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX542\Input\");
            string outputLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX542\Output\");
            string archiveLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX542\Archive\");
            string rejectLocation = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX542\Reject\");
            FileHelper fileHelper = new FileHelper(runDate, inputLocation, outputLocation, archiveLocation, rejectLocation);

(2) Here is sample code reading multiple files with the same RunDate into a List<T>.

            Log.LogInfo("STEP 1 of X: Read input files from Controllers (EDW Team) containing brick and mortar sales for the run date.");
            DelimitedStreamReaderOptions brickAndMortarPosFileOptions = new DelimitedStreamReaderOptions(
                Constants.CharComma,
                new Nullable<char>(),
                true,
                false,
                12,
                1
            );
            //NOTE: An example daily expected file name from EDW is [erxtra_20210307.txt]
            //      On rare occasion there might be a supplemental file of records missed in that first file of the day,
            //      and in that situation the supplemental file might be named [erxtra_20210307_012.txt] with "_012" representing store 12 pos data.
            //      So, by putting in an asterisk in the file name matching pattern, like this [erxtra_20210307*.txt]
            //      then the additional file will automatically be included without any additional code changes.
            List<BrickAndMortarPosRecord> brickAndMortarPosRecords = fileHelper.ReadFilesToList<BrickAndMortarPosRecord>(
                "erxtra_" + runDate.ToString("yyyyMMdd") + "*.txt", 
                brickAndMortarPosFileOptions);

(3) Here is the class of the BrickAndMortarPosRecord object, which needs the interface IRecord.

namespace RX.PharmacyBusiness.ETL.CRX542.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class BrickAndMortarPosRecord : IRecord
    {	
        public int Store_Num { get; set; }
        public int Terminal_Num { get; set; }
        public DateTime Transaction_Date_Time { get; set; }
        public int Operator { get; set; }
        public int Transaction_Num { get; set; }
        public int Rx_Transaction_Num { get; set; }
        public int Refill_Num { get; set; }
        public string POS_TxType { get; set; }
        public decimal Total_Price { get; set; }
        public decimal Insurance_Amt { get; set; }
        public decimal Copay_Amt { get; set; }
        public int Partial_Fill_Sequence_Num { get; set; }
        public string Mail_Flag { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            var constraintViolations = new StringBuilder();
            //Oracle data type NUMBER(9,2) where 9 is precision (total number of digits) and scale is 2 (number of digits to the right of the decimal point).
            decimal constraintPrecision9Scale2 = 9999999.99M;

            if (!int.TryParse(fields.ElementAt(0), out int tempStore_Num))
                constraintViolations.AppendLine("Store_Num is not an integer.");

            if (!int.TryParse(fields.ElementAt(1), out int tempTerminal_Num))
                constraintViolations.AppendLine("Terminal_Num is not an integer.");

            if (!DateTime.TryParseExact(
                    fields.ElementAt(2),
                    "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime tempTransaction_Date_Time))
                constraintViolations.AppendLine("Transaction_Date_Time is not a DateTime.");

            if (!int.TryParse(fields.ElementAt(3), out int tempOperator))
                constraintViolations.AppendLine("Operator is not an integer.");

            if (!int.TryParse(fields.ElementAt(4), out int tempTransaction_Num))
                constraintViolations.AppendLine("Transaction_Num is not an integer.");

            if (!int.TryParse(fields.ElementAt(5), out int tempRx_Transaction_Num))
                constraintViolations.AppendLine("Rx_Transaction_Num is not an integer.");

            if (!int.TryParse(fields.ElementAt(6), out int tempRefill_Num))
                constraintViolations.AppendLine("Refill_Num is not an integer.");

            string tempPOS_TxType = string.Empty;
            if (string.IsNullOrEmpty(fields.ElementAt(7)))
                constraintViolations.AppendLine("POS_TxType cannot be blank.");
            else if (fields.ElementAt(7).Length > 1)
                constraintViolations.AppendLine("POS_TxType is longer than the max 25 characters.");
            else
                tempPOS_TxType = fields.ElementAt(7);

            if (!decimal.TryParse(fields.ElementAt(8), out decimal tempTotal_Price))
                constraintViolations.AppendLine("Total_Price is not a decimal.");
            else
            {
                if (tempTotal_Price > constraintPrecision9Scale2)
                    constraintViolations.AppendLine(string.Format("Total_Price value is greater than limit of {0}.", constraintPrecision9Scale2));

                decimal scale = 100 * (tempTotal_Price - Math.Floor(tempTotal_Price));
                if (scale - Math.Floor(scale) > 0)
                    constraintViolations.AppendLine("Total_Price cannot have more than two decimal places for the cents portion of the dollar value.");
            }

            if (!decimal.TryParse(fields.ElementAt(9), out decimal tempInsurance_Amt))
                constraintViolations.AppendLine("Insurance_Amt is not a decimal.");
            else
            {
                if (tempInsurance_Amt > constraintPrecision9Scale2)
                    constraintViolations.AppendLine(string.Format("Insurance_Amt value is greater than limit of {0}.", constraintPrecision9Scale2));

                decimal scale = 100 * (tempInsurance_Amt - Math.Floor(tempInsurance_Amt));
                if (scale - Math.Floor(scale) > 0)
                    constraintViolations.AppendLine("Insurance_Amt cannot have more than two decimal places for the cents portion of the dollar value.");
            }

            if (!decimal.TryParse(fields.ElementAt(10), out decimal tempCopay_Amt))
                constraintViolations.AppendLine("Copay_Amt is not a decimal.");
            else
            {
                if (tempCopay_Amt > constraintPrecision9Scale2)
                    constraintViolations.AppendLine(string.Format("Copay_Amt value is greater than limit of {0}.", constraintPrecision9Scale2));

                decimal scale = 100 * (tempCopay_Amt - Math.Floor(tempCopay_Amt));
                if (scale - Math.Floor(scale) > 0)
                    constraintViolations.AppendLine("Copay_Amt cannot have more than two decimal places for the cents portion of the dollar value.");
            }

            if (!int.TryParse(fields.ElementAt(11), out int tempPartial_Fill_Sequence_Num))
                constraintViolations.AppendLine("Partial_Fill_Sequence_Num is not an integer.");

            if (constraintViolations.Length > 0)
            {
                throw new FormatException(constraintViolations.ToString());
            }
            else
            {
                this.Store_Num = tempStore_Num;
                this.Terminal_Num = tempTerminal_Num;
                this.Transaction_Date_Time = tempTransaction_Date_Time;
                this.Operator = tempOperator;
                this.Transaction_Num = tempTransaction_Num;
                this.Rx_Transaction_Num = tempRx_Transaction_Num;
                this.Refill_Num = tempRefill_Num;
                this.POS_TxType = tempPOS_TxType;
                this.Total_Price = tempTotal_Price;
                this.Insurance_Amt = tempInsurance_Amt;
                this.Copay_Amt = tempCopay_Amt;
                this.Partial_Fill_Sequence_Num = tempPartial_Fill_Sequence_Num;
                this.Mail_Flag = "N";
            }
        }
    }
}





