namespace RX.PharmacyBusiness.ETL.RXS506.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class RowCountRecord : IRecord
    {
        public string TableNane { get; set; }
        public long RowCount { get; set; }
        public void SetValuesFromInputFields(IEnumerable<string> fields)
        {
            this.TableNane = fields.ElementAt(0);
            this.RowCount = long.Parse(fields.ElementAt(1));
        }
    }
}
