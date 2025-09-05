using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.DataSetMapper;
using Library.McKessonDWInterface.Helper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System.Data;

namespace ZZZTest.Library.McKessonDWInterface.DataSetMapper
{
    public class DataSetMapperTests
    {
        private readonly ILogger<DataSetMapperImp> _loggerMock = Substitute.For<ILogger<DataSetMapperImp>>();
        private readonly ITurnaroundTimeHelper _turnaroundTimeHelperMock = Substitute.For<ITurnaroundTimeHelper>();
        private readonly ISureScriptsHelper _sureScriptsHelperMock = Substitute.For<ISureScriptsHelper>();
        private readonly IAstuteAdherenceDispenseHelper _astuteAdherenceDispenseHelperMock = Substitute.For<IAstuteAdherenceDispenseHelper>();

        private readonly DataSetMapperImp _sut;

        public DataSetMapperTests()
        {
            _sut = new DataSetMapperImp(_loggerMock,
                                        _turnaroundTimeHelperMock,
                                        _sureScriptsHelperMock,
                                        _astuteAdherenceDispenseHelperMock
                                        );
        }

        [Fact]
        public void MapOmnisysClaim_MapsFields_ToReturnedList()
        {
            // Arrange
            string pharmacyNpi = "5938768038";
            string perscriptionNumber = "000008000769";
            string refillNumber = "000";
            DateOnly dateOfService = new DateOnly(2024, 5, 2);
            DateOnly dateOfSale = new DateOnly(2018, 5, 1);
            string ndcNumber = "00957392085";
            string cardholderId = "8693278593     ";
            string authorizationNumber = "A1P6250C215631          ";

            DataSet input = JsonConvert.DeserializeObject<DataSet>(
                string.Format("{{\"Table\": [{{\"PHARMACY_NPI\": \"{0}\", \"PRESCRIPTION_NBR\":\"{1}\", \"REFILL_NBR\":\"{2}\",\"DATE_OF_SERVICE\":\"{3}\", \"SOLD_DATE\":\"{4}\", \"NDC_NBR\":\"{5}\", \"CARDHOLDER_ID\": \"{6}\",\"AUTHORIZATION_NBR\": \"{7}\",\"RESERVED_FOR_FUTURE_USE\":\"           \"}}]}}",
                pharmacyNpi,
                perscriptionNumber,
                refillNumber,
                dateOfService.ToString("yyyyMMdd"),
                dateOfSale.ToString("yyyyMMdd"),
                ndcNumber,
                cardholderId,
                authorizationNumber)) ?? throw new Exception("Error serializing");

            // Act
            IEnumerable<OmnisysClaimRow> result = _sut.MapOmnisysClaim(input);

            // Assert
            OmnisysClaimRow singleResult = Assert.Single(result);
            Assert.Equal(pharmacyNpi, singleResult.PharmacyNpi);
            Assert.Equal(perscriptionNumber, singleResult.PrescriptionNbr);
            Assert.Equal(refillNumber, singleResult.RefillNumber);
            Assert.Equal(dateOfService, singleResult.DateOfService);
            Assert.Equal(dateOfSale, singleResult.SoldDate);
            Assert.Equal(ndcNumber, singleResult.NdcNumber);
            Assert.Equal(cardholderId, singleResult.CardholderId);
            Assert.Equal(authorizationNumber, singleResult.AuthorizationNumber);
        }

        [Fact]
        public void MapNewTagPatientGroups_MapsFields_ToReturnedList()
        {
            // Arrange
            DateTime? patientAddDate = new DateTime(2024, 4, 2);
            int? storeNum = 67;
            long? patientNum = 1;
            long? groupNum = 2;
            string? groupName = "Test Group";
            string? groupDescription = "Test Group Description";
            string? employeeUserName = "Test User Name";
            string? employeeFirstName = "Test First";
            string? employeeLastName = "Test Last";
            string? eventDescription = "Test Event";
            DateTime? groupStartDate = new DateTime(2024, 4, 1);

            DataSet input = JsonConvert.DeserializeObject<DataSet>(
                string.Format("{{\"Table\": [{{\"pt_add_date\": \"{0}\", \"store_num\":\"{1}\", \"patient_num\":\"{2}\", \"group_num\":\"{3}\", \"group_name\":\"{4}\", \"group_desc\":\"{5}\", \"emp_username\":\"{6}\", \"emp_fname\":\"{7}\", \"emp_lname\":\"{8}\", \"event_description\":\"{9}\", \"group_start_date\":\"{10}\"}}]}}",
                patientAddDate?.ToString("yyyyMMdd HH:mm:ss"),
                storeNum,
                patientNum,
                groupNum,
                groupName,
                groupDescription,
                employeeUserName,
                employeeFirstName,
                employeeLastName,
                eventDescription,
                groupStartDate?.ToString("yyyyMMdd HH:mm:ss"))
                ) ?? throw new Exception("Error serializing");

            // Act
            IEnumerable<NewTagPatientGroupsRow> result = _sut.MapNewTagPatientGroups(input);

            // Assert
            NewTagPatientGroupsRow singleResult = Assert.Single(result);
            Assert.Equal(patientAddDate, singleResult.PatientAddDate);
            Assert.Equal(storeNum, singleResult.StoreNum);
            Assert.Equal(patientNum, singleResult.PatientNum);
            Assert.Equal(groupNum, singleResult.GroupNum);
            Assert.Equal(groupName, singleResult.GroupName);
            Assert.Equal(groupDescription, singleResult.GroupDescription);
            Assert.Equal(employeeUserName, singleResult.EmployeeUserName);
            Assert.Equal(employeeFirstName, singleResult.EmployeeFirstName);
            Assert.Equal(employeeLastName, singleResult.EmployeeLastName);
            Assert.Equal(eventDescription, singleResult.EventDescription);
            Assert.Equal(groupStartDate, singleResult.GroupStartDate);
        }

