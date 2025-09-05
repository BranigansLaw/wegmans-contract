namespace RX.PharmacyBusiness.ETL.CRX582.Business
{
    public static class BooleanExtensions
    {
        public static string ToShortYesNo(this bool source)
        {
            return source ? "Y" : "N";
        }
    }
}
