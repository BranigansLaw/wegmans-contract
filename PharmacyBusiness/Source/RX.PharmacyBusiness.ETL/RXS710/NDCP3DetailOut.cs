namespace RX.PharmacyBusiness.ETL.RXS710
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class NDCP3DetailOut
    {
        private const string DASH = "-";
        private const string DECIMAL_POINT = ".";
        private const char SPACE = ' ';
        private const char ZERO = '0';

        private string RecordType { get; set; }
        private string PharmacyNcpdp { get; set; }
        private string Filler4 { get; set; }
        private string DispensedDate { get; set; }
        private string DispensedNdc { get; set; }
        private string DispensedDrug { get; set; }
        private string RefillNumber { get; set; }
        private string QuantityFilled { get; set; }
        private string DaysSupply { get; set; }
        private string AcquisitionCost { get; set; }
        private string DispensingFee { get; set; }
        private string CopayAmount { get; set; }
        private string PriceOfTransaction { get; set; }
        private string PatientBirthDate { get; set; }
        private string Gender { get; set; }
        private string Filler1 { get; set; }
        private string PlanGroup { get; set; }
        private string PlanCarrierId { get; set; }
        private string DoctorDeaNumber { get; set; }
        private string DiagnosisCode { get; set; }
        private string CustomerLocation { get; set; }
        private string DateWritten { get; set; }
        private string DawCode { get; set; }
        private string DependentCode { get; set; }
        private string CompoundCode { get; set; }
        private string AuthorizedRefills { get; set; }
        private string LevelOfService { get; set; }
        private string OriginOfRx { get; set; }
        private string DrugType { get; set; }
        private string DoctorLastName { get; set; }
        private string DaysSupplyBasis { get; set; }
        private string PaymentType { get; set; }
        private string PrescriptionNumber { get; set; }
        private string PatientZipCode { get; set; }
        private string DoctorFirstName { get; set; }
        private string Bin { get; set; }
        private string ChainIndependent { get; set; }
        private string DosageInfo { get; set; }
        private string DivNumber { get; set; }
        private string PrescribedNdc { get; set; }
        private string PatientKey { get; set; }
        private string DoctorZipCode { get; set; }
        private string Filler2 { get; set; }
        private string SupplierSpecPlan { get; set; }
        private string PlanNameDescription { get; set; }
        private string StoreNpi { get; set; }
        private string PrescriberNpi { get; set; }
        private string Filler3 { get; set; }
        private string Trailer { get; set; }

        public NDCP3DetailOut() { }

        public NDCP3DetailOut(NDCP3DetailIn recordIn)
        {
            this.RecordType = "4";
            this.PharmacyNcpdp = recordIn.NcpdpNumber.TruncateValue(0, 12).PadRightAndValidate(12, SPACE);
            this.Filler4 = (string.Empty).PadRight(7); // Intentionally Blank
            this.DispensedDate = recordIn.DispenseDate.ToString("yyyyMMdd").ValidateLength(8);

            var dispensedNdc = string.Empty;
            if (recordIn.CompoundNumber is null)
            {
                dispensedNdc = recordIn.DispensedNdc.Replace(DASH, string.Empty);
            }
            else
            {
                switch (recordIn.ProductSchedule)
                {
                    case "2":
                        dispensedNdc = "99999999992";
                        break;
                    case "3":
                        dispensedNdc = "99999999993";
                        break;
                    case "4":
                        dispensedNdc = "99999999994";
                        break;
                    case "5":
                        dispensedNdc = "99999999995";
                        break;
                    default:
                        dispensedNdc = "99999999996";
                        break;
                }
            }
            this.DispensedNdc = dispensedNdc.PadLeftAndValidate(11, ZERO);

            this.DispensedDrug = recordIn.DrugDescription.TruncateValue(0, 28).PadRightAndValidate(30, SPACE);
            this.RefillNumber = recordIn.RefillNumber.ToString().PadLeftAndValidate(2, ZERO);
            this.QuantityFilled = this.ApplyMaxValue(recordIn.QuantityDispensed, 99999).ToString("00000.000").Replace(DECIMAL_POINT, string.Empty).PadLeftAndValidate(8, ZERO);
            this.DaysSupply = this.ApplyMaxValue(recordIn.DaysSupply, 999).ToString().PadLeftAndValidate(3, ZERO);
            this.AcquisitionCost = recordIn.TotalUserDefined.ToString("000000.00;-00000.00").Replace(DECIMAL_POINT, string.Empty).PadLeftAndValidate(8, ZERO);

            decimal finalPrice = 0;
            decimal totalCost = 0;
            if (recordIn.FinalPrice.HasValue)
            {
                finalPrice = recordIn.FinalPrice.Value;
            }
            if (recordIn.TotalCost.HasValue)
            {
                totalCost = recordIn.TotalCost.Value;
            }
            var dispensingFee = (finalPrice - totalCost).ToString("0000.00;-000.00").Replace(DECIMAL_POINT, string.Empty);
            // Zero out any dispensing fee values greater than the max length to work around McKesson issue
            if (dispensingFee.Length > 6)
            {
                dispensingFee = string.Empty;
            }
            this.DispensingFee = dispensingFee.PadLeftAndValidate(6, ZERO);


            this.CopayAmount = recordIn.PatientPayAmount.ToString("0000.00;-000.00").Replace(DECIMAL_POINT, string.Empty).ValidateLength(6);

            var priceOfTransaction = string.Empty;
            if (recordIn.FinalPrice.HasValue)
            {
                priceOfTransaction = recordIn.FinalPrice.Value.ToString("000000.00;-00000.00");
            }
            this.PriceOfTransaction = priceOfTransaction.Replace(DECIMAL_POINT, string.Empty).PadLeftAndValidate(8, SPACE);

            this.PatientBirthDate = recordIn.BirthDate.ToString("yyyyMMdd").ValidateLength(8);

            var gender = string.Empty;
            switch (recordIn.Gender)
            {
                case "M":
                    gender = "1";
                    break;
                case "F":
                    gender = "2";
                    break;
                default:
                    gender = "3";
                    break;
            }
            this.Gender = gender.ValidateLength(1);

            this.Filler1 = (string.Empty).PadRight(18); // Intentionally Blank
            this.PlanGroup = (recordIn.PlanGroup ?? string.Empty).PadRightAndValidate(15, SPACE);
            this.PlanCarrierId = recordIn.PlanCode.TruncateValue(0, 3).PadRightAndValidate(3, SPACE);
            this.DoctorDeaNumber = (recordIn.PrescriberDea ?? string.Empty).PadRightAndValidate(10, SPACE);
            this.DiagnosisCode = (string.Empty).PadRight(6);
            this.CustomerLocation = "00"; // Intentionally Zero
            this.DateWritten = recordIn.PrescribedDate.ToString("yyyyMMdd").ValidateLength(8);
            this.DawCode = (recordIn.DawCode ?? string.Empty).Trim().PadLeftAndValidate(1, ZERO);

            var dependentCode = string.Empty;
            if (int.TryParse(recordIn.TpPersonCode, out int tpPersonCode))
            {
                dependentCode = tpPersonCode.ToString();
            }
            this.DependentCode = dependentCode.PadLeftAndValidate(3, ZERO);

            var compoundCode = string.Empty;
            switch (recordIn.IsCompound)
            {
                case null:
                    compoundCode = "1";
                    break;
                case "Y":
                    compoundCode = "2";
                    break;
                default:
                    compoundCode = "1";
                    break;
            }
            this.CompoundCode = compoundCode.ValidateLength(1);

            this.AuthorizedRefills = this.ApplyMaxValue(recordIn.RefillsAuthorized, 99).ToString().PadLeftAndValidate(2, ZERO);
            this.LevelOfService = "00"; // Intentionally Zero
            this.OriginOfRx = "0"; // Intentionally Zero
            this.DrugType = "0"; // Intentionally Zero
            this.DoctorLastName = recordIn.PrescriberLastName.TruncateValue(0, 15).PadRightAndValidate(15, SPACE);
            this.DaysSupplyBasis = "0"; // Intentionally Zero

            var paymentType = string.Empty;
            switch (recordIn.TpPlanNum)
            {
                case null:
                case "0":
                    paymentType = "0";
                    break;
                default:
                    paymentType = "2";
                    break;
            }
            this.PaymentType = paymentType.ValidateLength(1);

            this.PrescriptionNumber = recordIn.RxNumber.TruncateValue(0, 8).PadRightAndValidate(8, SPACE);
            this.PatientZipCode = recordIn.PatientZip.TruncateValue(0, 9).PadRightAndValidate(9, SPACE);

            // The Dr. first name field contains everything that is not the last name. If possible, the most accurate way to build it 
            // (i.e. the one that matches the original version of this report) is from the full name field.
            var doctorFirstName = string.Empty;
            var doctorLastName = recordIn.PrescriberLastName.TruncateValue(0, 28);
            var doctorFullName = $"{recordIn.PrescriberFirstName} {recordIn.PrescriberMiddleName} {doctorLastName}";
            if (!string.IsNullOrEmpty(doctorFullName) && doctorFullName.IndexOf(",") >= 0)
            {
                doctorFirstName = doctorFullName.Substring(doctorFullName.IndexOf(",") + 1).TrimStart(SPACE);
            }
            else
            {
                doctorFirstName = recordIn.PrescriberFirstName;
            }
            this.DoctorFirstName = doctorFirstName.TruncateValue(0, 10).PadRightAndValidate(10, SPACE);

            this.Bin = recordIn.BinNumber.TruncateValue(0, 6).PadLeftAndValidate(6, SPACE);
            this.ChainIndependent = "C"; // Always "C"
            this.DosageInfo = new String(recordIn.UnexpandedDirections.Where(x => (int)x >= 32 && (int)x <= 126).ToArray()).TruncateValue(0, 5).PadRightAndValidate(5, SPACE);
            this.DivNumber = (string.Empty).PadRight(3);
            this.PrescribedNdc = recordIn.WrittenNdc.Replace(DASH, string.Empty).ValidateLength(11);

            // The patient key is built out of the first 4 characters of the patients last name, the first 2 of his or her first name and his
            // or her date of birth formatted as "MM/DD/CC" where "CC" is the century ("19" for "1900-1999", "20" for "2000+")
            var patientKey = string.Empty;
            var patientLastName = string.Empty;
            var patientFirstName = string.Empty;
            var patientKeyLastName = string.Empty;
            var patientKeyFirstName = string.Empty;
            var patientKeyDob = string.Empty;
            var patientName = $"{recordIn.PatientFirstName} {recordIn.PatientMiddleName} {recordIn.PatientLastName.TruncateValue(0, 28)}";
            if (!string.IsNullOrEmpty(patientName))
            {
                var commaPosition = patientName.IndexOf(",");
                if (commaPosition >= 0)
                {
                    patientLastName = patientName.Substring(0, commaPosition);
                    patientFirstName = patientName.Substring(commaPosition + 1).TrimStart(SPACE);
                }
            }
            if (patientFirstName.Length == 0 || patientLastName.Length == 0)
            {
                patientFirstName = recordIn.PatientFirstName;
                patientLastName = recordIn.PatientLastName;
            }
            if (!string.IsNullOrEmpty(patientLastName))
            {
                patientKeyLastName = patientLastName.TruncateValue(0, 4);
                patientKey += patientKeyLastName;
            }
            if (!string.IsNullOrEmpty(patientFirstName))
            {
                patientKeyFirstName = patientFirstName.TruncateValue(0, 2);
                patientKey += patientKeyFirstName;
            }
            patientKeyDob = recordIn.BirthDate.ToString("MM/dd/yyyy").Substring(0, 8);
            patientKey += patientKeyDob;
            this.PatientKey = this.Mask(patientKey).PadRightAndValidate(14, SPACE);

            this.DoctorZipCode = recordIn.PrescriberZip.TruncateValue(0, 5).PadLeftAndValidate(5, SPACE);
            this.Filler2 = (string.Empty).PadRight(4);
            this.SupplierSpecPlan = recordIn.PlanCode.TruncateValue(3, 10).PadRightAndValidate(15, SPACE);
            this.PlanNameDescription = recordIn.PlanName.TruncateValue(0, 25).PadRightAndValidate(25, SPACE);
            this.StoreNpi = recordIn.PharmacyNpi.TruncateValue(0, 10).PadRightAndValidate(10, SPACE);
            this.PrescriberNpi = recordIn.PrescriberNpi.TruncateValue(0, 10).PadRightAndValidate(10, SPACE);
            this.Filler3 = (string.Empty).PadRight(22);
            this.Trailer = "X";
        }

        public string WriteFixedWidth()
        {                                       // FIELD NAME                   COLUMNS
            return $"{this.RecordType}" +       // Record Type Identifier       1
                $"{this.PharmacyNcpdp}" +       // Store Number (NABP)          2 - 13
                $"{this.Filler4}" +             // Filler                       14 - 20
                $"{this.DispensedDate}" +       // Dispensed Date               21 - 28
                $"{this.DispensedNdc}" +        // Dispensed NDC Code           29 - 39
                $"{this.DispensedDrug}" +       // Dispensed NDC Description    40 - 69
                $"{this.RefillNumber}" +        // Refill Number                70 - 71
                $"{this.QuantityFilled}" +      // Quantity Filled              72 - 79
                $"{this.DaysSupply}" +          // Days Supply                  80 - 82
                $"{this.AcquisitionCost}" +     // Acquisition Cost             83 - 90
                $"{this.DispensingFee}" +       // Dispensing Fee               91 - 96
                $"{this.CopayAmount}" +         // Copay Amount                 97 - 102
                $"{this.PriceOfTransaction}" +  // Price of Tx                  103 - 110
                $"{this.PatientBirthDate}" +    // Patient Birth Date           111 - 118
                $"{this.Gender}" +              // Gender                       119
                $"{this.Filler1}" +             // Filler                       120 - 137
                $"{this.PlanGroup}" +           // Plan Group                   138 - 152
                $"{this.PlanCarrierId}" +       // Plan Carrier ID              153 - 155
                $"{this.DoctorDeaNumber}" +     // Doctor DEA Number            156 - 165
                $"{this.DiagnosisCode}" +       // Diagnosis Code               166 - 171
                $"{this.CustomerLocation}" +    // Customer Location            172 - 173
                $"{this.DateWritten}" +         // Date Written                 174 - 181
                $"{this.DawCode}" +             // TP DAW Code                  182
                $"{this.DependentCode}" +       // LPT                          183 - 185
                $"{this.CompoundCode}" +        // Compound Code                186
                $"{this.AuthorizedRefills}" +   // Authorized Refills           187 - 188
                $"{this.LevelOfService}" +      // Level of Service             189 - 190
                $"{this.OriginOfRx}" +          // Origin of Rx                 191
                $"{this.DrugType}" +            // Drug Type                    192
                $"{this.DoctorLastName}" +      // Doctor Last Name             193 - 207
                $"{this.DaysSupplyBasis}" +     // Days Supply Basis            208
                $"{this.PaymentType}" +         // Payment Type                 209
                $"{this.PrescriptionNumber}" +  // Rx Number                    210 - 217
                $"{this.PatientZipCode}" +      // Patient's Zip Code           218 - 226
                $"{this.DoctorFirstName}" +     // Doctor First Name            227 - 236
                $"{this.Bin}" +                 // Bin                          237 - 242
                $"{this.ChainIndependent}" +    // Chain/Independent            243
                $"{this.DosageInfo}" +          // Dosage Info                  244 - 248
                $"{this.DivNumber}" +           // DIV Number                   249 - 251
                $"{this.PrescribedNdc}" +       // Prescribed NDC Code          252 - 262
                $"{this.PatientKey}" +          // Patient Key                  263 - 276
                $"{this.DoctorZipCode}" +       // Doctors Zip Code             277 - 281
                $"{this.Filler2}" +             // Filler                       282 - 285
                $"{this.SupplierSpecPlan}" +    // Supplier Spec Plan           286 - 300
                $"{this.PlanNameDescription}" + // Plan Name/Description        301 - 325
                $"{this.StoreNpi}" +            // NPI Store                    326 - 335
                $"{this.PrescriberNpi}" +       // NPI Doctor                   336 - 345
                $"{this.Filler3}" +             // Filler                       346 - 367
                $"{this.Trailer}";              // X                            368
        }

        private decimal ApplyMaxValue(decimal? value, decimal maxValue)
        {
            if (value is null)
            {
                return 0;
            }
            else if (value > maxValue)
            {
                return maxValue;
            }
            else
            {
                return value.Value;
            }
        }

        private string Mask(string value)
        {
            var result = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                var chars = value.ToUpper().ToArray();
                foreach (var c in chars)
                {
                    if (MaskDictionary.ContainsKey(c))
                    {
                        result += MaskDictionary[c];
                    }
                    else if (c != SPACE)
                    {
                        result += 'X';
                    }
                }
            }
            return result;
        }

        private static readonly Dictionary<char, char> MaskDictionary = new Dictionary<char, char>()
        {
            { '0', 'H' },
            { '1', 'B' },
            { '2', 'U' },
            { '3', '7' },
            { '4', 'Y' },
            { '5', 'M' },
            { '6', '2' },
            { '7', 'K' },
            { '8', 'R' },
            { '9', 'E' },
            { 'A', 'J' },
            { 'B', '3' },
            { 'C', 'V' },
            { 'D', 'G' },
            { 'E', 'S' },
            { 'F', 'X' },
            { 'G', 'N' },
            { 'H', 'L' },
            { 'I', '0' },
            { 'J', '8' },
            { 'K', 'T' },
            { 'L', '6' },
            { 'M', '1' },
            { 'N', 'D' },
            { 'O', 'Z' },
            { 'P', 'Q' },
            { 'Q', 'O' },
            { 'R', '5' },
            { 'S', 'P' },
            { 'T', 'C' },
            { 'U', 'A' },
            { 'V', '4' },
            { 'W', '9' },
            { 'X', 'W' },
            { 'Y', 'I' },
            { 'Z', 'F' }
        };
    }
}