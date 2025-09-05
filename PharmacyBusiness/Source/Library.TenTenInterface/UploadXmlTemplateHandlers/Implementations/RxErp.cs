using Library.TenTenInterface.Extensions;

namespace Library.TenTenInterface.XmlTemplateHandlers.Implementations
{
    /// <summary>
    /// Thr properties listed here related to data types and data requirements within 1010data data destination rather than from data source.
    /// </summary>
    public class RxErp : ITenTenUploadConvertible
    {
        public required DateTime? RxErpChangeDate { get; set; }
        public required decimal? PatientNbr { get; set; }
        public required int? StoreNbr { get; set; }
        public required int? RxNbr { get; set; }
        public required string? RxNbrTxt { get; set; }
        public required string? NdcWithoutDashes { get; set; }
        public required string? DrugName { get; set; }
        public required decimal? DaysSupply { get; set; }
        public required int? RefillsAuthorized { get; set; }
        public required DateTime? LastFillDate { get; set; }
        public required DateTime? ErpOrigTargetDate { get; set; }
        public required DateTime? ErpTargetDate { get; set; }
        public required string? ErpEnrollStatus { get; set; }
        public required DateTime? ErpEnrollStatusDateTime { get; set; }
        public required string? ErpEnrollReason { get; set; }
        public required string? ErpExcludeReason { get; set; }
        public required string? ErpDeliveryMethod { get; set; }
        public required string? ErpDeliveryMethodPersist { get; set; }
        public required string? IsConsentToFill { get; set; }
        public required string? UserConsentToFill { get; set; }
        public required DateTime? DateConsentToFill { get; set; }
        public required DateTime? RxRecordEffectiveStartDateTime { get; set; }
        public required string? RecordType { get; set; }
        public required DateTime? ErpUsermodifiedTargetDate { get; set; }
        public required string? ErpModifiedUserType { get; set; }
        public required string? ErpModifiedUser { get; set; }
        public required string? ErpUserModifiedRecordType { get; set; }
        public required DateTime? ErpUserModifiedOnDate { get; set; }
        public required int? RecordUpdates { get; set; }
        public required string? PatientAutofillEnrollStatus { get; set; }
        public required string? PatientErpEnrollStatus { get; set; }
        public required DateTime? PatientErpEnrollDate { get; set; }
        public required string? PatientAutoEnrollFutureRxs { get; set; }
        public required string? PatientPreferredDeliveryMethod { get; set; }
        public required string? PatientPrefersMailDelivery { get; set; }
        public required string? PatientErpPaymentMethod { get; set; }
        public required string? PatientRequestConsentToFill { get; set; }
        public required string? PatientRequestConsentToErp { get; set; }
        public required string? PatientExternalId { get; set; }
        public required string? ProhibitRenewalRequest { get; set; }
        public required string? CsrDiagnosisCode { get; set; }
        public required string? DrugForm { get; set; }
        public required decimal? QtyAuthorized { get; set; }
        public required decimal? RefillQty { get; set; }
        public required DateTime? ExpireDate { get; set; }
        public required string? ReassignedRxNum { get; set; }
        public required DateTime? RxCreatedDate { get; set; }
        public required string? RxCreatedUser { get; set; }
        public required string? RxOriginCode { get; set; }
        public required string? RxStatus { get; set; }
        public required decimal? RxRecordNum { get; set; }
        public required decimal? FirstFillQty { get; set; }
        public required DateTime? DatePrescribed { get; set; }
        public required string? OriginalNdcWithoutDashes { get; set; }
        public required string? OriginalDrugName { get; set; }
        public required decimal? OriginalQty { get; set; }
        public required string? Serial { get; set; }
        public required string? DawCode { get; set; }
        public required string? SelectedUpc { get; set; }
        public required string? AckCode { get; set; }
        public required string? LetterOnFile { get; set; }
        public required string? EmployeeLoginName { get; set; }
        public required DateTime? LastAckChangeDate { get; set; }
        public required string? LastAckChangeUser { get; set; }
        public required string? LastAckChangeUserName { get; set; }
        public required DateTime? OriginalFillDate { get; set; }
        public required decimal? TotalDailyDose { get; set; }
        public required decimal? TotalQtyDispensed { get; set; }
        public required decimal? TotalQtyTransferred { get; set; }
        public required decimal? LegalFills { get; set; }
        public required decimal? LatestFill { get; set; }
        public required decimal? LatestFillDuration { get; set; }
        public required decimal? LatestFillNonInprocess { get; set; }
        public required decimal? LatestFillCanceled { get; set; }
        public required decimal? LastDispFill { get; set; }
        public required decimal? LastFillRefill { get; set; }
        public required decimal? LastFillReleased { get; set; }
        public required decimal? LastSoldFill { get; set; }
        public required DateTime? ProfileIncludeDate { get; set; }
        public required decimal? FirstFill { get; set; }
        public required decimal? FirstRefill { get; set; }
        public required DateTime? EarliestFillDate { get; set; }
        public required decimal? PrevFillReleased { get; set; }
        public required DateTime? SyncDate { get; set; }
        public required string? IsLinked { get; set; }
        public required string? CancelReason { get; set; }
        public required decimal? PrescriberNum { get; set; }
        public required decimal? PrescriberAddressNum { get; set; }
        public required string? PrescriberOrderNbr { get; set; }
        public required string? IsEPCS { get; set; }
        public required string? RrrDenied { get; set; }

