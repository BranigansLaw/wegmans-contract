namespace Library.TenTenInterface.DataModel
{
    public class SpecialtyDispenseExclusionRow
    {
        public string? NdcWo { get; set; }
        public string? Medication { get; set; }

        /// <summary>
        /// The order of parameters in this constructor is the same order as in the 1010data query results.
        /// Also, the parameter names are spelled exactly as the column headers in the 1010data query results to make mapping easier.
        /// </summary>
        /// <param name="ndc_wo"></param>
        /// <param name="medication"></param>
        public SpecialtyDispenseExclusionRow(string? ndc_wo, string? medication)
        {
            NdcWo = ndc_wo;
            Medication = medication;
        }
    }
}
