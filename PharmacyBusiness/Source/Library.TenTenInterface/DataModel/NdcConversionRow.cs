namespace Library.TenTenInterface.DataModel
{
    public class NdcConversionRow
    {
        public string? NdcWo { get; private set; }
        public string? NdcConversionWo { get; private set; }
        public string? DrugNameConversion { get; private set; }

        /// <summary>
        /// The order of parameters in this constructor is the same order as in the 1010data query results.
        /// Also, the parameter names are spelled exactly as the column headers in the 1010data query results to make mapping easier.
        /// </summary>
        /// <param name="ndc_wo"></param>
        /// <param name="ndc_conversion_wo"></param>
        /// <param name="drug_name_conversion"></param>
        public NdcConversionRow(
            string? ndc_wo,
            string? ndc_conversion_wo,
            string? drug_name_conversion)
        {
            NdcWo = ndc_wo;
            NdcConversionWo = ndc_conversion_wo;
            DrugNameConversion = drug_name_conversion;
        }
    }
}
