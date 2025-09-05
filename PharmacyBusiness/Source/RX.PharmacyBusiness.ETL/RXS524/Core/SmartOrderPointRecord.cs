namespace RX.PharmacyBusiness.ETL.RXS524.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Wegmans.PharmacyLibrary.IO;

    public class SmartOrderPointRecord : IRecord
    {
        public string hardcoded_i { get; set; }
        public string order_ndc_wo { get; set; }
        public string left_zero_padded_store { get; set; }
        public string always_blank_1 { get; set; }
        public string order_point1 { get; set; }
        public string order_point2 { get; set; }
        public string always_blank_2 { get; set; }
        public string always_blank_3 { get; set; }
        public string always_blank_4 { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            this.hardcoded_i = fields.ElementAt(0);
            this.order_ndc_wo = fields.ElementAt(1);
            this.left_zero_padded_store = fields.ElementAt(2);
            this.always_blank_1 = fields.ElementAt(3);
            this.order_point1 = fields.ElementAt(4);
            this.order_point2 = fields.ElementAt(5);
            this.always_blank_2 = fields.ElementAt(6);
            this.always_blank_3 = fields.ElementAt(7);
            this.always_blank_4 = fields.ElementAt(8);
        }
    }
}
