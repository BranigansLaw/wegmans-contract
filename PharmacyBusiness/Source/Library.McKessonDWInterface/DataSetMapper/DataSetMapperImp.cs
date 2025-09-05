using Library.LibraryUtilities.Extensions;
using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.Helper;
using Library.TenTenInterface.DataModel;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Library.McKessonDWInterface.DataSetMapper
{
    public class DataSetMapperImp : IDataSetMapper
    {
        private readonly ILogger<DataSetMapperImp> _logger;
        private readonly ITurnaroundTimeHelper _turnaroundTimeHelper;
        private readonly ISureScriptsHelper _sureScriptsHelper;
        private readonly IAstuteAdherenceDispenseHelper _astuteAdherenceDispenseHelper;

        public DataSetMapperImp(
            ILogger<DataSetMapperImp> logger,
            ITurnaroundTimeHelper turnaroundTimeHelper,
            ISureScriptsHelper sureScriptsHelper,
            IAstuteAdherenceDispenseHelper astuteAdherenceDispenseHelper
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _turnaroundTimeHelper = turnaroundTimeHelper ?? throw new ArgumentNullException(nameof(turnaroundTimeHelper));
            _sureScriptsHelper = sureScriptsHelper ?? throw new ArgumentNullException(nameof(sureScriptsHelper));
            _astuteAdherenceDispenseHelper = astuteAdherenceDispenseHelper ?? throw new ArgumentNullException(nameof(astuteAdherenceDispenseHelper));
        }

        /// <inheritdoc />
        public IEnumerable<OmnisysClaimRow> MapOmnisysClaim(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<OmnisysClaimRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new OmnisysClaimRow
                {
                    PharmacyNpi = row.NonNullField<string>(0, rowNumber),
                    PrescriptionNbr = row.NonNullField<string>(1, rowNumber),
                    RefillNumber = row.NonNullField<string>(2, rowNumber),
                    DateOfService = DateOnly.ParseExact(row.NonNullField<string>(3, rowNumber), "yyyyMMdd"),
                    SoldDate = DateOnly.ParseExact(row.NonNullField<string>(4, rowNumber), "yyyyMMdd"),
                    NdcNumber = row.NonNullField<string>(5, rowNumber),
                    CardholderId = row.NonNullField<string>(6, rowNumber),
                    AuthorizationNumber = row.Field<string?>(7) ?? string.Empty,
                    ReservedForFutureUse = row.Field<string?>(8) ?? string.Empty,
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<NewTagPatientGroupsRow> MapNewTagPatientGroups(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<NewTagPatientGroupsRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new NewTagPatientGroupsRow
                {
                    PatientAddDate = row.NullableField<DateTime>(0, rowNumber),
                    StoreNum = row.NullableField<int>(1, rowNumber),
                    PatientNum = row.NullableField<decimal>(2, rowNumber),
                    GroupNum = row.NullableField<decimal>(3, rowNumber),
                    GroupName = row.NullableString(4, rowNumber),
                    GroupDescription = row.NullableString(5, rowNumber),
                    EmployeeUserName = row.NullableString(6, rowNumber),
                    EmployeeFirstName = row.NullableString(7, rowNumber),
                    EmployeeLastName = row.NullableString(8, rowNumber),
                    EventDescription = row.NullableString(9, rowNumber),
                    GroupStartDate = row.NullableField<DateTime>(10, rowNumber)
                });


                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<RxErpRow> MapRxErp(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<RxErpRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new RxErpRow
                {
                    RxErpChangeDate = row.NullableField<DateTime>(0, rowNumber),
                    PatientNbr = row.NullableField<decimal>(1, rowNumber),
                    StoreNbr = row.NullableField<int>(2, rowNumber),
                    RxNbr = row.NullableField<int>(3, rowNumber),
                    RxNbrTxt = row.NullableString(4, rowNumber),
                    NdcWithoutDashes = row.NullableString(5, rowNumber),
                    DrugName = row.NullableString(6, rowNumber),
                    DaysSupply = row.NullableField<decimal>(7, rowNumber),
                    RefillsAuthorized = row.NullableField<int>(8, rowNumber),
                    LastFillDate = row.NullableField<DateTime>(9, rowNumber),
                    ErpOrigTargetDate = row.NullableField<DateTime>(10, rowNumber),
                    ErpTargetDate = row.NullableField<DateTime>(11, rowNumber),
                    ErpEnrollStatus = row.NullableString(12, rowNumber),
                    ErpEnrollStatusDateTime = row.NullableField<DateTime>(13, rowNumber),
                    ErpEnrollReason = row.NullableString(14, rowNumber),
                    ErpExcludeReason = row.NullableString(15, rowNumber),
                    ErpDeliveryMethod = row.NullableString(16, rowNumber),
                    ErpDeliveryMethodPersist = row.NullableString(17, rowNumber),
                    IsConsentToFill = row.NullableString(18, rowNumber),
                    UserConsentToFill = row.NullableString(19, rowNumber),
                    DateConsentToFill = row.NullableField<DateTime>(20, rowNumber),
                    RxRecordEffectiveStartDateTime = row.NullableField<DateTime>(21, rowNumber),
                    RecordType = row.NullableString(22, rowNumber),
                    ErpUsermodifiedTargetDate = row.NullableField<DateTime>(23, rowNumber),
                    ErpModifiedUserType = row.NullableString(24, rowNumber),
                    ErpModifiedUser = row.NullableString(25, rowNumber),
                    ErpUserModifiedRecordType = row.NullableString(26, rowNumber),
                    ErpUserModifiedOnDate = row.NullableField<DateTime>(27, rowNumber),
                    RecordUpdates = row.NullableField<int>(28, rowNumber),
                    PatientAutofillEnrollStatus = row.NullableString(29, rowNumber),
                    PatientErpEnrollStatus = row.NullableString(30, rowNumber),
                    PatientErpEnrollDate = row.NullableField<DateTime>(31, rowNumber),
                    PatientAutoEnrollFutureRxs = row.NullableString(32, rowNumber),
                    PatientPreferredDeliveryMethod = row.NullableString(33, rowNumber),
                    PatientPrefersMailDelivery = row.NullableString(34, rowNumber),
                    PatientErpPaymentMethod = row.NullableString(35, rowNumber),
                    PatientRequestConsentToFill = row.NullableString(36, rowNumber),
                    PatientRequestConsentToErp = row.NullableString(37, rowNumber),
                    PatientExternalId = row.NullableString(38, rowNumber),
                    ProhibitRenewalRequest = row.NullableString(39, rowNumber),
                    CsrDiagnosisCode = row.NullableString(40, rowNumber),
                    DrugForm = row.NullableString(41, rowNumber),
                    QtyAuthorized = row.NullableField<decimal>(42, rowNumber),
                    RefillQty = row.NullableField<decimal>(43, rowNumber),
                    ExpireDate = row.NullableField<DateTime>(44, rowNumber),
                    ReassignedRxNum = row.NullableString(45, rowNumber),
                    RxCreatedDate = row.NullableField<DateTime>(46, rowNumber),
                    RxCreatedUser = row.NullableString(47, rowNumber),
                    RxOriginCode = row.NullableString(48, rowNumber),
                    RxStatus = row.NullableString(49, rowNumber),
                    RxRecordNum = row.NullableField<decimal>(50, rowNumber),
                    FirstFillQty = row.NullableField<decimal>(51, rowNumber),
                    DatePrescribed = row.NullableField<DateTime>(52, rowNumber),
                    OriginalNdcWithoutDashes = row.NullableString(53, rowNumber),
                    OriginalDrugName = row.NullableString(54, rowNumber),
                    OriginalQty = row.NullableField<decimal>(55, rowNumber),
                    Serial = row.NullableString(56, rowNumber),
                    DawCode = row.NullableString(57, rowNumber),
                    SelectedUpc = row.NullableString(58, rowNumber),
                    AckCode = row.NullableString(59, rowNumber),
                    LetterOnFile = row.NullableString(60, rowNumber),
                    EmployeeLoginName = row.NullableString(61, rowNumber),
                    LastAckChangeDate = row.NullableField<DateTime>(62, rowNumber),
                    LastAckChangeUser = row.NullableString(63, rowNumber),
                    LastAckChangeUserName = row.NullableString(64, rowNumber),
                    OriginalFillDate = row.NullableField<DateTime>(65, rowNumber),
                    TotalDailyDose = row.NullableField<decimal>(66, rowNumber),
                    TotalQtyDispensed = row.NullableField<decimal>(67, rowNumber),
                    TotalQtyTransferred = row.NullableField<decimal>(68, rowNumber),
                    LegalFills = row.NullableField<decimal>(69, rowNumber),
                    LatestFill = row.NullableField<decimal>(70, rowNumber),
                    LatestFillDuration = row.NullableField<decimal>(71, rowNumber),
                    LatestFillNonInprocess = row.NullableField<decimal>(72, rowNumber),
                    LatestFillCanceled = row.NullableField<decimal>(73, rowNumber),
                    LastDispFill = row.NullableField<decimal>(74, rowNumber),
                    LastFillRefill = row.NullableField<decimal>(75, rowNumber),
                    LastFillReleased = row.NullableField<decimal>(76, rowNumber),
                    LastSoldFill = row.NullableField<decimal>(77, rowNumber),
                    ProfileIncludeDate = row.NullableField<DateTime>(78, rowNumber),
                    FirstFill = row.NullableField<decimal>(79, rowNumber),
                    FirstRefill = row.NullableField<decimal>(80, rowNumber),
                    EarliestFillDate = row.NullableField<DateTime>(81, rowNumber),
                    PrevFillReleased = row.NullableField<decimal>(82, rowNumber),
                    SyncDate = row.NullableField<DateTime>(83, rowNumber),
                    IsLinked = row.NullableString(84, rowNumber),
                    CancelReason = row.NullableString(85, rowNumber),
                    PrescriberNum = row.NullableField<decimal>(86, rowNumber),
                    PrescriberAddressNum = row.NullableField<decimal>(87, rowNumber),
                    PrescriberOrderNbr = row.NullableString(88, rowNumber),
                    IsEPCS = row.NullableString(89, rowNumber),
                    RrrDenied = row.NullableString(90, rowNumber),
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<SoldDetailRow> MapSoldDetail(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<SoldDetailRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new SoldDetailRow
                {
                    StoreNbr = row.NullableField<int>(0, rowNumber),
                    RxNbr = row.NullableField<int>(1, rowNumber),
                    RefillNbr = row.NullableField<int>(2, rowNumber),
                    PartialFillSequenceNbr = row.NullableField<int>(3, rowNumber),
                    SoldDate = row.NullableField<DateTime>(4, rowNumber),
                    OrderNbr = row.NullableString(5, rowNumber),
                    QtyDispensed = row.NullableField<decimal>(6, rowNumber),
                    NdcWithoutDashes = row.NullableString(7, rowNumber),
                    AcquisitionCost = row.NullableField<decimal>(8, rowNumber),
                    ThirdPartyPay = row.NullableField<decimal>(9, rowNumber),
                    PatientPay = row.NullableField<decimal>(10, rowNumber),
                    TransactionPrice = row.NullableField<decimal>(11, rowNumber),
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<StoreInventoryHistoryRow> MapStoreInventoryHistory(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<StoreInventoryHistoryRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new StoreInventoryHistoryRow
                {
                    DateOfService = row.NullableField<DateTime>(0, rowNumber),
                    StoreNbr = row.NullableField<int>(1, rowNumber),
                    NdcWithoutDashes = row.NullableString(2, rowNumber),
                    DrugName = row.NullableString(3, rowNumber),
                    Sdgi = row.NullableString(4, rowNumber),
                    Gcn = row.NullableString(5, rowNumber),
                    GcnSequence = row.NullableField<decimal>(6, rowNumber),
                    OrangeBookCode = row.NullableString(7, rowNumber),
                    FormCode = row.NullableString(8, rowNumber),
                    PackSize = row.NullableField<decimal>(9, rowNumber),
                    TruePack = row.NullableField<decimal>(10, rowNumber),
                    PM = row.NullableString(11, rowNumber),
                    IsPreferred = row.NullableString(12, rowNumber),
                    LastAcquisitionCostPack = row.NullableField<decimal>(13, rowNumber),
                    LastAcquisitionCostUnit = row.NullableField<decimal>(14, rowNumber),
                    LastAcquisitionCostDate = row.NullableField<DateTime>(15, rowNumber),
                    OnHandQty = row.NullableField<decimal>(16, rowNumber),
                    OnHandValue = row.NullableField<decimal>(17, rowNumber),
                    ComittedQty = row.NullableField<decimal>(18, rowNumber),
                    ComittedValue = row.NullableField<decimal>(19, rowNumber),
                    TotalQTY = row.NullableField<decimal>(20, rowNumber),
                    TotalValue = row.NullableField<decimal>(21, rowNumber),
                    AcquisitionCostPack = row.NullableField<decimal>(22, rowNumber),
                    AcquisitionCostUnit = row.NullableField<decimal>(23, rowNumber),
                    PrimarySupplier = row.NullableString(24, rowNumber),
                    LastChangeDate = row.NullableField<DateTime>(25, rowNumber),
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<TatRawDataRow> MapTatRawData(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<TatRawDataRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new TatRawDataRow
                {
                    OrderNbr = row.NonNullField<string>(0, rowNumber),
                    Facility = row.NonNullField<string>(1, rowNumber),
                    McKessonPatientKey = row.NonNullField<long>(2, rowNumber),
                    RxNbr = row.NullableString(3, rowNumber) ?? string.Empty,
                    RefillNbr = row.NullableField<long>(4, rowNumber) ?? 0,
                    DateIn = row.NonNullField<DateTime>(5, rowNumber),
                    DateOut = row.NonNullField<DateTime>(6, rowNumber),
                    WfsdKey = row.NonNullField<long>(7, rowNumber),
                    WfsdDescription = row.NonNullField<string>(8, rowNumber),
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<TatDetailsRow> PopulateTatDerivedDetails(IEnumerable<TatRawDataRow> dataRows, string tatTarget)
        {
            List<TatDetailsRow> rows = new List<TatDetailsRow>();

            foreach (TatRawDataRow dataRow in dataRows)
            {
                rows.Add(new TatDetailsRow()
                {
                    OrderNbr = dataRow.OrderNbr,
                    Facility = dataRow.Facility,
                    McKessonPatientKey = dataRow.McKessonPatientKey,
                    RxNbr = dataRow.RxNbr,
                    RefillNbr = dataRow.RefillNbr,
                    DateIn = dataRow.DateIn,
                    DateOut = dataRow.DateOut,
                    WfsdKey = dataRow.WfsdKey,
                    WfsdDescription = dataRow.WfsdDescription
                });
            }

            foreach (var groupedRows in rows
                .GroupBy(row => new
                {
                    row.OrderNbr,
                    row.RxNbr,
                    row.RefillNbr
                }))
            {
                int dateInRank = 1;
                int dateOutRank = groupedRows.Count();
                DateTime previousRowDateOut = DateTime.MinValue;

                foreach (var row in groupedRows
                    .OrderBy(r => r.DateIn)
                    .ThenBy(r => r.DateOut))
                {
                    if (previousRowDateOut == DateTime.MinValue)
                        previousRowDateOut = row.DateIn;

                    string wfsdDescription = row.WfsdDescription;
                    DateTime thisRowDateOut = row.DateOut;

                    string isIntervention = _turnaroundTimeHelper.DeriveIsIntervention(wfsdDescription, tatTarget) ? "Y" : "N";
                    string isException = _turnaroundTimeHelper.DeriveIsException(wfsdDescription) ? "Y" : "N";
                    decimal elapsedDaysInStep = _turnaroundTimeHelper.DeriveDaysInStep(previousRowDateOut, thisRowDateOut);
                    decimal elapsedDaysInStepOffHrs = _turnaroundTimeHelper.DeriveDaysOffHours(previousRowDateOut, thisRowDateOut);

                    row.IsIntervention = isIntervention;
                    row.IsException = isException;
                    row.ElapsedDaysInStep = elapsedDaysInStep;
                    row.ElapsedDaysInStepOffHrs = elapsedDaysInStepOffHrs;
                    row.DateInRank = dateInRank;
                    row.DateOutRank = dateOutRank;

                    dateInRank++;
                    dateOutRank--;
                    previousRowDateOut = row.DateOut;
                }
            }

            return rows;
        }

        /// <inheritdoc />
        public IEnumerable<TatSummaryRow> PopulateTatDerivedSummary(IEnumerable<TatDetailsRow> dataRows, string tatTarget)
        {
            List<TatSummaryRow> summaryRows = new List<TatSummaryRow>();

            foreach (var groupedDataRows in dataRows
                .Where(r => r.RxNbr.IndexOf("RR") == -1) //exclude refill requests
                .GroupBy(row => new
                {
                    row.OrderNbr,
                    row.RxNbr,
                    row.RefillNbr
                }))
            {
                DateTime firstDateIn = groupedDataRows.Where(r => r.DateInRank == 1).First().DateIn;
                DateTime lastDateOut = groupedDataRows.Where(r => r.DateOutRank == 1).Last().DateOut;
                decimal daysOverall = _turnaroundTimeHelper.DeriveDaysInStep(firstDateIn, lastDateOut);
                decimal deductDaysIntervention = (decimal)groupedDataRows.Where(r => r.IsIntervention == "Y").Sum(r => r.ElapsedDaysInStep ?? 0);
                decimal deductDaysOffHours = (decimal)groupedDataRows.Where(r => r.IsIntervention == "N").Sum(r => r.ElapsedDaysInStepOffHrs ?? 0);
                decimal daysNetTat = daysOverall - deductDaysIntervention - deductDaysOffHours;
                string hasExceptions = groupedDataRows.Any(r => r.IsException == "Y") ? "Y" : "N";

                foreach (var groupedDataRow in groupedDataRows
                    .OrderBy(r => r.DateIn)
                    .ThenBy(r => r.DateOut))
                {
                    if (!string.IsNullOrEmpty(groupedDataRow.RxNbr) ||
                        tatTarget == TurnaroundTimeHelperImp.TAT_Target_SPECIALTY)
                    {
                        TatSummaryRow summaryRow = new TatSummaryRow
                        {
                            OrderNbr = groupedDataRows.First().OrderNbr,
                            Facility = groupedDataRows.First().Facility,
                            RxNbr = groupedDataRows.First().RxNbr,
                            RefillNbr = groupedDataRows.First().RefillNbr,
                            DateIn = groupedDataRows.Where(r => r.DateInRank == 1).First().DateIn,
                            DateOut = groupedDataRows.Where(r => r.DateOutRank == 1).Last().DateOut,
                            DaysOverall = daysOverall,
                            DeductDaysIntervention = deductDaysIntervention,
                            DeductDaysOffHours = deductDaysOffHours,
                            DaysNetTat = daysNetTat,
                            HasExceptions = hasExceptions
                        };

                        summaryRows.Add(summaryRow);
                        break;
                    }
                }
            }

            return summaryRows
                .OrderByDescending(r => r.DeductDaysOffHours)
                .ThenBy(r => r.OrderNbr)
                .ThenBy(r => r.Facility)
                .ThenBy(r => r.RxNbr)
                .ThenBy(r => r.RefillNbr);
        }

        /// <inheritdoc />
        public IEnumerable<TatSummaryMaxRxRow> GetOneRxWithinAnOrderHavingLargestDaysNetTat(IEnumerable<TatSummaryRow> summaryRows)
        {
            List<TatSummaryMaxRxRow> summaryMaxRxRows = new List<TatSummaryMaxRxRow>();

            foreach (var groupedSummaryRows in summaryRows
                .Where(r => r.RxNbr.IndexOf("RR") == -1) //exclude refill requests
                .GroupBy(row => new
                {
                    row.OrderNbr,
                    row.Facility
                }))
            {
                TatSummaryRow rxWithLargestDaysNetTat = groupedSummaryRows
                    .OrderByDescending(r => r.DaysNetTat)
                    .ThenBy(r => r.RxNbr)
                    .ThenBy(r => r.RefillNbr)
                    .First();

                TatSummaryMaxRxRow summaryMaxRow = new TatSummaryMaxRxRow
                {
                    OrderNbr = rxWithLargestDaysNetTat.OrderNbr,
                    Facility = rxWithLargestDaysNetTat.Facility,
                    DateIn = rxWithLargestDaysNetTat.DateIn,
                    DateOut = rxWithLargestDaysNetTat.DateOut,
                    DaysOverall = rxWithLargestDaysNetTat.DaysOverall,
                    DeductDaysIntervention = rxWithLargestDaysNetTat.DeductDaysIntervention,
                    DeductDaysOffHours = rxWithLargestDaysNetTat.DeductDaysOffHours,
                    DaysNetTat = rxWithLargestDaysNetTat.DaysNetTat,
                    CountRxInOrder = groupedSummaryRows.Select(r => new { r.RxNbr, r.RefillNbr }).Distinct().Count(),
                    HasExceptions = groupedSummaryRows.Any(r => r.HasExceptions == "Y") ? "Y" : "N"
                };

                summaryMaxRxRows.Add(summaryMaxRow);
            }

            return summaryMaxRxRows;
        }

        public IEnumerable<TpcDataRow> MapThirdPartyClaimBase(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<TpcDataRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new TpcDataRow
                {
                    BenefitGroupId = row.NullableString(0, rowNumber) ?? string.Empty,
                    DateOfService = row.NullableField<DateTime>(1, rowNumber),
                    DawCode = row.NullableString(2, rowNumber) ?? string.Empty,
                    DaysSupply = row.NullableField<decimal>(3, rowNumber),
                    DispensedDrugNumber = row.NullableField<decimal>(4, rowNumber),
                    DispensedQtyAwp = row.NullableField<decimal>(5, rowNumber),
                    FacilityNum = row.NullableField<decimal>(6, rowNumber),
                    FillStateChgTs = row.NullableField<DateTime>(7, rowNumber),
                    FillStateCode = row.NullableString(8, rowNumber) ?? string.Empty,
                    FillStateKey = row.NullableField<decimal>(9, rowNumber),
                    FillStatePriceNum = row.NullableField<decimal>(10, rowNumber),
                    FinalPrice = row.NullableField<decimal>(11, rowNumber),
                    IncentiveFeeAmount = row.NullableField<decimal>(12, rowNumber),
                    InternalFillId = row.NullableField<decimal>(13, rowNumber),
                    IsSameDayReversal = row.NullableString(14, rowNumber) ?? string.Empty,
                    MedicalConditionNum = row.NullableField<decimal>(15, rowNumber),
                    OriginalProductQty = row.NullableField<decimal>(16, rowNumber),
                    PartialFillSeq = row.NullableField<decimal>(17, rowNumber),
                    PatientNum = row.NullableField<decimal>(18, rowNumber),
                    PatientPayAmount = row.NullableField<decimal>(19, rowNumber),
                    PrescribedDate = row.NullableField<DateTime>(20, rowNumber),
                    PrescriberAddressNum = row.NullableField<decimal>(21, rowNumber),
                    PrescriberIdDea = row.NullableString(22, rowNumber) ?? string.Empty,
                    PrescriberIdNpi = row.NullableString(23, rowNumber) ?? string.Empty,
                    PrescriberNum = row.NullableField<decimal>(24, rowNumber),
                    PrescriberState = row.NullableString(25, rowNumber) ?? string.Empty,
                    PrescriberZip = row.NullableString(26, rowNumber) ?? string.Empty,
                    QtyDispensed = row.NullableField<decimal>(27, rowNumber),
                    RefillNum = row.NullableField<decimal>(28, rowNumber),
                    RefillAuthorized = row.NullableField<decimal>(29, rowNumber),
                    RefillsRemaining = row.NullableField<decimal>(30, rowNumber),
                    RxFillSeq = row.NullableField<decimal>(31, rowNumber),
                    RxNumber = row.NullableString(32, rowNumber) ?? string.Empty,
                    RxOriginCode = row.NullableString(33, rowNumber) ?? string.Empty,
                    RxRecordNum = row.NullableField<decimal>(34, rowNumber),
                    ShortFillStatus = row.NullableString(35, rowNumber) ?? string.Empty,
                    SplitBillingCode = row.NullableString(36, rowNumber) ?? string.Empty,
                    TlogSequenceNum = row.NullableField<decimal>(37, rowNumber),
                    TotalCost = row.NullableField<decimal>(38, rowNumber),
                    TotalFee = row.NullableField<decimal>(39, rowNumber),
                    TotalUandc = row.NullableField<decimal>(40, rowNumber),
                    TotalUserDefined = row.NullableField<decimal>(41, rowNumber),
                    TpCurrPatientPayAmount = row.NullableField<decimal>(42, rowNumber),
                    TpCurrTotalCost = row.NullableField<decimal>(43, rowNumber),
                    TpCurrTotalFee = row.NullableField<decimal>(44, rowNumber),
                    TpItemClaimKey = row.NullableField<decimal>(45, rowNumber),
                    TpItemClaimNum = row.NullableField<decimal>(46, rowNumber),
                    TpMessageKey = row.NullableField<decimal>(47, rowNumber),
                    TpPatientNum = row.NullableField<decimal>(48, rowNumber),
                    TpPlanAmountPaid = row.NullableField<decimal>(49, rowNumber),
                    TpPlanNumber = row.NullableField<decimal>(50, rowNumber),
                    TpPriorAuthNum = row.NullableField<decimal>(51, rowNumber),
                    TpProcessorDestNum = row.NullableField<decimal>(52, rowNumber),
                    TransactionCode = row.NullableString(53, rowNumber) ?? string.Empty,
                    FsFillStateChgTs = row.NullableField<DateTime>(54, rowNumber),
                    FsFillStateCode = row.NullableField<decimal>(55, rowNumber),
                    FsRxFillSeq = row.NullableField<decimal>(56, rowNumber),
                    FsRxRecordNum = row.NullableField<decimal>(57, rowNumber),
                    IngredientNdc1 = row.NullableString(58, rowNumber) ?? string.Empty,
                    IngredientNdc2 = row.NullableString(59, rowNumber) ?? string.Empty,
                    IngredientNdc3 = row.NullableString(60, rowNumber) ?? string.Empty,
                    IngredientNdc4 = row.NullableString(61, rowNumber) ?? string.Empty,
                    IngredientNdc5 = row.NullableString(62, rowNumber) ?? string.Empty,
                    IngredientNdc6 = row.NullableString(63, rowNumber) ?? string.Empty,
                    IngredientPrice1 = row.NullableField<decimal>(64, rowNumber),
                    IngredientPrice2 = row.NullableField<decimal>(65, rowNumber),
                    IngredientPrice3 = row.NullableField<decimal>(66, rowNumber),
                    IngredientPrice4 = row.NullableField<decimal>(67, rowNumber),
                    IngredientPrice5 = row.NullableField<decimal>(68, rowNumber),
                    IngredientPrice6 = row.NullableField<decimal>(69, rowNumber),
                    IngredientQty1 = row.NullableField<decimal>(70, rowNumber),
                    IngredientQty2 = row.NullableField<decimal>(71, rowNumber),
                    IngredientQty3 = row.NullableField<decimal>(72, rowNumber),
                    IngredientQty4 = row.NullableField<decimal>(73, rowNumber),
                    IngredientQty5 = row.NullableField<decimal>(74, rowNumber),
                    IngredientQty6 = row.NullableField<decimal>(75, rowNumber),
                    BirthDate = row.NullableField<DateTime>(76, rowNumber),
                    PFirstName = row.NullableString(77, rowNumber) ?? string.Empty,
                    PGender = row.NullableString(78, rowNumber) ?? string.Empty,
                    PLastName = row.NullableString(79, rowNumber) ?? string.Empty,
                    PMiddleName = row.NullableString(80, rowNumber) ?? string.Empty,
                    Address1 = row.NullableString(81, rowNumber) ?? string.Empty,
                    Address2 = row.NullableString(82, rowNumber) ?? string.Empty,
                    City = row.NullableString(83, rowNumber) ?? string.Empty,
                    PaFirstName = row.NullableString(84, rowNumber) ?? string.Empty,
                    PaGender = row.NullableString(85, rowNumber) ?? string.Empty,
                    PaLastName = row.NullableString(86, rowNumber) ?? string.Empty,
                    PaMiddleName = row.NullableString(87, rowNumber) ?? string.Empty,
                    PhoneNum = row.NullableString(88, rowNumber) ?? string.Empty,
                    State = row.NullableString(89, rowNumber) ?? string.Empty,
                    Zip = row.NullableString(90, rowNumber) ?? string.Empty,
                    AuthNumber = row.NullableString(91, rowNumber) ?? string.Empty,
                    EmergencyConditionCode = row.NullableString(92, rowNumber) ?? string.Empty,
                    ExternalBillingIndicator = row.NullableString(93, rowNumber) ?? string.Empty,
                    HLevel = row.NullableField<decimal>(94, rowNumber),
                    TpMessageNum = row.NullableField<decimal>(95, rowNumber),
                    BinNum = row.NullableString(96, rowNumber) ?? string.Empty,
                    ProcessorControlNum = row.NullableString(97, rowNumber) ?? string.Empty,
                    RsBasisOfReimbursmentDet = row.NullableField<decimal>(98, rowNumber),
                    RsNetworkReimbursmentId = row.NullableString(99, rowNumber) ?? string.Empty,
                    DrugGcnSeq = row.NullableString(100, rowNumber) ?? string.Empty,
                    DrugIsMaintDrug = row.NullableString(101, rowNumber) ?? string.Empty,
                    DrugNdc = row.NullableString(102, rowNumber) ?? string.Empty,
                    DrugPackageSize = row.NullableField<decimal>(103, rowNumber),
                    DrugStrength = row.NullableString(104, rowNumber) ?? string.Empty,
                    DrugStrengthUnits = row.NullableString(105, rowNumber) ?? string.Empty,
                    FdFacilityId = row.NullableString(106, rowNumber) ?? string.Empty,
                    FdNcpdpProvider = row.NullableString(107, rowNumber) ?? string.Empty,
                    McdFrmttdCondRefNum = row.NullableString(108, rowNumber) ?? string.Empty,
                    PresFirstName = row.NullableString(109, rowNumber) ?? string.Empty,
                    PresLastName = row.NullableString(110, rowNumber) ?? string.Empty,
                    PrsPrescriberKey = row.NullableField<decimal>(111, rowNumber),
                    PrsPrescirberNum = row.NullableField<decimal>(112, rowNumber),
                    PadrKey = row.NullableField<decimal>(113, rowNumber),
                    PadrAddress1 = row.NullableString(114, rowNumber) ?? string.Empty,
                    PadrAddress2 = row.NullableString(115, rowNumber) ?? string.Empty,
                    PadrCity = row.NullableString(116, rowNumber) ?? string.Empty,
                    PrdIsCompound = row.NullableString(117, rowNumber) ?? string.Empty,
                    PrdIsGeneric = row.NullableString(118, rowNumber) ?? string.Empty,
                    PrdIsPriceMaintained = row.NullableString(119, rowNumber) ?? string.Empty,
                    PrdName = row.NullableString(120, rowNumber) ?? string.Empty,
                    PrdPackageSize = row.NullableField<decimal>(121, rowNumber),
                    PdPatientKey = row.NullableField<decimal>(122, rowNumber),
                    TpdCardholderId = row.NullableString(123, rowNumber) ?? string.Empty,
                    TpdCardHolderPatientNum = row.NullableField<decimal>(124, rowNumber),
                    TpdRelationshipNum = row.NullableField<decimal>(125, rowNumber),
                    TpdTpPlanNum = row.NullableField<decimal>(126, rowNumber),
                    TpldPlanCode = row.NullableString(127, rowNumber) ?? string.Empty,
                    RequestMessage = row.NullableString(128, rowNumber) ?? string.Empty,
                    LookUpTransmissionDate = row.NullableField<DateTime>(129, rowNumber),
                    LookUpFirstAdjudicatedDate = row.NullableField<DateTime>(130, rowNumber),
                    LookUpFirstDispenseDate = row.NullableField<DateTime>(131, rowNumber),
                    LookUpQtyDispensed = row.NullableField<decimal>(132, rowNumber),
                    LookUpDaysSupply = row.NullableField<decimal>(133, rowNumber),
                    LookUpAcquisitionCost = row.NullableField<decimal>(134, rowNumber),
                    LookUpCfStoreIndicator = row.NullableString(135, rowNumber) ?? string.Empty,
                    IsSenior = row.NullableString(136, rowNumber) ?? string.Empty,
                    OrderNum = row.NullableField<decimal>(137, rowNumber)
                });

                rowNumber++;
            }
            return result;
        }

        public IEnumerable<decimal?> MapThirdPartyClaimsAcquisitionCost(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<decimal?> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(row.NullableField<decimal>(0, rowNumber));
                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryRawDataRow> MapSureScriptsMedicalHistoryRawData(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<SureScriptsMedicalHistoryRawDataRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new SureScriptsMedicalHistoryRawDataRow
                {
                    RecordSequenceNumber = row.NullableField<long>(0, rowNumber),
                    ParticipantPatientId = row.NullableField<long>(1, rowNumber),
                    PatientLastName = row.NullableString(2, rowNumber),
                    PatientFirstName = row.NullableString(3, rowNumber),
                    PatientMiddleName = row.NullableString(4, rowNumber),
                    PatientPrefix = row.NullableString(5, rowNumber),
                    PatientSuffix = row.NullableString(6, rowNumber),
                    PatientDateOfBirth = row.NullableField<DateTime>(7, rowNumber),
                    PatientGender = row.NullableString(8, rowNumber),
                    PatientAddress1 = row.NullableString(9, rowNumber),
                    PatientCity = row.NullableString(10, rowNumber),
                    PatientState = row.NullableString(11, rowNumber),
                    PatientZipCode = row.NullableString(12, rowNumber),
                    NcpdpId = row.NullableString(13, rowNumber),
                    ChainSiteId = row.NullableString(14, rowNumber),
                    PharmacyName = row.NullableString(15, rowNumber),
                    FacilityAddress1 = row.NullableString(16, rowNumber),
                    FacilityCity = row.NullableString(17, rowNumber),
                    FacilityState = row.NullableString(18, rowNumber),
                    FacilityZipCode = row.NullableString(19, rowNumber),
                    FacilityPhoneNumber = row.NullableString(20, rowNumber),
                    FPrimaryCareProviderLastName = row.NullableString(21, rowNumber),
                    PrimaryCareProviderFirstName = row.NullableString(22, rowNumber),
                    PrimaryCareProviderAddress1 = row.NullableString(23, rowNumber),
                    PrimaryCareProviderCity = row.NullableString(24, rowNumber),
                    PrimaryCareProviderState = row.NullableString(25, rowNumber),
                    PrimaryCareProviderZipCode = row.NullableString(26, rowNumber),
                    PrimaryCareProviderAreaCode = row.NullableString(27, rowNumber),
                    PrimaryCareProviderPhoneNumber = row.NullableString(28, rowNumber),
                    PrescriptionNumber = row.NullableString(29, rowNumber),
                    FillNumber = row.NullableField<long>(30, rowNumber),
                    NdcNumberDispensed = row.NullableString(31, rowNumber),
                    MedicationName = row.NullableString(32, rowNumber),
                    QuantityPrescribed = row.NullableField<decimal>(33, rowNumber),
                    QuantityDispensed = row.NullableField<decimal>(34, rowNumber),
                    DaysSupply = row.NullableField<long>(35, rowNumber),
                    SigText = row.NullableString(36, rowNumber),
                    DateWritten = row.NullableField<int>(37, rowNumber),
                    DateFilled = row.NullableField<int>(38, rowNumber),
                    DatePickedUpDispensed = row.NullableField<decimal>(39, rowNumber),
                    RefillsOriginallyAuthorized = row.NullableField<decimal>(40, rowNumber),
                    RefillsRemaining = row.NullableField<decimal>(41, rowNumber),
                    Logic_FillFactKey = row.NullableField<long>(42, rowNumber),
                    Logic_PdPatientKey = row.NullableField<long>(43, rowNumber),
                    Logic_PatientAddressUsage = row.NullableString(44, rowNumber),
                    Logic_PatientAddressCreateDate = row.NullableField<DateTime>(45, rowNumber),
                    Logic_PresPhoneKey = row.NullableField<long>(46, rowNumber),
                    Logic_PresPhoneStatus = row.NullableString(47, rowNumber),
                    Logic_PresPhoneSourceCode = row.NullableString(48, rowNumber),
                    Logic_PresPhoneHLevel = row.NullableField<decimal>(49, rowNumber)
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> MapSureScriptsPhysicianNotificationLettersRawData(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<SureScriptsPhysicianNotificationLetterRawDataRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new SureScriptsPhysicianNotificationLetterRawDataRow
                {
                    RecordSequenceNumber = row.NullableField<long>(0, rowNumber),
                    ParticipantPatientId = row.NullableField<long>(1, rowNumber),
                    PatientLastName = row.NullableString(2, rowNumber),
                    PatientFirstName = row.NullableString(3, rowNumber),
                    PatientMiddleName = row.NullableString(4, rowNumber),
                    PatientPhoneAreaCode = row.NullableString(5, rowNumber),
                    PatientPhoneNumber = row.NullableString(6, rowNumber),
                    PatientDateOfBirth = row.NullableField<DateTime>(7, rowNumber),
                    PatientGender = row.NullableString(8, rowNumber),
                    PatientAddress1 = row.NullableString(9, rowNumber),
                    PatientAddress2 = row.NullableString(10, rowNumber),
                    PatientCity = row.NullableString(11, rowNumber),
                    PatientState = row.NullableString(12, rowNumber),
                    PatientZipCode = row.NullableString(13, rowNumber),
                    PrimaryCareProviderNpi = row.NullableString(14, rowNumber),
                    InternalPrimaryCareProviderId = row.NullableField<long>(15, rowNumber),
                    TransactionPrecriberLastName = row.NullableString(16, rowNumber),
                    PrimaryCareProviderLastName = row.NullableString(17, rowNumber),
                    TransactionPrecriberFirstName = row.NullableString(18, rowNumber),
                    PrimaryCareProviderFirstName = row.NullableString(19, rowNumber),
                    PrimaryCareProviderAddress1 = row.NullableString(20, rowNumber),
                    PrimaryCareProviderCity = row.NullableString(21, rowNumber),
                    PrimaryCareProviderState = row.NullableString(22, rowNumber),
                    PrimaryCareProviderZipCode = row.NullableString(23, rowNumber),
                    PrimaryCareProviderPhoneAreaCode = row.NullableString(24, rowNumber),
                    PrimaryCareProviderPhoneNumber = row.NullableString(25, rowNumber),
                    PrimaryCareProviderFaxAreaCode = row.NullableString(26, rowNumber),
                    PrimaryCareProviderFaxNumber = row.NullableString(27, rowNumber),
                    VaccineCvxCode = row.NullableString(28, rowNumber),
                    VaccineManufacturerName = row.NullableString(29, rowNumber),
                    VaccineName = row.NullableString(30, rowNumber),
                    VaccineLotNumber = row.NullableString(31, rowNumber),
                    VaccineExpirationDate = row.NullableField<DateTime>(32, rowNumber),
                    VaccineInformationStatementName = row.NullableString(33, rowNumber),
                    AdministeredDate = row.NullableField<decimal>(34, rowNumber),
                    AdministeredAmount = row.NullableField<decimal>(35, rowNumber),
                    AdministeredUnits = row.NullableString(36, rowNumber),
                    RouteOfAdministrationDescription = row.NullableString(37, rowNumber),
                    FacilityNpi = row.NullableString(38, rowNumber),
                    OtherFacilityIdentification = row.NullableString(39, rowNumber),
                    FacilityAddress1 = row.NullableString(40, rowNumber),
                    FacilityAddress2 = row.NullableString(41, rowNumber),
                    FacilityCity = row.NullableString(42, rowNumber),
                    FacilityState = row.NullableString(43, rowNumber),
                    FacilityZipCode = row.NullableString(44, rowNumber),
                    FacilityPhoneNumber = row.NullableString(45, rowNumber),
                    ProfessionalConsultationComplete = row.NullableString(46, rowNumber),
                    Logic_DrugNdc = row.NullableString(47, rowNumber),
                    Logic_FillFactKey = row.NullableField<long>(48, rowNumber),
                    Logic_FillFactPatientKey = row.NullableField<long>(49, rowNumber),
                    Logic_FacilityIdNum = row.NullableField<long>(50, rowNumber),
                    Logic_FacilityIdDatestamp = row.NullableField<DateTime>(51, rowNumber),
                    Logic_PatientAddrUsage = row.NullableString(52, rowNumber),
                    Logic_PatientAddrCreateDate = row.NullableField<DateTime>(53, rowNumber),
                    Logic_PresPhoneKey = row.NullableField<long>(54, rowNumber),
                    Logic_PresPhoneStatus = row.NullableString(55, rowNumber),
                    Logic_PresPhoneSourceCode = row.NullableString(56, rowNumber),
                    Logic_PresPhoneHLevel = row.NullableField<decimal>(57, rowNumber),
                    Logic_PresFaxKey = row.NullableField<long>(58, rowNumber),
                    Logic_PresFaxStatus = row.NullableString(59, rowNumber),
                    Logic_PresFaxSourceCode = row.NullableString(60, rowNumber),
                    Logic_PresFaxHLevel = row.NullableField<decimal>(61, rowNumber),
                    Logic_PresNpiPresKey = row.NullableField<long>(62, rowNumber),
                    Logic_PresNpiHLevel = row.NullableField<decimal>(63, rowNumber),
                    Logic_FpfOrderNum = row.NullableField<long>(64, rowNumber),
                    Logic_FpfRespPartyKey = row.NullableField<decimal>(65, rowNumber)
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryReportRow> MapSureScriptsMedicalHistoryRawDataToFinalReport(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows)
        {
            List<SureScriptsMedicalHistoryReportRow> reportRows = new List<SureScriptsMedicalHistoryReportRow>();

            //TODO: (See User Story 595600) Develop _sureScriptsHelper to implement business logic in small unit-testable methods.

            foreach (var dataRow in dataRows)
            {
                SureScriptsMedicalHistoryReportRow reportRow = new()
                {
                    RecordType = "PEF",
                    RecordSequenceNumber = dataRow.RecordSequenceNumber,
                    ParticipantPatientId = dataRow.ParticipantPatientId,
                    AlternatePatientIdQualifier = null,
                    AlternatePatientId = null,
                    PatientLocationCode = null,
                    PatientLastName = _sureScriptsHelper.SetDefaultString(dataRow.PatientLastName),
                    PatientFirstName = _sureScriptsHelper.SetDefaultString(dataRow.PatientFirstName),
                    PatientMiddleName = _sureScriptsHelper.SetDefaultString(dataRow.PatientMiddleName),
                    PatientPrefix = _sureScriptsHelper.SetDefaultString(dataRow.PatientPrefix),
                    PatientSuffix = _sureScriptsHelper.SetDefaultString(dataRow.PatientSuffix),
                    PatientDob = _sureScriptsHelper.FormatDateString(_sureScriptsHelper.SetDefaultDate(dataRow.PatientDateOfBirth)),
                    PatientGender = _sureScriptsHelper.SetDefaultString(dataRow.PatientGender),
                    PatientAddress1 = _sureScriptsHelper.SetDefaultString(dataRow.PatientAddress1),
                    PatientAddress2 = null,
                    PatientCity = _sureScriptsHelper.SetDefaultString(dataRow.PatientCity),
                    PatientState = _sureScriptsHelper.SetDefaultString(dataRow.PatientState),
                    PatientZipCode = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.PatientZipCode)),
                    PatientCountry = null,
                    PatientPhoneNumber = null,
                    PatientActiveIndicator = 0,
                    PdmpIndicator = 0,
                    DrugRecordIndicator = 0,
                    Allergy = null,
                    DiagnosisCodeQualifier = null,
                    DiagnosisCode = null,
                    SpeciesCode = null,
                    NameOfAnimal = null,
                    NcpdpId = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.NcpdpId)),
                    ChainSiteId = _sureScriptsHelper.SetDefaultString(dataRow.ChainSiteId),
                    PharmacyType = null,
                    PharmacyName = _sureScriptsHelper.SetDefaultString(dataRow.PharmacyName),
                    RxOriginCode = null,
                    PharmacyDea = null,
                    PharmacyDeaSuffix = null,
                    PharmacyNpi = null,
                    FacilityAddress1 = _sureScriptsHelper.SetDefaultString(dataRow.FacilityAddress1),
                    FacilityAddress2 = null,
                    FacilityCity = _sureScriptsHelper.SetDefaultString(dataRow.FacilityCity),
                    FacilityState = _sureScriptsHelper.SetDefaultString(dataRow.FacilityState),
                    FacilityZipCode = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.FacilityZipCode)),
                    FacilityPhoneNumber = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.FacilityPhoneNumber)),
                    ContactName = null,
                    PharmacistNpi = null,
                    PharmacistLastName = null,
                    PharmacistFirstName = null,
                    PharmacistMiddleName = null,
                    PharmacistStateLicense = null,
                    StateCodeIssuingRxSerialNumber = null,
                    StateIssuedRxSerialNumber = null,
                    PharmacyBoardLicenseNumber = null,
                    PharmacyAlternateLicenseNumber = null,
                    CountyCode = null,
                    PrescriberOrderNumber = null,
                    PrescriberSpi = null,
                    PrescriberNpi = null,
                    AlternatePrescriberId = null,
                    PrescriberDea = null,
                    DeaSuffix = null,
                    StateLicenseNumber = null,
                    PrimaryCareProviderLastName = _sureScriptsHelper.SetNullableValue(_sureScriptsHelper.SetDefaultString(dataRow.FPrimaryCareProviderLastName), "NotAvailable"),
                    PrimaryCareProviderFirstName = _sureScriptsHelper.SetDefaultString(dataRow.PrimaryCareProviderFirstName),
                    PrimaryCareProviderMiddleName = null,
                    PrimaryCareProviderPrefix = null,
                    PrimaryCareProviderSuffix = null,
                    PrimaryCareProviderAddress1 = _sureScriptsHelper.SetNullableValue(_sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetStringToMaxLength(dataRow.PrimaryCareProviderAddress1, 35)), "NotAvailable"),
                    PrimaryCareProviderAddress2 = null,
                    PrimaryCareProviderCity = _sureScriptsHelper.SetDefaultString(dataRow.PrimaryCareProviderCity),
                    PrimaryCareProviderState = _sureScriptsHelper.SetDefaultString(dataRow.PrimaryCareProviderState),
                    PrimaryCareProviderZipCode = _sureScriptsHelper.SetDefaultString(dataRow.PrimaryCareProviderZipCode),
                    PrimaryCareProviderPhoneNumber = _sureScriptsHelper.SetDefaultString(dataRow.PrimaryCareProviderAreaCode + _sureScriptsHelper.SetDigitsOnly(dataRow.PrimaryCareProviderPhoneNumber)),
                    PrimaryCareProviderFaxNumber = null,
                    PrescriptionNumber = _sureScriptsHelper.SetDefaultString(dataRow.PrescriptionNumber),
                    FillNumber = _sureScriptsHelper.LPad(_sureScriptsHelper.SetDefaultString(dataRow.FillNumber.ToString()), 2, '0'),
                    PartialFillIndicator = null,
                    NdcNumberDispensed = _sureScriptsHelper.SetDefaultString(dataRow.NdcNumberDispensed),
                    DrugClass = null,
                    MedicationName = _sureScriptsHelper.SetDefaultString(dataRow.MedicationName),
                    QuantityPrescribed = _sureScriptsHelper.RemoveTrailingZeroFromDecimal(_sureScriptsHelper.DispensedQuantityModifier(_sureScriptsHelper.SetNullableValue(dataRow.QuantityPrescribed, 0))),
                    QuantityDispensed = _sureScriptsHelper.RemoveTrailingZeroFromDecimal(_sureScriptsHelper.DispensedQuantityModifier(_sureScriptsHelper.SetNullableValue(dataRow.QuantityDispensed, 0))),
                    QualifierUnitsOfMeasure = null,
                    DaysSupply = _sureScriptsHelper.SetNullableValue(dataRow.DaysSupply, 0),
                    BasisOfDaysSupply = null,
                    SigText = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetStringToMaxLength(dataRow.SigText, 140)),
                    DateWritten = dataRow.DateWritten,
                    DateAdjudicated = null,
                    DateFilled = dataRow.DateFilled,
                    DateDispensed = dataRow.DatePickedUpDispensed,
                    RefillsOriginallyAuthorized = _sureScriptsHelper.SetNullableValue(dataRow.RefillsOriginallyAuthorized, 0),
                    RefillsRemaining = _sureScriptsHelper.SetRefillsRemaining(dataRow.RefillsRemaining),
                    RxNormCode = null,
                    ElectronicPrescriptionOrderNumber = null,
                    CompoundIngredientSequenceNumber = _sureScriptsHelper.DeriveCompoundIngredientSequenceNumber(dataRow.NdcNumberDispensed, dataRow.Logic_PrescriptionFillRank),
                    CompoundIngredientId = null,
                    CompoundIngredientQuantity = null,
                    CompoundIngredientUnitsOfMeasure = null,
                    CompoundIngredientMedicationName = null,
                    CompoundIngredientDrugClass = null,
                    IssuingJurisdiction = null,
                    IdQualifier = null,
                    DropOffPickUpPersonId = null,
                    DropOffPickUpPersonRelationship = null,
                    RxDropOffPickUpLastName = null,
                    RxDropOffPickUpFirstName = null,
                    Message = null,
                    RxNormProductQualifier = null,
                    ElectronicRxReferenceNumber = null,
                    DropOffPickUpQualifier = null,
                    PartialFillSequenceNumber = null
                };

                reportRows.Add(reportRow);
            }

            return reportRows;
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> MapSureScriptsPhysicianNotificationLetterRawDataToFinalReport(IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> dataRows, string storeStateForReport)
        {
            List<SureScriptsPhysicianNotificationLetterReportRow> reportRows = [];

            foreach (SureScriptsPhysicianNotificationLetterRawDataRow dataRow in dataRows)
            {
                bool isStatePA = _sureScriptsHelper.IsStatePA(dataRow.FacilityState, storeStateForReport);
                long? internalPrimaryCareProviderId = null;
                if (long.TryParse(_sureScriptsHelper.SetDefaultStringConsideringState(dataRow.InternalPrimaryCareProviderId.ToString(), isStatePA), out long parsed))
                {
                    internalPrimaryCareProviderId = parsed;
                }

                SureScriptsPhysicianNotificationLetterReportRow reportRow = new()
                {
                    RecordType = null,
                    RecordSequenceNumber = dataRow.RecordSequenceNumber,
                    ParticipantPatientId = dataRow.ParticipantPatientId,
                    PatientIdIssuer = null,
                    PatientLastName = _sureScriptsHelper.SetDefaultString(dataRow.PatientLastName),
                    PatientFirstName = _sureScriptsHelper.SetDefaultString(dataRow.PatientFirstName),
                    PatientMiddleName = _sureScriptsHelper.SetDefaultString(dataRow.PatientMiddleName),
                    PatientPhoneNumber = _sureScriptsHelper.SetDefaultString(dataRow.PatientPhoneAreaCode + _sureScriptsHelper.SetDigitsOnly(dataRow.PatientPhoneNumber)),
                    PatientDob = _sureScriptsHelper.SetDefaultDate(dataRow.PatientDateOfBirth),
                    PatientGender = _sureScriptsHelper.SetNullableValue(dataRow.PatientGender, "U"),
                    PatientAddress1 = _sureScriptsHelper.SetDefaultString(dataRow.PatientAddress1),
                    PatientAddress2 = _sureScriptsHelper.SetDefaultString(dataRow.PatientAddress2),
                    PatientCity = _sureScriptsHelper.SetDefaultString(dataRow.PatientCity),
                    PatientState = _sureScriptsHelper.SetDefaultString(dataRow.PatientState),
                    PatientZipCode = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.PatientZipCode)),
                    PrimaryCareProviderNpi = _sureScriptsHelper.SetDefaultStringConsideringState(dataRow.PrimaryCareProviderNpi, isStatePA),
                    PrimaryCareProviderStateLicenseNumber = null,
                    InternalPrimaryCareProviderId = internalPrimaryCareProviderId,
                    PrimaryCareProviderLastName = _sureScriptsHelper.SetDefaultStringConsideringState(dataRow.PrimaryCareProviderLastName, isStatePA),
                    PrimaryCareProviderFirstName = _sureScriptsHelper.SetDefaultStringConsideringState(dataRow.PrimaryCareProviderFirstName, isStatePA),
                    PrimaryCareProviderAddress1 = _sureScriptsHelper.SetDefaultStringConsideringState(_sureScriptsHelper.SetNullableValue(dataRow.PrimaryCareProviderAddress1, "NotAvailable"), isStatePA),
                    PrimaryCareProviderAddress2 = null,
                    PrimaryCareProviderCity = _sureScriptsHelper.SetDefaultStringConsideringState(dataRow.PrimaryCareProviderCity, isStatePA),
                    PrimaryCareProviderState = _sureScriptsHelper.SetDefaultStringConsideringState(dataRow.PrimaryCareProviderState, isStatePA),
                    PrimaryCareProviderZipCode = _sureScriptsHelper.SetDefaultStringConsideringState(_sureScriptsHelper.SetDigitsOnly(dataRow.PrimaryCareProviderZipCode), isStatePA),
                    PrimaryCareProviderPhoneNumber = _sureScriptsHelper.SetDefaultStringConsideringState(_sureScriptsHelper.SetDigitsOnly(dataRow.PrimaryCareProviderPhoneAreaCode + dataRow.PrimaryCareProviderPhoneNumber), isStatePA),
                    PrimaryCareProviderFaxNumber = _sureScriptsHelper.SetDefaultStringConsideringState(_sureScriptsHelper.SetDigitsOnly(dataRow.PrimaryCareProviderFaxNumber + dataRow.PrimaryCareProviderFaxAreaCode), isStatePA),
                    VaccineCptCode = null,
                    VaccineCvxCode = _sureScriptsHelper.SetVaccineCvxCode(dataRow.VaccineCvxCode, dataRow.VaccineInformationStatementName, dataRow.VaccineName, dataRow.Logic_DrugNdc),
                    VaccineName = _sureScriptsHelper.SetVaccineName(dataRow.Logic_DrugNdc, dataRow.VaccineName),
                    VaccineManufacturerName = _sureScriptsHelper.SetVaccineManufacturerName(dataRow.Logic_DrugNdc, dataRow.VaccineManufacturerName),
                    VaccineManufacturerCode = null,
                    VaccineLotNumber = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetNullableValue(dataRow.VaccineLotNumber, "NotAvailable")),
                    VaccineExpirationDate = _sureScriptsHelper.SetDefaultDate(dataRow.VaccineExpirationDate),
                    VaccineInformationStatementName = _sureScriptsHelper.SetDefaultString(dataRow.VaccineInformationStatementName),
                    VaccineInformationStatementDate = null,
                    AdministeredDate = dataRow.AdministeredDate,
                    DataEntryDate = null,
                    AdministeredAmount = dataRow.AdministeredAmount,
                    AdministeredUnits = dataRow.AdministeredUnits,
                    AdministeredUnitsCode = null,
                    RouteOfAdministrationDescription = _sureScriptsHelper.SetRouteOfAdministration(dataRow.VaccineInformationStatementName, dataRow.RouteOfAdministrationDescription),
                    RouteOfAdministrationCode = null,
                    SiteOfAdministrationDescription = _sureScriptsHelper.SetSiteOfAdministration(dataRow.VaccineInformationStatementName, dataRow.RouteOfAdministrationDescription),
                    SiteOfAdministrationCode = null,
                    DoseNumberInSeries = null,
                    NumberOfDosesInSeries = null,
                    AdministeringProviderInternalId = null,
                    AdministeringProviderNpi = null,
                    AdministeringProviderLastName = null,
                    AdministeringProviderFirstName = null,
                    AdministeringProviderStateLicenseNumber = null,
                    FacilityNpi = _sureScriptsHelper.SetDefaultString(dataRow.FacilityNpi),
                    FacilityIdentification = _sureScriptsHelper.SetDefaultString(dataRow.OtherFacilityIdentification),
                    FacilityAddress1 = _sureScriptsHelper.SetDefaultString(dataRow.FacilityAddress1),
                    FacilityAddress2 = _sureScriptsHelper.SetDefaultString(dataRow.FacilityAddress2),
                    FacilityCity = _sureScriptsHelper.SetDefaultString(dataRow.FacilityCity),
                    FacilityState = _sureScriptsHelper.SetDefaultString(dataRow.FacilityState),
                    FacilityZipCode = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.FacilityZipCode)),
                    FacilityPhoneNumber = _sureScriptsHelper.SetDefaultString(_sureScriptsHelper.SetDigitsOnly(dataRow.FacilityPhoneNumber)),
                    NotificationLetterConfigurationCode = null,
                    SourceSystemId = null,
                    SignedConsentForMinor = null,
                    ProfessionalConsultationComplete = _sureScriptsHelper.SetDefaultString(dataRow.ProfessionalConsultationComplete),
                    EligibilityConsultationComplete = null,
                    CustomerDefinedElement1 = null,
                    CustomerDefinedElement2 = null,
                    CustomerDefinedElement3 = null,
                    CustomerDefinedElement4 = null,
                    FacilityIdRank = null,
                    PatientPhoneRank = null,
                    PatientAddressRank = null,
                    PrescriberPhoneRank = null,
                    PrescriberPcpFaxRank = null,
                    PrescriberNpiRank = null,
                    FillPricingFactRank = null,
                    FillFactKey = dataRow.Logic_FillFactKey.HasValue ? dataRow.Logic_FillFactKey.Value : 0,
                    PdPatientKey = null,
                    FdFaciltyKey = null,
                    RxNumber = null,
                    RefillNumber = null,
                    SoldDateKey = null
                };

                reportRows.Add(reportRow);
            }

            return reportRows;
        }

        /// <inheritdoc />
        public IEnumerable<WorkloadBalanceRow> MapWorkloadBalance(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<WorkloadBalanceRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new WorkloadBalanceRow
                {
                    FacilityId = row.NullableString(0, rowNumber),
                    RxNumber = row.NullableString(1, rowNumber),
                    Ndc = row.NullableString(2, rowNumber),
                    DrugName = row.NullableString(3, rowNumber),
                    DateIn = row.NullableField<DateTime>(4, rowNumber),
                    DateOut = row.NullableField<DateTime>(5, rowNumber),
                    WorkflowStep = row.NullableString(6, rowNumber),
                    DeliveryMethod = row.NullableString(7, rowNumber),
                    UserId = row.NullableString(8, rowNumber),
                    QtyDispensed = row.NullableField<decimal>(9, rowNumber),
                    OwningFacility = row.NullableString(10, rowNumber),
                    Wlb = row.NullableString(11, rowNumber),
                    SkipPreVerCode = row.NullableField<decimal>(12, rowNumber)
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<AstuteAdherenceDispenseRawDataRow> MapAstuteAdherenceDispenses(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<AstuteAdherenceDispenseRawDataRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new AstuteAdherenceDispenseRawDataRow
                {
                    RxNumber = row.NullableString(0, rowNumber) ?? string.Empty,
                    SoldDate = row.NullableField<DateTime>(1, rowNumber),
                    FacilityId = row.NullableString(2, rowNumber) ?? string.Empty,
                    PatientLastName = row.NullableString(3, rowNumber) ?? string.Empty,
                    PatientFirstName = row.NullableString(4, rowNumber) ?? string.Empty,
                    RefillNumber = row.NullableField<int>(5, rowNumber),
                    ProductName = row.NullableString(6, rowNumber) ?? string.Empty,
                    Quantity = row.NullableField<decimal>(7, rowNumber),
                    DaysSupply = row.NullableField<double>(8, rowNumber),
                    TotalRefillsRemaining = row.NullableField<decimal>(9, rowNumber),
                    DrugNDC = row.NullableString(10, rowNumber) ?? string.Empty,
                    CardholderOrPatientId = row.NullableString(11, rowNumber) ?? string.Empty,
                    PlanCode = row.NullableString(12, rowNumber) ?? string.Empty,
                    ShipAddress1 = row.NullableString(13, rowNumber) ?? string.Empty,
                    ShipAddress2 = row.NullableString(14, rowNumber) ?? string.Empty,
                    ShipCity = row.NullableString(15, rowNumber) ?? string.Empty,
                    ShipState = row.NullableString(16, rowNumber) ?? string.Empty,
                    ShipZipCode = row.NullableString(17, rowNumber) ?? string.Empty,
                    PatientDateOfBirth = row.NullableField<DateTime>(18, rowNumber),
                    SoldTime = row.NullableString(19, rowNumber) ?? string.Empty,
                    DateOfService = row.NullableField<DateTime>(20, rowNumber),
                    PersonCode = row.NullableString(21, rowNumber) ?? string.Empty,
                    RxFillSequence = row.NullableField<decimal>(22, rowNumber),
                    WrittenDate = row.NullableField<DateTime>(23, rowNumber),
                    PrescriberFirstName = row.NullableString(24, rowNumber) ?? string.Empty,
                    PrescriberLastName = row.NullableString(25, rowNumber) ?? string.Empty,
                    PrescriberNpi = row.NullableString(26, rowNumber) ?? string.Empty,
                    PrescriberDea = row.NullableString(27, rowNumber) ?? string.Empty,
                    PrescriberAddress1 = row.NullableString(28, rowNumber) ?? string.Empty,
                    PrescriberAddress2 = row.NullableString(29, rowNumber) ?? string.Empty,
                    PrescriberCity = row.NullableString(30, rowNumber) ?? string.Empty,
                    PrescriberState = row.NullableString(31, rowNumber) ?? string.Empty,
                    PrescriberZip = row.NullableString(32, rowNumber) ?? string.Empty,
                    PrescriberPhone = row.NullableString(33, rowNumber) ?? string.Empty,
                    PatientNum = row.NullableString(34, rowNumber) ?? string.Empty,
                    PatientPricePaid = row.NullableField<decimal>(35, rowNumber),
                    CourierName = row.NullableString(36, rowNumber) ?? string.Empty,
                    TrackingNumber = row.NullableString(37, rowNumber) ?? string.Empty,
                    TotalRefillsAllowed = row.NullableField<decimal>(38, rowNumber),
                    OrderNumber = row.NullableString(39, rowNumber) ?? string.Empty,
                    PatientGroupName = row.NullableString(40, rowNumber) ?? string.Empty,
                });

                rowNumber++;
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<AstuteAdherenceDispenseReportRow> PopulateAstuteAdherenceDispenseDerivedDetails(
            IEnumerable<AstuteAdherenceDispenseRawDataRow> rawDataRows,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows,
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows,
            out Dictionary<decimal, List<string>> constraintViolations)
        {
            List<AstuteAdherenceDispenseReportRow> reportRows = new List<AstuteAdherenceDispenseReportRow>();
            constraintViolations = new Dictionary<decimal, List<string>>();
            constraintViolations.Clear();

            foreach (var rawDataRow in rawDataRows)
            {

                DateTime rawDataSoldDate = rawDataRow.SoldDate ?? default;
                double rawDataDaysSupply = rawDataRow.DaysSupply ?? default;
                decimal rawDataTotalRefillsRemaining = rawDataRow.TotalRefillsRemaining ?? 0; // Per request from Melissa set to 0.
                string rawDataFacilityId = rawDataRow.FacilityId ?? string.Empty;
                string rawDataPlanCode = rawDataRow.PlanCode ?? string.Empty;
                string rawDataProductName = rawDataRow.ProductName ?? string.Empty;
                string rawDataPatientGroupName = rawDataRow.PatientGroupName ?? string.Empty;
                string rawDataDrugNdc = rawDataRow.DrugNDC ?? string.Empty;
                decimal rawDataPatientPricePaid = rawDataRow.PatientPricePaid ?? 0; // Per request from Melissa set to 0
                int rawDataRefillNumber = rawDataRow.RefillNumber ?? default;

                string missingValue = $"is missing a value";

                List<string> violations = new List<string>();
                if (rawDataSoldDate == default)
                    violations.Add($"SoldDate {missingValue}");
                if (rawDataDaysSupply == default)
                    violations.Add($"DaysSupply {missingValue}");
                if (string.IsNullOrEmpty(rawDataFacilityId))
                    violations.Add($"FacilityId {missingValue}");
                if (string.IsNullOrEmpty(rawDataPlanCode))
                    violations.Add($"PlanCode {missingValue}");
                if (string.IsNullOrEmpty(rawDataProductName))
                    violations.Add($"ProductName {missingValue}");
                if (string.IsNullOrEmpty(rawDataDrugNdc))
                    violations.Add($"DrugNdc {missingValue}");

                if (violations.Count > 0 && rawDataRow.RxFillSequence.HasValue)
                {
                    constraintViolations.Add(rawDataRow.RxFillSequence.Value, violations);
                    continue;
                }

                AstuteAdherenceDispenseReportRow reportRow = new AstuteAdherenceDispenseReportRow(rawDataRow);


                reportRow.StoreNumber = _astuteAdherenceDispenseHelper.DeriveStoreNumber(
                    rawDataFacilityId);

                reportRow.BaseProductName = _astuteAdherenceDispenseHelper.DeriveBaseProductName(
                    rawDataProductName);

                reportRow.ProgramType = _astuteAdherenceDispenseHelper.DeriveProgramType(
                    rawDataPlanCode,
                    reportRow.BaseProductName,
                    reportRow.StoreNumber.Value);

                reportRow.ProgramHeader = _astuteAdherenceDispenseHelper.DeriveProgramHeader(
                    reportRow.ProgramType,
                    rawDataDrugNdc,
                    reportRow.BaseProductName,
                    completeSpecialtyItemRows);

                reportRow.ProgramType = _astuteAdherenceDispenseHelper.DeriveProgramType(
                    rawDataPlanCode,
                    reportRow.BaseProductName,
                    reportRow.StoreNumber.Value);

                reportRow.BaseProductName = _astuteAdherenceDispenseHelper.DeriveBaseProductName(
                    rawDataProductName);

                reportRow.TitrationDoseFlag = _astuteAdherenceDispenseHelper.DeriveTitrationDoseFlag(
                    rawDataPatientGroupName,
                    rawDataDrugNdc,
                    reportRow.ProgramHeader,
                    rawDataDaysSupply);

                reportRow.CallDate = _astuteAdherenceDispenseHelper.DeriveCallDate(
                    rawDataSoldDate,
                    rawDataDaysSupply,
                    reportRow.ProgramType,
                    reportRow.BaseProductName,
                    reportRow.TitrationDoseFlag.Value,
                    rawDataTotalRefillsRemaining);

                reportRow.PatientIdType = _astuteAdherenceDispenseHelper.DerivePatientIdType(
                    reportRow.ProgramType);

                reportRow.FillDate = _astuteAdherenceDispenseHelper.DeriveFillDate(
                    reportRow.ProgramType,
                    reportRow.BaseProductName,
                    rawDataSoldDate,
                    rawDataDaysSupply);

                reportRow.FillNumber = _astuteAdherenceDispenseHelper.DeriveFillNumber(
                    rawDataRefillNumber);

                reportRow.SoldDateTime = _astuteAdherenceDispenseHelper.DeriveSoldDateTime(
                    rawDataSoldDate,
                    rawDataRow.SoldTime);

                reportRow.CrmProductName = _astuteAdherenceDispenseHelper.DeriveCrmProductName(
                    rawDataProductName,
                    reportRow.BaseProductName);

                reportRow.NextWorkflowStatus = _astuteAdherenceDispenseHelper.DeriveNextWorkflowStatus(
                    reportRow.ProgramType,
                    reportRow.TitrationDoseFlag.Value,
                    rawDataDaysSupply,
                    reportRow.BaseProductName,
                    rawDataTotalRefillsRemaining);

                bool shouldSkipDispense = _astuteAdherenceDispenseHelper.ShouldSkipDispense(
                    reportRow.ProgramType,
                    reportRow.StoreNumber.Value,
                    rawDataPatientPricePaid,
                    rawDataPlanCode,
                    rawDataDrugNdc,
                    specialtyDispenseExclusionRows);

                if (shouldSkipDispense)
                {
                    //TODO: Add to DEBUG file just for certification purposes.
                    continue;
                }

                reportRows.Add(reportRow);
            }

            return reportRows;
        }

        /// <inheritdoc />
        public IEnumerable<ImmunizationRow> MapImmunizations(DataSet ds)
        {
            if (ds.Tables.Count != 1)
            {
                throw new Exception("More than 1 table passed in data set");
            }

            DataTable queryTable = ds.Tables[0];
            ICollection<ImmunizationRow> result = [];

            _logger.LogDebug($"Parsing {queryTable.Rows.Count} rows");

            int rowNumber = 1;
            foreach (DataRow row in queryTable.Rows)
            {
                result.Add(new ImmunizationRow
                {
                    StrFirstName = row.NullableString(0, rowNumber),
                    StrMiddleName = row.NullableString(1, rowNumber),
                    StrLastName = row.NullableString(2, rowNumber),
                    DtBirthDate = row.NullableString(3, rowNumber),
                    CGender = row.NullableString(4, rowNumber),
                    StrAddressOne = row.NullableString(5, rowNumber),
                    StrAddressTwo = row.NullableString(6, rowNumber),
                    StrCity = row.NullableString(7, rowNumber),
                    StrState = row.NullableString(8, rowNumber),
                    StrZip = row.NullableString(9, rowNumber),
                    StrProductNumber = row.NullableString(10, rowNumber),
                    StrProductNdc = row.NullableString(11, rowNumber),
                    DecDispensedQty = row.NullableString(12, rowNumber),
                    CGenericIndentifier = row.NullableString(13, rowNumber),
                    StrVerifiedRphFirst = row.NullableString(14, rowNumber),
                    StrVerifiedRphLast = row.NullableString(15, rowNumber),
                    StrFacilityId = row.NullableString(16, rowNumber),
                    StrRxNumber = row.NullableString(17, rowNumber),
                    StrDosageForm = row.NullableString(18, rowNumber),
                    StrStrength = row.NullableString(19, rowNumber),
                    DtSoldDate = row.NullableString(20, rowNumber),
                    DtCanceledDate = row.NullableString(21, rowNumber),
                    CActionCode = row.NullableString(22, rowNumber),
                    StrDea = row.NullableString(23, rowNumber),
                    StrNpi = row.NullableString(24, rowNumber),
                    DecInternalPtNum = row.NullableField<int>(25, rowNumber),
                    LotNumber = row.NullableString(26, rowNumber),
                    ExpDate = row.NullableString(27, rowNumber),
                    StrPatientMail = row.NullableString(28, rowNumber),
                    VisPresentedDate = row.NullableString(29, rowNumber),
                    AdministrationSite = row.NullableString(30, rowNumber),
                    ProtectionIndicator = row.NullableString(31, rowNumber),
                    RecipRaceOne = row.NullableString(32, rowNumber),
                    RecipEthnicity = row.NullableString(33, rowNumber),
                    VfcStatus = row.NullableString(34, rowNumber),
                    NyPriorityGroup = row.NullableString(35, rowNumber),
                    PhoneNumber = row.NullableString(36, rowNumber),
                    //Not consuming index 37 as it is not used in the report
                    RefillNumber = row.NullableField<int>(38, rowNumber),
                    PrescribedById = row.NullableString(39, rowNumber),
                    GenderIdentity = row.NullableString(40, rowNumber),
                    PrimaryLanguage = row.NullableString(41, rowNumber),
                    StrVerifiedRphTitle = row.NullableString(42, rowNumber)

                });

                rowNumber++;
            }

            return result;
        }
    }
}
