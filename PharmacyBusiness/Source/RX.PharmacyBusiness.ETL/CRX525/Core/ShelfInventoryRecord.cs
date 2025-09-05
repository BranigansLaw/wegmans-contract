namespace RX.PharmacyBusiness.ETL.CRX525.Core
{
    using System;

    public class ShelfInventoryRecord
    {
        public const int FixedWidthInputLineLength = 92;

        public ShelfInventoryRecord()
        {
        }

        public ShelfInventoryRecord(string input, string fileName)
        {
            if (input.Length == FixedWidthInputLineLength)
            { 
                this.Store = input.Substring(0, 6);
                this.Date = input.Substring(6, 6);
                this.AreaBay = input.Substring(12, 2);
                this.Shelf = input.Substring(16, 1);
                this.Code = input.Substring(20, 3);
                this.NDC = input.Substring(23, 11);
                this.Description = input.Substring(34, 30);
                this.Cost = input.Substring(64, 8);
                this.Quantity = input.Substring(72, 6);
                this.ExtendedCost = input.Substring(78, 14);
                this.FileName = fileName;
            }
            else
            {
                throw new ArgumentNullException("input");
            }
        }

        public string Store { get; set; }
        public string Date { get; set; }
        public string AreaBay { get; set; }
        public string Shelf { get; set; }
        public string Code { get; set; }
        public string NDC { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public string Quantity { get; set; }
        public string ExtendedCost { get; set; }
        public string FileName { get; set; }
    }
}