        [Fact]
        public void MapSoldDetail_MapsFields_ToReturnedList()
        {
            // Arrange
            SoldDetailRow testRow = new SoldDetailRow
            {
                StoreNbr = 1,
                RxNbr = 12345678,
                RefillNbr = 2,
                PartialFillSequenceNbr = 3,
                SoldDate = new DateTime(2024, 4, 5, 16, 42, 59),
                OrderNbr = "A12345",
                QtyDispensed = 4,
                NdcWithoutDashes = "00001000203",
                AcquisitionCost = 1.11M,
                ThirdPartyPay = 2.22M,
                PatientPay = 3.33M,
                TransactionPrice = 4.44M,
            };

            DataSet input = JsonConvert.DeserializeObject<DataSet>(
                string.Format(@"{{""Table"": [{{
                    ""StoreNbr"": ""{0}"",
                    ""RxNbr"":""{1}"",
                    ""RefillNbr"":""{2}"",
                    ""PartialFillSequenceNbr"":""{3}"",
                    ""SoldDate"":""{4}"",
                    ""OrderNbr"":""{5}"",
                    ""QtyDispensed"":""{6}"",
                    ""NdcWithoutDashes"":""{7}"",
                    ""AcquisitionCost"":""{8}"",
                    ""ThirdPartyPay"":""{9}"",
                    ""PatientPay"":""{10}"",
                    ""TransactionPrice"":""{11}""
                    }}]}}",
                    testRow.StoreNbr,
                    testRow.RxNbr,
                    testRow.RefillNbr,
                    testRow.PartialFillSequenceNbr,
                    testRow.SoldDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.OrderNbr,
                    testRow.QtyDispensed,
                    testRow.NdcWithoutDashes,
                    testRow.AcquisitionCost,
                    testRow.ThirdPartyPay,
                    testRow.PatientPay,
                    testRow.TransactionPrice)) ?? throw new Exception("Error serializing");

            // Act
            IEnumerable<SoldDetailRow> result = _sut.MapSoldDetail(input);

            // Assert
            SoldDetailRow singleResult = Assert.Single(result);
            Assert.Equal(testRow.StoreNbr, singleResult.StoreNbr);
            Assert.Equal(testRow.RxNbr, singleResult.RxNbr);
            Assert.Equal(testRow.RefillNbr, singleResult.RefillNbr);
            Assert.Equal(testRow.PartialFillSequenceNbr, singleResult.PartialFillSequenceNbr);
            Assert.Equal(testRow.SoldDate, singleResult.SoldDate);
            Assert.Equal(testRow.OrderNbr, singleResult.OrderNbr);
            Assert.Equal(testRow.QtyDispensed, singleResult.QtyDispensed);
            Assert.Equal(testRow.NdcWithoutDashes, singleResult.NdcWithoutDashes);
            Assert.Equal(testRow.AcquisitionCost, singleResult.AcquisitionCost);
            Assert.Equal(testRow.ThirdPartyPay, singleResult.ThirdPartyPay);
            Assert.Equal(testRow.PatientPay, singleResult.PatientPay);
            Assert.Equal(testRow.TransactionPrice, singleResult.TransactionPrice);
        }

        [Fact]
        public void MapRxErp_MapsFields_ToReturnedList()
        {
            // Arrange
            RxErpRow testRow = new RxErpRow
            {
                RxErpChangeDate = new DateTime(2001, 4, 5, 16, 42, 59),
                PatientNbr = 1.11M,
                StoreNbr = 22,
                RxNbr = 32,
                RxNbrTxt = "a1",
                NdcWithoutDashes = "a2",
                DrugName = "a3",
                DaysSupply = 41.11M,
                RefillsAuthorized = 52,
                LastFillDate = new DateTime(2002, 4, 5, 16, 42, 59),
                ErpOrigTargetDate = new DateTime(2003, 4, 5, 16, 42, 59),
                ErpTargetDate = new DateTime(2004, 4, 5, 16, 42, 59),
                ErpEnrollStatus = "a4",
                ErpEnrollStatusDateTime = new DateTime(2005, 4, 5, 16, 42, 59),
                ErpEnrollReason = "a5",
                ErpExcludeReason = "a6",
                ErpDeliveryMethod = "a7",
                ErpDeliveryMethodPersist = "a8",
                IsConsentToFill = "a9",
                UserConsentToFill = "a10",
                DateConsentToFill = new DateTime(2006, 4, 5, 16, 42, 59),
                RxRecordEffectiveStartDateTime = new DateTime(2007, 4, 5, 16, 42, 59),
                RecordType = "a11",
                ErpUsermodifiedTargetDate = new DateTime(2008, 4, 5, 16, 42, 59),
                ErpModifiedUserType = "a12",
                ErpModifiedUser = "a13",
                ErpUserModifiedRecordType = "a14",
                ErpUserModifiedOnDate = new DateTime(2009, 4, 5, 16, 42, 59),
                RecordUpdates = 62,
                PatientAutofillEnrollStatus = "a15",
                PatientErpEnrollStatus = "a16",
                PatientErpEnrollDate = new DateTime(2010, 4, 5, 16, 42, 59),
                PatientAutoEnrollFutureRxs = "a17",
                PatientPreferredDeliveryMethod = "a18",
                PatientPrefersMailDelivery = "a19",
                PatientErpPaymentMethod = "a20",
                PatientRequestConsentToFill = "a21",
                PatientRequestConsentToErp = "a22",
                PatientExternalId = "a23",
                ProhibitRenewalRequest = "a24",
                CsrDiagnosisCode = "a25",
                DrugForm = "a26",
                QtyAuthorized = 71.11M,
                RefillQty = 82,
                ExpireDate = new DateTime(2011, 4, 5, 16, 42, 59),
                ReassignedRxNum = "a27",
                RxCreatedDate = new DateTime(2012, 4, 5, 16, 42, 59),
                RxCreatedUser = "a28",
                RxOriginCode = "a29",
                RxStatus = "a30",
                RxRecordNum = 91.11M,
                FirstFillQty = 101.11M,
                DatePrescribed = new DateTime(2013, 4, 5, 16, 42, 59),
                OriginalNdcWithoutDashes = "a31",
                OriginalDrugName = "a32",
                OriginalQty = 111.11M,
                Serial = "a33",
                DawCode = "a34",
                SelectedUpc = "a35",
                AckCode = "a36",
                LetterOnFile = "a37",
                EmployeeLoginName = "a38",
                LastAckChangeDate = new DateTime(2014, 4, 5, 16, 42, 59),
                LastAckChangeUser = "a39",
                LastAckChangeUserName = "a40",
                OriginalFillDate = new DateTime(2015, 4, 5, 16, 42, 59),
                TotalDailyDose = 121.11M,
                TotalQtyDispensed = 131.11M,
                TotalQtyTransferred = 141.11M,
                LegalFills = 151.11M,
                LatestFill = 161.11M,
                LatestFillDuration = 171.11M,
                LatestFillNonInprocess = 181.11M,
                LatestFillCanceled = 191.11M,
                LastDispFill = 201.11M,
                LastFillRefill = 211.11M,
                LastFillReleased = 221.11M,
                LastSoldFill = 231.11M,
                ProfileIncludeDate = new DateTime(2016, 4, 5, 16, 42, 59),
                FirstFill = 241.11M,
                FirstRefill = 251.11M,
                EarliestFillDate = new DateTime(2017, 4, 5, 16, 42, 59),
                PrevFillReleased = 261.11M,
                SyncDate = new DateTime(2018, 4, 5, 16, 42, 59),
                IsLinked = "a41",
                CancelReason = "a42",
                PrescriberNum = 271.11M,
                PrescriberAddressNum = 281.11M,
                PrescriberOrderNbr = "a43",
                IsEPCS = "a44",
                RrrDenied = "a45",
            };

            DataSet input = JsonConvert.DeserializeObject<DataSet>(
                string.Format(@"{{""Table"": [{{
                    ""RxErpChangeDate"": ""{0}"",
                    ""PatientNbr"":""{1}"",
                    ""StoreNbr"":""{2}"",
                    ""RxNbr"":""{3}"",
                    ""RxNbrTxt"":""{4}"",
                    ""NdcWithoutDashes"":""{5}"",
                    ""DrugName"":""{6}"",
                    ""DaysSupply"":""{7}"",
                    ""RefillsAuthorized"":""{8}"",
                    ""LastFillDate"":""{9}"",
                    ""ErpOrigTargetDate"": ""{10}"",
                    ""ErpTargetDate"":""{11}"",
                    ""ErpEnrollStatus"":""{12}"",
                    ""ErpEnrollStatusDateTime"":""{13}"",
                    ""ErpEnrollReason"":""{14}"",
                    ""ErpExcludeReason"":""{15}"",
                    ""ErpDeliveryMethod"":""{16}"",
                    ""ErpDeliveryMethodPersist"":""{17}"",
                    ""IsConsentToFill"":""{18}"",
                    ""UserConsentToFill"":""{19}"",
                    ""DateConsentToFill"": ""{20}"",
                    ""RxRecordEffectiveStartDateTime"":""{21}"",
                    ""RecordType"":""{22}"",
                    ""ErpUsermodifiedTargetDate"":""{23}"",
                    ""ErpModifiedUserType"":""{24}"",
                    ""ErpModifiedUser"":""{25}"",
                    ""ErpUserModifiedRecordType"":""{26}"",
                    ""ErpUserModifiedOnDate"":""{27}"",
                    ""RecordUpdates"":""{28}"",
                    ""PatientAutofillEnrollStatus"":""{29}"",
                    ""PatientErpEnrollStatus"": ""{30}"",
                    ""PatientErpEnrollDate"":""{31}"",
                    ""PatientAutoEnrollFutureRxs"":""{32}"",
                    ""PatientPreferredDeliveryMethod"":""{33}"",
                    ""PatientPrefersMailDelivery"":""{34}"",
                    ""PatientErpPaymentMethod"":""{35}"",
                    ""PatientRequestConsentToFill"":""{36}"",
                    ""PatientRequestConsentToErp"":""{37}"",
                    ""PatientExternalId"":""{38}"",
                    ""ProhibitRenewalRequest"":""{39}"",
                    ""CsrDiagnosisCode"": ""{40}"",
                    ""DrugForm"":""{41}"",
                    ""QtyAuthorized"":""{42}"",
                    ""RefillQty"":""{43}"",
                    ""ExpireDate"":""{44}"",
                    ""ReassignedRxNum"":""{45}"",
                    ""RxCreatedDate"":""{46}"",
                    ""RxCreatedUser"":""{47}"",
                    ""RxOriginCode"":""{48}"",
                    ""RxStatus"":""{49}"",
                    ""RxRecordNum"": ""{50}"",
                    ""FirstFillQty"":""{51}"",
                    ""DatePrescribed"":""{52}"",
                    ""OriginalNdcWithoutDashes"":""{53}"",
                    ""OriginalDrugName"":""{54}"",
                    ""OriginalQty"":""{55}"",
                    ""Serial"":""{56}"",
                    ""DawCode"":""{57}"",
                    ""SelectedUpc"":""{58}"",
                    ""AckCode"":""{59}"",
                    ""LetterOnFile"": ""{60}"",
                    ""EmployeeLoginName"":""{61}"",
                    ""LastAckChangeDate"":""{62}"",
                    ""LastAckChangeUser"":""{63}"",
                    ""LastAckChangeUserName"":""{64}"",
                    ""OriginalFillDate"":""{65}"",
                    ""TotalDailyDose"":""{66}"",
                    ""TotalQtyDispensed"":""{67}"",
                    ""TotalQtyTransferred"":""{68}"",
                    ""LegalFills"":""{69}"",
                    ""LatestFill"": ""{70}"",
                    ""LatestFillDuration"":""{71}"",
                    ""LatestFillNonInprocess"":""{72}"",
                    ""LatestFillCanceled"":""{73}"",
                    ""LastDispFill"":""{74}"",
                    ""LastFillRefill"":""{75}"",
                    ""LastFillReleased"":""{76}"",
                    ""LastSoldFill"":""{77}"",
                    ""ProfileIncludeDate"":""{78}"",
                    ""FirstFill"":""{79}"",
                    ""FirstRefill"": ""{80}"",
                    ""EarliestFillDate"":""{81}"",
                    ""PrevFillReleased"":""{82}"",
                    ""SyncDate"":""{83}"",
                    ""IsLinked"":""{84}"",
                    ""CancelReason"":""{85}"",
                    ""PrescriberNum"":""{86}"",
                    ""PrescriberAddressNum"":""{87}"",
                    ""PrescriberOrderNbr"":""{88}"",
                    ""IsEPCS"":""{89}"",
                    ""RrrDenied"":""{90}""
                    }}]}}",
                    testRow.RxErpChangeDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.PatientNbr,
                    testRow.StoreNbr,
                    testRow.RxNbr,
                    testRow.RxNbrTxt,
                    testRow.NdcWithoutDashes,
                    testRow.DrugName,
                    testRow.DaysSupply,
                    testRow.RefillsAuthorized,
                    testRow.LastFillDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.ErpOrigTargetDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.ErpTargetDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.ErpEnrollStatus,
                    testRow.ErpEnrollStatusDateTime?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.ErpEnrollReason,
                    testRow.ErpExcludeReason,
                    testRow.ErpDeliveryMethod,
                    testRow.ErpDeliveryMethodPersist,
                    testRow.IsConsentToFill,
                    testRow.UserConsentToFill,
                    testRow.DateConsentToFill?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.RxRecordEffectiveStartDateTime?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.RecordType,
                    testRow.ErpUsermodifiedTargetDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.ErpModifiedUserType,
                    testRow.ErpModifiedUser,
                    testRow.ErpUserModifiedRecordType,
                    testRow.ErpUserModifiedOnDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.RecordUpdates,
                    testRow.PatientAutofillEnrollStatus,
                    testRow.PatientErpEnrollStatus,
                    testRow.PatientErpEnrollDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.PatientAutoEnrollFutureRxs,
                    testRow.PatientPreferredDeliveryMethod,
                    testRow.PatientPrefersMailDelivery,
                    testRow.PatientErpPaymentMethod,
                    testRow.PatientRequestConsentToFill,
                    testRow.PatientRequestConsentToErp,
                    testRow.PatientExternalId,
                    testRow.ProhibitRenewalRequest,
                    testRow.CsrDiagnosisCode,
                    testRow.DrugForm,
                    testRow.QtyAuthorized,
                    testRow.RefillQty,
                    testRow.ExpireDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.ReassignedRxNum,
                    testRow.RxCreatedDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.RxCreatedUser,
                    testRow.RxOriginCode,
                    testRow.RxStatus,
                    testRow.RxRecordNum,
                    testRow.FirstFillQty,
                    testRow.DatePrescribed?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.OriginalNdcWithoutDashes,
                    testRow.OriginalDrugName,
                    testRow.OriginalQty,
                    testRow.Serial,
                    testRow.DawCode,
                    testRow.SelectedUpc,
                    testRow.AckCode,
                    testRow.LetterOnFile,
                    testRow.EmployeeLoginName,
                    testRow.LastAckChangeDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.LastAckChangeUser,
                    testRow.LastAckChangeUserName,
                    testRow.OriginalFillDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.TotalDailyDose,
                    testRow.TotalQtyDispensed,
                    testRow.TotalQtyTransferred,
                    testRow.LegalFills,
                    testRow.LatestFill,
                    testRow.LatestFillDuration,
                    testRow.LatestFillNonInprocess,
                    testRow.LatestFillCanceled,
                    testRow.LastDispFill,
                    testRow.LastFillRefill,
                    testRow.LastFillReleased,
                    testRow.LastSoldFill,
                    testRow.ProfileIncludeDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.FirstFill,
                    testRow.FirstRefill,
                    testRow.EarliestFillDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.PrevFillReleased,
                    testRow.SyncDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.IsLinked,
                    testRow.CancelReason,
                    testRow.PrescriberNum,
                    testRow.PrescriberAddressNum,
                    testRow.PrescriberOrderNbr,
                    testRow.IsEPCS,
                    testRow.RrrDenied)) ?? throw new Exception("Error serializing");

            // Act
            IEnumerable<RxErpRow> result = _sut.MapRxErp(input);

            // Assert
            RxErpRow singleResult = Assert.Single(result);
            Assert.Equal(testRow.RxErpChangeDate, singleResult.RxErpChangeDate);
            Assert.Equal(testRow.PatientNbr, singleResult.PatientNbr);
            Assert.Equal(testRow.StoreNbr, singleResult.StoreNbr);
            Assert.Equal(testRow.RxNbr, singleResult.RxNbr);
            Assert.Equal(testRow.RxNbrTxt, singleResult.RxNbrTxt);
            Assert.Equal(testRow.NdcWithoutDashes, singleResult.NdcWithoutDashes);
            Assert.Equal(testRow.DrugName, singleResult.DrugName);
            Assert.Equal(testRow.DaysSupply, singleResult.DaysSupply);
            Assert.Equal(testRow.RefillsAuthorized, singleResult.RefillsAuthorized);
            Assert.Equal(testRow.LastFillDate, singleResult.LastFillDate);
            Assert.Equal(testRow.ErpOrigTargetDate, singleResult.ErpOrigTargetDate);
            Assert.Equal(testRow.ErpTargetDate, singleResult.ErpTargetDate);
            Assert.Equal(testRow.ErpEnrollStatus, singleResult.ErpEnrollStatus);
            Assert.Equal(testRow.ErpEnrollStatusDateTime, singleResult.ErpEnrollStatusDateTime);
            Assert.Equal(testRow.ErpEnrollReason, singleResult.ErpEnrollReason);
            Assert.Equal(testRow.ErpExcludeReason, singleResult.ErpExcludeReason);
            Assert.Equal(testRow.ErpDeliveryMethod, singleResult.ErpDeliveryMethod);
            Assert.Equal(testRow.ErpDeliveryMethodPersist, singleResult.ErpDeliveryMethodPersist);
            Assert.Equal(testRow.IsConsentToFill, singleResult.IsConsentToFill);
            Assert.Equal(testRow.UserConsentToFill, singleResult.UserConsentToFill);
            Assert.Equal(testRow.DateConsentToFill, singleResult.DateConsentToFill);
            Assert.Equal(testRow.RxRecordEffectiveStartDateTime, singleResult.RxRecordEffectiveStartDateTime);
            Assert.Equal(testRow.RecordType, singleResult.RecordType);
            Assert.Equal(testRow.ErpUsermodifiedTargetDate, singleResult.ErpUsermodifiedTargetDate);
            Assert.Equal(testRow.ErpModifiedUserType, singleResult.ErpModifiedUserType);
            Assert.Equal(testRow.ErpModifiedUser, singleResult.ErpModifiedUser);
            Assert.Equal(testRow.ErpUserModifiedRecordType, singleResult.ErpUserModifiedRecordType);
            Assert.Equal(testRow.ErpUserModifiedOnDate, singleResult.ErpUserModifiedOnDate);
            Assert.Equal(testRow.RecordUpdates, singleResult.RecordUpdates);
            Assert.Equal(testRow.PatientAutofillEnrollStatus, singleResult.PatientAutofillEnrollStatus);
            Assert.Equal(testRow.PatientErpEnrollStatus, singleResult.PatientErpEnrollStatus);
            Assert.Equal(testRow.PatientErpEnrollDate, singleResult.PatientErpEnrollDate);
            Assert.Equal(testRow.PatientAutoEnrollFutureRxs, singleResult.PatientAutoEnrollFutureRxs);
            Assert.Equal(testRow.PatientPreferredDeliveryMethod, singleResult.PatientPreferredDeliveryMethod);
            Assert.Equal(testRow.PatientPrefersMailDelivery, singleResult.PatientPrefersMailDelivery);
            Assert.Equal(testRow.PatientErpPaymentMethod, singleResult.PatientErpPaymentMethod);
            Assert.Equal(testRow.PatientRequestConsentToFill, singleResult.PatientRequestConsentToFill);
            Assert.Equal(testRow.PatientRequestConsentToErp, singleResult.PatientRequestConsentToErp);
            Assert.Equal(testRow.PatientExternalId, singleResult.PatientExternalId);
            Assert.Equal(testRow.ProhibitRenewalRequest, singleResult.ProhibitRenewalRequest);
            Assert.Equal(testRow.CsrDiagnosisCode, singleResult.CsrDiagnosisCode);
            Assert.Equal(testRow.DrugForm, singleResult.DrugForm);
            Assert.Equal(testRow.QtyAuthorized, singleResult.QtyAuthorized);
            Assert.Equal(testRow.RefillQty, singleResult.RefillQty);
            Assert.Equal(testRow.ExpireDate, singleResult.ExpireDate);
            Assert.Equal(testRow.ReassignedRxNum, singleResult.ReassignedRxNum);
            Assert.Equal(testRow.RxCreatedDate, singleResult.RxCreatedDate);
            Assert.Equal(testRow.RxCreatedUser, singleResult.RxCreatedUser);
            Assert.Equal(testRow.RxOriginCode, singleResult.RxOriginCode);
            Assert.Equal(testRow.RxStatus, singleResult.RxStatus);
            Assert.Equal(testRow.RxRecordNum, singleResult.RxRecordNum);
            Assert.Equal(testRow.FirstFillQty, singleResult.FirstFillQty);
            Assert.Equal(testRow.DatePrescribed, singleResult.DatePrescribed);
            Assert.Equal(testRow.OriginalNdcWithoutDashes, singleResult.OriginalNdcWithoutDashes);
            Assert.Equal(testRow.OriginalDrugName, singleResult.OriginalDrugName);
            Assert.Equal(testRow.OriginalQty, singleResult.OriginalQty);
            Assert.Equal(testRow.Serial, singleResult.Serial);
            Assert.Equal(testRow.DawCode, singleResult.DawCode);
            Assert.Equal(testRow.SelectedUpc, singleResult.SelectedUpc);
            Assert.Equal(testRow.AckCode, singleResult.AckCode);
            Assert.Equal(testRow.LetterOnFile, singleResult.LetterOnFile);
            Assert.Equal(testRow.EmployeeLoginName, singleResult.EmployeeLoginName);
            Assert.Equal(testRow.LastAckChangeDate, singleResult.LastAckChangeDate);
            Assert.Equal(testRow.LastAckChangeUser, singleResult.LastAckChangeUser);
            Assert.Equal(testRow.LastAckChangeUserName, singleResult.LastAckChangeUserName);
            Assert.Equal(testRow.OriginalFillDate, singleResult.OriginalFillDate);
            Assert.Equal(testRow.TotalDailyDose, singleResult.TotalDailyDose);
            Assert.Equal(testRow.TotalQtyDispensed, singleResult.TotalQtyDispensed);
            Assert.Equal(testRow.TotalQtyTransferred, singleResult.TotalQtyTransferred);
            Assert.Equal(testRow.LegalFills, singleResult.LegalFills);
            Assert.Equal(testRow.LatestFill, singleResult.LatestFill);
            Assert.Equal(testRow.LatestFillDuration, singleResult.LatestFillDuration);
            Assert.Equal(testRow.LatestFillNonInprocess, singleResult.LatestFillNonInprocess);
            Assert.Equal(testRow.LatestFillCanceled, singleResult.LatestFillCanceled);
            Assert.Equal(testRow.LastDispFill, singleResult.LastDispFill);
            Assert.Equal(testRow.LastFillRefill, singleResult.LastFillRefill);
            Assert.Equal(testRow.LastFillReleased, singleResult.LastFillReleased);
            Assert.Equal(testRow.LastSoldFill, singleResult.LastSoldFill);
            Assert.Equal(testRow.ProfileIncludeDate, singleResult.ProfileIncludeDate);
            Assert.Equal(testRow.FirstFill, singleResult.FirstFill);
            Assert.Equal(testRow.FirstRefill, singleResult.FirstRefill);
            Assert.Equal(testRow.EarliestFillDate, singleResult.EarliestFillDate);
            Assert.Equal(testRow.PrevFillReleased, singleResult.PrevFillReleased);
            Assert.Equal(testRow.SyncDate, singleResult.SyncDate);
            Assert.Equal(testRow.IsLinked, singleResult.IsLinked);
            Assert.Equal(testRow.CancelReason, singleResult.CancelReason);
            Assert.Equal(testRow.PrescriberNum, singleResult.PrescriberNum);
            Assert.Equal(testRow.PrescriberAddressNum, singleResult.PrescriberAddressNum);
            Assert.Equal(testRow.PrescriberOrderNbr, singleResult.PrescriberOrderNbr);
            Assert.Equal(testRow.IsEPCS, singleResult.IsEPCS);
            Assert.Equal(testRow.RrrDenied, singleResult.RrrDenied);
        }

        [Fact]
        public void MapStoreInventoryHistory_MapsFields_ToReturnedList()
        {
            // Arrange
            StoreInventoryHistoryRow testRow = new StoreInventoryHistoryRow
            {
                DateOfService = new DateTime(2024, 4, 5, 0, 0, 0),
                StoreNbr = 67,
                NdcWithoutDashes = "00001000203",
                DrugName = "Gummy Bears",
                Sdgi = "1",
                Gcn = "2",
                GcnSequence = 0M,
                OrangeBookCode = "007",
                FormCode = "006",
                PackSize = 1.1M,
                TruePack = 2.2M,
                PM = "pm",
                IsPreferred = "Y",
                LastAcquisitionCostPack = 3.3M,
                LastAcquisitionCostUnit = 4.4M,
                LastAcquisitionCostDate = new DateTime(2023, 2, 1, 10, 11, 12),
                OnHandQty = 5,
                OnHandValue = 6,
                ComittedQty = 7,
                ComittedValue = 8,
                TotalQTY = 9,
                TotalValue = 10.10M,
                AcquisitionCostPack = 11.11M,
                AcquisitionCostUnit = 12.12M,
                PrimarySupplier = "Supplier",
                LastChangeDate = new DateTime(2024, 4, 5, 16, 42, 59),
            };

            DataSet input = JsonConvert.DeserializeObject<DataSet>(
                string.Format(@"{{""Table"": [{{
                    ""DateOfService"": ""{0}"",
                    ""StoreNbr"":""{1}"",
                    ""NdcWithoutDashes"":""{2}"",
                    ""DrugName"":""{3}"",
                    ""Sdgi"":""{4}"",
                    ""Gcn"":""{5}"",
                    ""GcnSequence"":""{6}"",
                    ""OrangeBookCode"":""{7}"",
                    ""FormCode"":""{8}"",
                    ""PackSize"":""{9}"",
                    ""TruePack"":""{10}"",
                    ""PM"":""{11}"",
                    ""IsPreferred"":""{12}"",
                    ""LastAcquisitionCostPack"":""{13}"",
                    ""LastAcquisitionCostUnit"":""{14}"",
                    ""LastAcquisitionCostDate"":""{15}"",
                    ""OnHandQty"":""{16}"",
                    ""OnHandValue"":""{17}"",
                    ""ComittedQty"":""{18}"",
                    ""ComittedValue"":""{19}"",
                    ""TotalQTY"":""{20}"",
                    ""TotalValue"":""{21}"",
                    ""AcquisitionCostPack"":""{22}"",
                    ""AcquisitionCostUnit"":""{23}"",
                    ""PrimarySupplier"":""{24}"",
                    ""LastChangeDate"":""{25}""
                    }}]}}",
                    testRow.DateOfService?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.StoreNbr,
                    testRow.NdcWithoutDashes,
                    testRow.DrugName,
                    testRow.Sdgi,
                    testRow.Gcn,
                    testRow.GcnSequence,
                    testRow.OrangeBookCode,
                    testRow.FormCode,
                    testRow.PackSize,
                    testRow.TruePack,
                    testRow.PM,
                    testRow.IsPreferred,
                    testRow.LastAcquisitionCostPack,
                    testRow.LastAcquisitionCostUnit,
                    testRow.LastAcquisitionCostDate?.ToString("yyyyMMdd HH:mm:ss"),
                    testRow.OnHandQty,
                    testRow.OnHandValue,
                    testRow.ComittedQty,
                    testRow.ComittedValue,
                    testRow.TotalQTY,
                    testRow.TotalValue,
                    testRow.AcquisitionCostPack,
                    testRow.AcquisitionCostUnit,
                    testRow.PrimarySupplier,
                    testRow.LastChangeDate?.ToString("yyyyMMdd HH:mm:ss"))) ?? throw new Exception("Error serializing");

            // Act
            IEnumerable<StoreInventoryHistoryRow> result = _sut.MapStoreInventoryHistory(input);

            // Assert
            StoreInventoryHistoryRow singleResult = Assert.Single(result);
            Assert.Equal(testRow.DateOfService, singleResult.DateOfService);
            Assert.Equal(testRow.StoreNbr, singleResult.StoreNbr);
            Assert.Equal(testRow.NdcWithoutDashes, singleResult.NdcWithoutDashes);
            Assert.Equal(testRow.DrugName, singleResult.DrugName);
            Assert.Equal(testRow.Sdgi, singleResult.Sdgi);
            Assert.Equal(testRow.Gcn, singleResult.Gcn);
            Assert.Equal(testRow.GcnSequence, singleResult.GcnSequence);
            Assert.Equal(testRow.OrangeBookCode, singleResult.OrangeBookCode);
            Assert.Equal(testRow.FormCode, singleResult.FormCode);
            Assert.Equal(testRow.PackSize, singleResult.PackSize);
            Assert.Equal(testRow.TruePack, singleResult.TruePack);
            Assert.Equal(testRow.PM, singleResult.PM);
            Assert.Equal(testRow.IsPreferred, singleResult.IsPreferred);
            Assert.Equal(testRow.LastAcquisitionCostPack, singleResult.LastAcquisitionCostPack);
            Assert.Equal(testRow.LastAcquisitionCostUnit, singleResult.LastAcquisitionCostUnit);
            Assert.Equal(testRow.LastAcquisitionCostDate, singleResult.LastAcquisitionCostDate);
            Assert.Equal(testRow.OnHandQty, singleResult.OnHandQty);
            Assert.Equal(testRow.OnHandValue, singleResult.OnHandValue);
            Assert.Equal(testRow.ComittedQty, singleResult.ComittedQty);
            Assert.Equal(testRow.ComittedValue, singleResult.ComittedValue);
            Assert.Equal(testRow.TotalQTY, singleResult.TotalQTY);
            Assert.Equal(testRow.TotalValue, singleResult.TotalValue);
            Assert.Equal(testRow.AcquisitionCostPack, singleResult.AcquisitionCostPack);
            Assert.Equal(testRow.AcquisitionCostUnit, singleResult.AcquisitionCostUnit);
            Assert.Equal(testRow.PrimarySupplier, singleResult.PrimarySupplier);
            Assert.Equal(testRow.LastChangeDate, singleResult.LastChangeDate);
        }

        [Theory]
        [InlineData("A123456", TurnaroundTimeHelperImp.TAT_Target_EXCELLUS)]
        public void PopulateTatDerivedData_ShouldReturnedSameRawDataRowsPlusAdditionalDerivedDataProperties(string orderNbrScenario, string tatTargetScenario)
        {
            // Setup
            var rawDataInput = GetSampleTatRawDataFromMcKessonDW(orderNbrScenario);
            var expectedResult = GetExpectedTatRawDataPlusDerivedProperties(orderNbrScenario);

            _turnaroundTimeHelperMock.DeriveIsIntervention(Arg.Any<string>(), Arg.Any<string>()).Returns(p =>
            {
                string wfsdDescription = (string)p[0];
                string tatTarget = (string)p[1];

                if (wfsdDescription == "Adjudication" ||
                wfsdDescription == "Fill on Arrival" ||
                wfsdDescription == "General Exception" ||
                wfsdDescription == "Payment Complete Exception")
                    return true;

                if (wfsdDescription == "Contact Manager" &&
                    (tatTarget == "EXCELLUS" ||
                     tatTarget == "IHA"))
                    return true;

                return false;
            });

            _turnaroundTimeHelperMock.DeriveIsException(Arg.Any<string>()).Returns(p =>
            {
                string wfsdDescription = (string)p[0];

                if (wfsdDescription == "Contact Manager")
                    return true;

                return false;
            });

            _turnaroundTimeHelperMock.DeriveDaysInStep(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(p =>
            {
                DateTime dateIn = (DateTime)p[0];
                DateTime dateOut = (DateTime)p[1];

                return decimal.Round((decimal)(dateOut - dateIn).TotalDays, 6, MidpointRounding.AwayFromZero);
            });

            _turnaroundTimeHelperMock.DeriveDaysOffHours(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(p =>
            {
                DateTime dateIn = (DateTime)p[0];
                DateTime dateOut = (DateTime)p[1];

                DateTime tempDate = dateIn;
                DateTime tempDateBeginTime;
                DateTime tempDateEndTime;
                decimal tempSpanDays;
                decimal spanDaysOffHours = 0;
                decimal spanDaysWorkingHours = 0;

                while (tempDate < dateOut)
                {
                    tempDateBeginTime = dateIn >= tempDate ? dateIn : tempDate.Date;
                    tempDateEndTime = dateOut <= tempDate.Date.AddDays(1) ? dateOut : tempDate.Date.AddDays(1);
                    tempSpanDays = (decimal)(tempDateEndTime - tempDateBeginTime).TotalDays;

                    if (tempDate.DayOfWeek == DayOfWeek.Saturday ||
                        tempDate.DayOfWeek == DayOfWeek.Sunday ||
                        (tempDate.Month == 1 && tempDate.Day == 1) ||
                        (tempDate.Month == 7 && tempDate.Day == 4) ||
                        (tempDate.Month == 12 && tempDate.Day == 25))
                    {
                        //Post Office closed on weekends and on holidays.
                        //Three holidays have fixed dates, and others can be added upon request.
                        spanDaysOffHours += tempSpanDays;
                    }
                    else
                    {
                        //Regular Working Hours
                        spanDaysWorkingHours += tempSpanDays;
                    }

                    tempDate = tempDateEndTime;
                }

                return decimal.Round(spanDaysOffHours, 6, MidpointRounding.AwayFromZero);
            });


            // Act
            var actualResult = _sut.PopulateTatDerivedDetails(rawDataInput, tatTargetScenario);

            // Assert
            Assert.Equal(expectedResult.Count(), actualResult.Count());
            for (int i = 0; i < expectedResult.Count(); i++)
            {
                Assert.Equal(expectedResult.ElementAt(i).OrderNbr, actualResult.ElementAt(i).OrderNbr);
                Assert.Equal(expectedResult.ElementAt(i).Facility, actualResult.ElementAt(i).Facility);
                Assert.Equal(expectedResult.ElementAt(i).McKessonPatientKey, actualResult.ElementAt(i).McKessonPatientKey);
                Assert.Equal(expectedResult.ElementAt(i).RxNbr, actualResult.ElementAt(i).RxNbr);
                Assert.Equal(expectedResult.ElementAt(i).RefillNbr, actualResult.ElementAt(i).RefillNbr);
                Assert.Equal(expectedResult.ElementAt(i).DateIn, actualResult.ElementAt(i).DateIn);
                Assert.Equal(expectedResult.ElementAt(i).DateOut, actualResult.ElementAt(i).DateOut);
                Assert.Equal(expectedResult.ElementAt(i).WfsdKey, actualResult.ElementAt(i).WfsdKey);
                Assert.Equal(expectedResult.ElementAt(i).WfsdDescription, actualResult.ElementAt(i).WfsdDescription);
                Assert.Equal(expectedResult.ElementAt(i).IsIntervention, actualResult.ElementAt(i).IsIntervention);
                Assert.Equal(expectedResult.ElementAt(i).IsException, actualResult.ElementAt(i).IsException);
                Assert.Equal(expectedResult.ElementAt(i).ElapsedDaysInStep, actualResult.ElementAt(i).ElapsedDaysInStep);
                Assert.Equal(expectedResult.ElementAt(i).ElapsedDaysInStepOffHrs, actualResult.ElementAt(i).ElapsedDaysInStepOffHrs);
                Assert.Equal(expectedResult.ElementAt(i).DateInRank, actualResult.ElementAt(i).DateInRank);
                Assert.Equal(expectedResult.ElementAt(i).DateOutRank, actualResult.ElementAt(i).DateOutRank);
            }
        }

        [Theory]
        [InlineData("A123456", TurnaroundTimeHelperImp.TAT_Target_EXCELLUS)]
        public void PopulateTatDerivedSummary_ShouldReturnedSummaryRowsOfAggregatedRawDataRows(string orderNbrScenario, string tatTargetScenario)
        {
            // Setup
            var rawDataPlusDerivedDataInput = GetExpectedTatRawDataPlusDerivedProperties(orderNbrScenario);
            var expectedResult = GetExpectedTatSummaryRowsBasedOnSampleTatRawData(orderNbrScenario).OrderBy(r => r.RxNbr).ToList();

            _turnaroundTimeHelperMock.DeriveDaysInStep(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(p =>
            {
                DateTime dateIn = (DateTime)p[0];
                DateTime dateOut = (DateTime)p[1];

                return decimal.Round((decimal)(dateOut - dateIn).TotalDays, 6, MidpointRounding.AwayFromZero);
            });


            var actualResult = _sut.PopulateTatDerivedSummary(rawDataPlusDerivedDataInput, tatTargetScenario).OrderBy(r => r.RxNbr).ToList();

            // Assert
            Assert.Equal(expectedResult.Count(), actualResult.Count());
            for (int i = 0; i < expectedResult.Count(); i++)
            {
                Assert.Equal(expectedResult.ElementAt(i).OrderNbr, actualResult.ElementAt(i).OrderNbr);
                Assert.Equal(expectedResult.ElementAt(i).Facility, actualResult.ElementAt(i).Facility);
                Assert.Equal(expectedResult.ElementAt(i).RxNbr, actualResult.ElementAt(i).RxNbr);
                Assert.Equal(expectedResult.ElementAt(i).RefillNbr, actualResult.ElementAt(i).RefillNbr);
                Assert.Equal(expectedResult.ElementAt(i).DateIn, actualResult.ElementAt(i).DateIn);
                Assert.Equal(expectedResult.ElementAt(i).DateOut, actualResult.ElementAt(i).DateOut);
                Assert.Equal(expectedResult.ElementAt(i).DaysOverall, actualResult.ElementAt(i).DaysOverall);
                Assert.Equal(expectedResult.ElementAt(i).DeductDaysIntervention, actualResult.ElementAt(i).DeductDaysIntervention);
                Assert.Equal(expectedResult.ElementAt(i).DeductDaysOffHours, actualResult.ElementAt(i).DeductDaysOffHours);
                Assert.Equal(expectedResult.ElementAt(i).DaysNetTat, actualResult.ElementAt(i).DaysNetTat);
                Assert.Equal(expectedResult.ElementAt(i).HasExceptions, actualResult.ElementAt(i).HasExceptions);
            }
        }

        [Theory]
        [InlineData("A123456")]
        public void PopulateTatDerivedSummaryMaxRx_ShouldReturnedMaxRxSummaryRowsOfAggregatedSummaryRows(string orderNbrScenario)
        {
            // Setup
            var summaryDataInput = GetExpectedTatSummaryRowsBasedOnSampleTatRawData(orderNbrScenario);
            IEnumerable<TatSummaryMaxRxRow> expectedResult = GetExpectedTatSummaryMaxRxRowsBasedOnSampleTatRawData(orderNbrScenario);

            // Act
            IEnumerable<TatSummaryMaxRxRow> actualResult = _sut.GetOneRxWithinAnOrderHavingLargestDaysNetTat(summaryDataInput);

            // Assert
            Assert.Equal(expectedResult.Count(), actualResult.Count());
            for (int i = 0; i < expectedResult.Count(); i++)
            {
                Assert.Equal(expectedResult.ElementAt(i).OrderNbr, actualResult.ElementAt(i).OrderNbr);
                Assert.Equal(expectedResult.ElementAt(i).Facility, actualResult.ElementAt(i).Facility);
                Assert.Equal(expectedResult.ElementAt(i).DateIn, actualResult.ElementAt(i).DateIn);
                Assert.Equal(expectedResult.ElementAt(i).DateOut, actualResult.ElementAt(i).DateOut);
                Assert.Equal(expectedResult.ElementAt(i).DaysOverall, actualResult.ElementAt(i).DaysOverall);
                Assert.Equal(expectedResult.ElementAt(i).DeductDaysIntervention, actualResult.ElementAt(i).DeductDaysIntervention);
                Assert.Equal(expectedResult.ElementAt(i).DeductDaysOffHours, actualResult.ElementAt(i).DeductDaysOffHours);
                Assert.Equal(expectedResult.ElementAt(i).DaysNetTat, actualResult.ElementAt(i).DaysNetTat);
                Assert.Equal(expectedResult.ElementAt(i).CountRxInOrder, actualResult.ElementAt(i).CountRxInOrder);
                Assert.Equal(expectedResult.ElementAt(i).HasExceptions, actualResult.ElementAt(i).HasExceptions);
            }
        }

        [Theory]
        [InlineData("425", (long)425)]
        [InlineData("", null)]
        public void MapSureScriptsPhysicianNotificationLetterRawDataToFinalReport_Handles_EmptySetDefaultStringConsideringStateResult(
            string setDefaultStringConsideringStateReturn,
            long? expectedInternalPrimaryCareProviderId
        )
        {
            // Arrange
            string state = "AB";
            bool isStatePA = true;
            long? internalPrimaryCareProviderId = 38450938;
            _sureScriptsHelperMock.IsStatePA(state, state).Returns(isStatePA);
            _sureScriptsHelperMock.SetDefaultStringConsideringState(internalPrimaryCareProviderId.ToString(), isStatePA).Returns(setDefaultStringConsideringStateReturn);

            // Act
            IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> res = _sut.MapSureScriptsPhysicianNotificationLetterRawDataToFinalReport([
                new SureScriptsPhysicianNotificationLetterRawDataRow {
                    FacilityState = state,
                    InternalPrimaryCareProviderId = internalPrimaryCareProviderId,
                }
            ], state);

            // Assert
            SureScriptsPhysicianNotificationLetterReportRow sin = Assert.Single(res);
            Assert.Equal(expectedInternalPrimaryCareProviderId, sin.InternalPrimaryCareProviderId);
        }

        public IEnumerable<TatDetailsRow> GetExpectedTatRawDataPlusDerivedProperties(string orderNbrScenario)
        {
            //NOTE: This is a sample data has one Order Nbr for three prescriptions and two refill requests.
            List<TatDetailsRow> rawDataRows =
            [
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "11111111", RefillNbr = 0, DateIn = DateTime.Parse("6/28/2024 1:22:59 PM"), DateOut = DateTime.Parse("6/28/2024 1:23:00 PM"), WfsdKey = 505, WfsdDescription = "Contact Manager", IsIntervention = "Y", IsException = "Y", ElapsedDaysInStep = 0.000012M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 1, DateOutRank = 2 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "11111111", RefillNbr = 0, DateIn = DateTime.Parse("6/29/2024 11:54:45 AM"), DateOut = DateTime.Parse("6/29/2024 11:55:16 AM"), WfsdKey = 506, WfsdDescription = "Data Entry", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.939074M, ElapsedDaysInStepOffHrs = 0.496713M, DateInRank = 2, DateOutRank = 1 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("6/28/2024 1:23:32 PM"), DateOut = DateTime.Parse("6/28/2024 1:23:33 PM"), WfsdKey = 505, WfsdDescription = "Contact Manager", IsIntervention = "Y", IsException = "Y", ElapsedDaysInStep = 0.000012M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 1, DateOutRank = 10 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("6/29/2024 12:00:11 PM"), DateOut = DateTime.Parse("6/29/2024 12:00:34 PM"), WfsdKey = 506, WfsdDescription = "Data Entry", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.942373M, ElapsedDaysInStepOffHrs = 0.500394M, DateInRank = 2, DateOutRank = 9 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("6/29/2024 12:00:35 PM"), DateOut = DateTime.Parse("6/29/2024 12:00:43 PM"), WfsdKey = 507, WfsdDescription = "Pre-Edit", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.000104M, ElapsedDaysInStepOffHrs = 0.000104M, DateInRank = 3, DateOutRank = 8 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/3/2024 8:43:32 AM"), DateOut = DateTime.Parse("7/3/2024 8:43:43 AM"), WfsdKey = 509, WfsdDescription = "Drug Utilization Review", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 3.863194M, ElapsedDaysInStepOffHrs = 1.499502M, DateInRank = 4, DateOutRank = 7 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/3/2024 8:44:03 AM"), DateOut = DateTime.Parse("7/3/2024 8:44:03 AM"), WfsdKey = 1029, WfsdDescription = "Order Grouping", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.000231M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 5, DateOutRank = 6 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/3/2024 8:50:13 AM"), DateOut = DateTime.Parse("7/3/2024 8:50:13 AM"), WfsdKey = 1030, WfsdDescription = "Pre-Authorization Pending", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.004282M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 6, DateOutRank = 5 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/7/2024 2:35:04 PM"), DateOut = DateTime.Parse("7/7/2024 2:37:18 PM"), WfsdKey = 1529, WfsdDescription = "Payment Exception", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 4.241030M, ElapsedDaysInStepOffHrs = 2.609236M, DateInRank = 7, DateOutRank = 4 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/7/2024 2:37:18 PM"), DateOut = DateTime.Parse("7/7/2024 2:39:16 PM"), WfsdKey = 1030, WfsdDescription = "Pre-Authorization Pending", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.001366M, ElapsedDaysInStepOffHrs = 0.001366M, DateInRank = 8, DateOutRank = 3 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "996", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/8/2024 11:55:34 AM"), DateOut = DateTime.Parse("7/8/2024 11:55:34 AM"), WfsdKey = 512, WfsdDescription = "Automated Dispensing", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.886319M, ElapsedDaysInStepOffHrs = 0.389398M, DateInRank = 9, DateOutRank = 2 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("7/8/2024 11:59:37 AM"), DateOut = DateTime.Parse("7/8/2024 11:59:37 AM"), WfsdKey = 1031, WfsdDescription = "Completion Pending", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.002813M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 10, DateOutRank = 1 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("6/29/2024 11:55:24 AM"), DateOut = DateTime.Parse("6/29/2024 11:56:08 AM"), WfsdKey = 506, WfsdDescription = "Data Entry", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.000509M, ElapsedDaysInStepOffHrs = 0.000509M, DateInRank = 1, DateOutRank = 8 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/3/2024 8:43:37 AM"), DateOut = DateTime.Parse("7/3/2024 8:44:03 AM"), WfsdKey = 509, WfsdDescription = "Drug Utilization Review", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 3.866609M, ElapsedDaysInStepOffHrs = 1.502685M, DateInRank = 2, DateOutRank = 7 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/3/2024 8:44:03 AM"), DateOut = DateTime.Parse("7/3/2024 8:44:03 AM"), WfsdKey = 1029, WfsdDescription = "Order Grouping", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 3, DateOutRank = 6 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/3/2024 8:50:13 AM"), DateOut = DateTime.Parse("7/3/2024 8:50:13 AM"), WfsdKey = 1030, WfsdDescription = "Pre-Authorization Pending", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.004282M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 4, DateOutRank = 5 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/6/2024 9:56:41 AM"), DateOut = DateTime.Parse("7/7/2024 2:37:18 PM"), WfsdKey = 1529, WfsdDescription = "Payment Exception", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 4.241030M, ElapsedDaysInStepOffHrs = 2.609236M, DateInRank = 5, DateOutRank = 4 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/7/2024 2:39:16 PM"), DateOut = DateTime.Parse("7/7/2024 2:39:16 PM"), WfsdKey = 1030, WfsdDescription = "Pre-Authorization Pending", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.001366M, ElapsedDaysInStepOffHrs = 0.001366M, DateInRank = 6, DateOutRank = 3 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "996", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/8/2024 11:55:34 AM"), DateOut = DateTime.Parse("7/8/2024 11:55:34 AM"), WfsdKey = 512, WfsdDescription = "Automated Dispensing", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.886319M, ElapsedDaysInStepOffHrs = 0.389398M, DateInRank = 7, DateOutRank = 2 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("7/8/2024 11:59:37 AM"), DateOut = DateTime.Parse("7/8/2024 11:59:37 AM"), WfsdKey = 1031, WfsdDescription = "Completion Pending", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0.002813M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 8, DateOutRank = 1 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "RR4444444", RefillNbr = 0, DateIn = DateTime.Parse("6/17/2024 11:02:44 AM"), DateOut = DateTime.Parse("6/17/2024 11:02:44 AM"), WfsdKey = 502, WfsdDescription = "Saved Order - Reception", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 1, DateOutRank = 1 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "RR5555555", RefillNbr = 0, DateIn = DateTime.Parse("6/17/2024 11:02:45 AM"), DateOut = DateTime.Parse("6/17/2024 11:02:45 AM"), WfsdKey = 502, WfsdDescription = "Saved Order - Reception", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 1, DateOutRank = 1 },
                new TatDetailsRow { OrderNbr = "A123456", Facility = "199", McKessonPatientKey = 1234567, RxNbr = "", RefillNbr = 0, DateIn = DateTime.Parse("6/28/2024 1:23:46 PM"), DateOut = DateTime.Parse("6/28/2024 1:23:46 PM"), WfsdKey = 502, WfsdDescription = "Saved Order - Reception", IsIntervention = "N", IsException = "N", ElapsedDaysInStep = 0M, ElapsedDaysInStepOffHrs = 0M, DateInRank = 1, DateOutRank = 1 },
            ];
            return rawDataRows.Where(r => r.OrderNbr == orderNbrScenario).ToList();
        }

        public IEnumerable<TatRawDataRow> GetSampleTatRawDataFromMcKessonDW(string orderNbrScenario)
        {
            //Simply set derived data properties to null
            List<TatDetailsRow> detailRows = GetExpectedTatRawDataPlusDerivedProperties(orderNbrScenario).ToList();
            List<TatRawDataRow> returnRows = new List<TatRawDataRow>();

            foreach (var rawDataRow in detailRows)
            {
                returnRows.Add(new TatRawDataRow
                {
                    OrderNbr = rawDataRow.OrderNbr,
                    Facility = rawDataRow.Facility,
                    McKessonPatientKey = rawDataRow.McKessonPatientKey,
                    RxNbr = rawDataRow.RxNbr,
                    RefillNbr = rawDataRow.RefillNbr,
                    DateIn = rawDataRow.DateIn,
                    DateOut = rawDataRow.DateOut,
                    WfsdKey = rawDataRow.WfsdKey,
                    WfsdDescription = rawDataRow.WfsdDescription
                });
            }

            return returnRows;
        }

        public IEnumerable<TatSummaryRow> GetExpectedTatSummaryRowsBasedOnSampleTatRawData(string orderNbrScenario)
        {
            List<TatSummaryRow> summaryRows =
            [
                new TatSummaryRow { OrderNbr = "A123456", Facility = "199", RxNbr = "11111111", RefillNbr = 0, DateIn = DateTime.Parse("6/28/2024 1:22:59 PM"), DateOut = DateTime.Parse("6/29/2024 11:55:16 AM"), DaysOverall = 0.939086M, DeductDaysIntervention = 0.000012M, DeductDaysOffHours = 0.496713M, DaysNetTat = 0.442361M, HasExceptions = "Y" },
                new TatSummaryRow { OrderNbr = "A123456", Facility = "199", RxNbr = "22222222", RefillNbr = 0, DateIn = DateTime.Parse("6/28/2024 1:23:32 PM"), DateOut = DateTime.Parse("7/8/2024 11:59:37 AM"), DaysOverall = 9.941725M, DeductDaysIntervention = 0.000012M, DeductDaysOffHours = 5.000000M, DaysNetTat = 4.941713M, HasExceptions = "Y" },
                new TatSummaryRow { OrderNbr = "A123456", Facility = "199", RxNbr = "33333333", RefillNbr = 0, DateIn = DateTime.Parse("6/29/2024 11:55:24 AM"), DateOut = DateTime.Parse("7/8/2024 11:59:37 AM"), DaysOverall = 9.002928M, DeductDaysIntervention = 0M, DeductDaysOffHours = 4.503194M, DaysNetTat = 4.499734M, HasExceptions ="N" },
            ];
            return summaryRows.Where(r => r.OrderNbr == orderNbrScenario).ToList();
        }

        public IEnumerable<TatSummaryMaxRxRow> GetExpectedTatSummaryMaxRxRowsBasedOnSampleTatRawData(string orderNbrScenario)
        {
            List<TatSummaryMaxRxRow> summaryMaxRxRows =
            [
                new TatSummaryMaxRxRow { OrderNbr = "A123456", Facility = "199", DateIn = DateTime.Parse("6/28/2024 1:23:32 PM"), DateOut = DateTime.Parse("7/8/2024 11:59:37 AM"), DaysOverall = 9.941725M, DeductDaysIntervention = 0.000012M, DeductDaysOffHours = 5M, DaysNetTat = 4.941713M, CountRxInOrder = 3, HasExceptions = "Y" },
            ];
            return summaryMaxRxRows.Where(r => r.OrderNbr == orderNbrScenario).ToList();
        }
    }
}
