namespace Library.TenTenInterface.DataModel
{
    public class CompleteSpecialtyItemRow
    {
        public string? NdcWo { get; set; }
        public string? ProgramHeader { get; set; }
        public string? ActualDrugName { get; set; }

        /// <summary>
        /// The order of parameters in this constructor is the same order as in the 1010data query results.
        /// Also, the parameter names are spelled exactly as the column headers in the 1010data query results to make mapping easier.
        /// WARNING: Do NOT change the order of these parameters, it is used in the TenTen CSV column export
        /// </summary>
        /// <param name="ndc_wo"></param>
        /// <param name="program_header"></param>
        /// <param name="actual_drug_name"></param>
        public CompleteSpecialtyItemRow(string? ndc_wo,
                                        string? program_header, 
                                        string? actual_drug_name)
        {
            NdcWo = ndc_wo;
            ProgramHeader = program_header;
            ActualDrugName = actual_drug_name;
        }
    }
}
