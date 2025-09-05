namespace RX.PharmacyBusiness.ETL.RXS710
{
    public class NDCP3HeaderOut
    {
        private const char SPACE = ' ';

        private string RecordType { get; set; }
        public string NcpdpNumber { get; set; }
        private string PharmacyNickname { get; set; }
        private string PharmacyAddress1 { get; set; }
        private string PharmacyCity { get; set; }
        private string PharmacyState { get; set; }
        private string PharmacyZip { get; set; }
        private string PharmacyPhoneNumber { get; set; }
        private string PharmacyNpi { get; set; }
        private string Filler { get; set; }
        private string Trailer { get; set; }

        public NDCP3HeaderOut() { }

        public NDCP3HeaderOut(NDCP3HeaderIn recordIn)
        {
            this.RecordType = "2";
            this.NcpdpNumber = recordIn.NcpdpNumber.TruncateValue(0, 12).PadRightAndValidate(12, SPACE);
            this.PharmacyNickname = (string.Empty).PadRight(20);
            this.PharmacyAddress1 = recordIn.PharmacyAddress1.TruncateValue(0, 20).PadRightAndValidate(20, SPACE);
            this.PharmacyCity = recordIn.PharmacyCity.TruncateValue(0, 18).PadRightAndValidate(18, SPACE);
            this.PharmacyState = recordIn.PharmacyState.ValidateLength(2);
            this.PharmacyZip = recordIn.PharmacyZip.TruncateValue(0, 9).PadRightAndValidate(9, SPACE);
            this.PharmacyPhoneNumber = recordIn.PharmacyPhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").TruncateValue(0, 10).PadRightAndValidate(10, SPACE);
            this.PharmacyNpi = (string.Empty).PadRight(10);
            this.Filler = (string.Empty).PadRight(265);
            this.Trailer = "X";
        }

        public string WriteFixedWidth()
        {                                       // FIELD NAME                   COLUMNS
            return $"{this.RecordType}" +       // Record Type Identifier       1
                $"{this.NcpdpNumber}" +         // Store Number (NABP)          2 - 13
                $"{this.PharmacyNickname}" +    // Pharmacy Name                14 - 33
                $"{this.PharmacyAddress1}" +    // Pharmacy Street Address      34 - 53
                $"{this.PharmacyCity}" +        // Pharmacy City                54 - 71
                $"{this.PharmacyState}" +       // Pharmacy State               72 - 73
                $"{this.PharmacyZip}" +         // Pharmacy Phone Number        74 - 82
                $"{this.PharmacyPhoneNumber}" + // Pharmacy Telephone Number    83 - 92
                $"{this.PharmacyNpi}" +         // Store NPI                    93 - 102
                $"{this.Filler}" +              // Filler                       103 - 367
                $"{this.Trailer}";              // X                            368
        }
    }
}
