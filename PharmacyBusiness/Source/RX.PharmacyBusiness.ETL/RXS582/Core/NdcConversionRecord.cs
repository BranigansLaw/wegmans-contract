namespace RX.PharmacyBusiness.ETL.RXS582.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Wegmans.PharmacyLibrary.IO;

    public class NdcConversionRecord : IRecord
    {
        public string DrugOriginalNdc { get; set; }
        public string DrugNdcConversion { get; set; }
        public string DrugNameConversion { get; set; }

        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            this.DrugOriginalNdc = fields.ElementAt(0).Trim();
            this.DrugNdcConversion = fields.ElementAt(1).Trim();
            this.DrugNameConversion = fields.ElementAt(2).Trim();
        }
    }
}