        /// <inheritdoc />
        public string BaseTemplatePath => "RxErp.xml";

        /// <inheritdoc />
        public string FolderNameOfDatedTables => "rx_erp";

        /// <inheritdoc />
        public string FolderTitleOfDatedTables => "Rx Erp";

        /// <inheritdoc />
        public string TenTenRollupTableTitle => "Rx Erp Rollup";

        /// <inheritdoc />
        public string[] TenTenRollupTableSegmentColumns => ["patient_num"];

        /// <inheritdoc />
        public string[] TenTenRollupTableSortColumns => [];

        /// <inheritdoc />
        public string TenTenGoLiveRollupTableFullPathOverride => string.Empty; //On Go Live Date (but NOT before then) change this value to "wegmans.wegmansdata.dwfeeds.rxerp", but DO NOT set this value prior to Go Live or you will effectively delete current Production data.

        /// <inheritdoc />
        public bool ReplaceExistingDataOnUpload => true;

        /// <inheritdoc />
        public IEnumerable<string> AdditionalPostprocessingSteps => [];

        /// <inheritdoc />
        public string ToTenTenUploadXml()
        {
            return $"<tr>" +
                $"<td>{RxErpChangeDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{PatientNbr}</td>" +
                $"<td>{StoreNbr}</td>" +
                $"<td>{RxNbr}</td>" +
                $"<td>{RxNbrTxt.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{NdcWithoutDashes.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{DrugName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{DaysSupply}</td>" +
                $"<td>{RefillsAuthorized}</td>" +
                $"<td>{LastFillDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{ErpOrigTargetDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{ErpTargetDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{ErpEnrollStatus.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpEnrollStatusDateTime.ConvertToTenTenDataType<decimal>()}</td>" +
                $"<td>{ErpEnrollReason.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpExcludeReason.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpDeliveryMethod.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpDeliveryMethodPersist.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{IsConsentToFill.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{UserConsentToFill.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{DateConsentToFill.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{RxRecordEffectiveStartDateTime.ConvertToTenTenDataType<decimal>()}</td>" +
                $"<td>{RecordType.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpUsermodifiedTargetDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{ErpModifiedUserType.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpModifiedUser.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpUserModifiedRecordType.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ErpUserModifiedOnDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{RecordUpdates}</td>" +
                $"<td>{PatientAutofillEnrollStatus.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientErpEnrollStatus.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientErpEnrollDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{PatientAutoEnrollFutureRxs.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientPreferredDeliveryMethod.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientPrefersMailDelivery.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientErpPaymentMethod.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientRequestConsentToFill.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientRequestConsentToErp.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PatientExternalId.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{ProhibitRenewalRequest.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{CsrDiagnosisCode.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{DrugForm.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{QtyAuthorized}</td>" +
                $"<td>{RefillQty}</td>" +
                $"<td>{ExpireDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{ReassignedRxNum.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{RxCreatedDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{RxCreatedUser.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{RxOriginCode.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{RxStatus.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{RxRecordNum}</td>" +
                $"<td>{FirstFillQty}</td>" +
                $"<td>{DatePrescribed.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{OriginalNdcWithoutDashes.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{OriginalDrugName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{OriginalQty}</td>" +
                $"<td>{Serial.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{DawCode.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{SelectedUpc.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{AckCode.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{LetterOnFile.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{EmployeeLoginName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{LastAckChangeDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{LastAckChangeUser.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{LastAckChangeUserName.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{OriginalFillDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{TotalDailyDose}</td>" +
                $"<td>{TotalQtyDispensed}</td>" +
                $"<td>{TotalQtyTransferred}</td>" +
                $"<td>{LegalFills}</td>" +
                $"<td>{LatestFill}</td>" +
                $"<td>{LatestFillDuration}</td>" +
                $"<td>{LatestFillNonInprocess}</td>" +
                $"<td>{LatestFillCanceled}</td>" +
                $"<td>{LastDispFill}</td>" +
                $"<td>{LastFillRefill}</td>" +
                $"<td>{LastFillReleased}</td>" +
                $"<td>{LastSoldFill}</td>" +
                $"<td>{ProfileIncludeDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{FirstFill}</td>" +
                $"<td>{FirstRefill}</td>" +
                $"<td>{EarliestFillDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{PrevFillReleased}</td>" +
                $"<td>{SyncDate.ConvertToTenTenDataType<int>()}</td>" +
                $"<td>{IsLinked.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{CancelReason.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{PrescriberNum}</td>" +
                $"<td>{PrescriberAddressNum}</td>" +
                $"<td>{PrescriberOrderNbr.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{IsEPCS.CleanStringForTenTenDataUpload()}</td>" +
                $"<td>{RrrDenied.CleanStringForTenTenDataUpload()}</td>" +
                $"</tr>";
        }
    }
}
