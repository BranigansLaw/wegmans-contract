namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class IvrOutboundCalls : ITenTenUploadConvertible
    {        
		public string? PhoneNbr { get; set; }

        public string? CallMadeDate { get; set; }

        public string? CallMadeTime { get; set; }

        public string? CallStatus { get; set; }

        public string? AnsweredBy { get; set; }

        public int? RxNum { get; set; }

        public int? NbrAttempts { get; set; }

        public int? StoreNum { get; set; }

        public string? CallType { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "IvrOutboundCalls.xml";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "IVR Outbound Calls Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => [];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.ivr_outbound_calls", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "ivr_outbound_calls";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "IVR Outbound Calls";

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
               $"<td>{PhoneNbr}</td>" +
               $"<td>{CallMadeDate}</td>" +
               $"<td>{CallMadeTime}</td>" +
               $"<td>{CallStatus}</td>" +
               $"<td>{AnsweredBy}</td>" +
               $"<td>{RxNum}</td>" +
               $"<td>{NbrAttempts}</td>" +
               $"<td>{StoreNum}</td>" +
               $"<td>{CallType}</td>" +
               $"</tr>";
        }
    }
}
