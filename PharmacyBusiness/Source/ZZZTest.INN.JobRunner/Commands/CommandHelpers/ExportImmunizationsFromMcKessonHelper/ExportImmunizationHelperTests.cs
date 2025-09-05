using INN.JobRunner.Commands.CommandHelpers.ExportImmunizationsFromMcKessonHelper;
using Library.McKessonCPSInterface.DataModel;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.ExportImmunizationsFromMcKessonHelper
{
    public class ExportImmunizationHelperTest
    {
        private readonly ExportImmunizationHelperImp _sut;

        public ExportImmunizationHelperTest()
        {
            _sut = new ExportImmunizationHelperImp();
        }

        [Fact]
        public void GetStoreProductNumberWhereNdcwoEqualsStrProductNdc_ShouldReturnExpectedResult()
        {
            // Arrange
            string expectedResult = "DrugA";
            IEnumerable<NdcConversionRow> ndcConversionRow = GetDataForNdcConversionRows();
            ImmunizationRow immunizationRow = GetMock_1_ImmunizationRow();

            // Act
            string? actualResult = _sut.GetStoreProductNumberWhereNdcwoEqualsStrProductNdc(ndcConversionRow, immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }


        [Fact]
        public void GetStoreProductNdcWhereNdcwoEqualsStrProductNdc_ShouldReturnExpectedResult()
        {
            // Arrange
            string expectedResult = "54321";
            IEnumerable<NdcConversionRow> ndcConversionRow = GetDataForNdcConversionRows();
            ImmunizationRow immunizationRow = GetMock_1_ImmunizationRow();

            // Act
            string? actualResult = _sut.GetStoreProductNdcWhereNdcwoEqualsStrProductNdc(ndcConversionRow, immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Fact]
        public void GetRaceRecipOneWherePatientNumberEqualsDecInternalPtNum_ShouldReturnExpectedResult()
        {
            // Arrange
            string expectedResult = "2106-3";
            IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows = GetImmunizationQuestionareDataForTests();
            ImmunizationRow immunizationRow = GetMock_1_ImmunizationRow();

            // Act
            string? actualResult = _sut.GetRaceRecipOneWherePatientNumberEqualsDecInternalPtNum(immunizationQuestionnaireRows, immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }


        [Fact]
        public void GetRecipEthnicityWherePatientNumberEqualsDecInternalPtNum_ShouldReturnExpectedResult()
        {
            // Arrange
            string expectedResult = "2135-2";
            IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows = GetImmunizationQuestionareDataForTests();
            ImmunizationRow immunizationRow = GetMock_2_ImmunizationRow();

            // Act
            string? actualResult = _sut.GetRecipEthnicityWherePatientNumberEqualsDecInternalPtNum(immunizationQuestionnaireRows, immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }


        [Fact]
        public void GetNyPriorityGroupWherePatientNumberEqualsDecInternalPtNum_ShouldReturnExpectedResult()
        {
            // Arrange
            string expectedResult = "HCPOTHER";
            IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows = GetImmunizationQuestionareDataForTests();
            ImmunizationRow immunizationRow = GetMock_3_ImmunizationRow();

            // Act
            string? actualResult = _sut.GetNyPriorityGroupWherePatientNumberEqualsDecInternalPtNum(immunizationQuestionnaireRows, immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("ENGLISH", "ENG")]
        [InlineData("SPANISH", "UNK")]
        [InlineData("", "UNK")]
        [InlineData(null, "UNK")]
        public void GetPrimaryLanguage_ShouldReturnExpectedResult(string primaryLanguage, string expectedResult)
        {
            // Arrange
            ImmunizationRow immunizationRow = GetMock_1_ImmunizationRow();
            immunizationRow.PrimaryLanguage = primaryLanguage;

            // Act
            string? actualResult = _sut.GetPrimaryLanguage(immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("", "UNK")]
        [InlineData(null, "UNK")]
        [InlineData("White", "White")]
        public void GetRecipEthnicity_ShouldReturnExpectedResult(string recipEthinicity, string expectedResult)
        {
            // Arrange
            ImmunizationRow immunizationRow = GetMock_1_ImmunizationRow();
            immunizationRow.RecipEthnicity = recipEthinicity;

            // Act
            string? actualResult = _sut.GetRecipEthnicity(immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("", "UNK")]
        [InlineData(null, "UNK")]
        [InlineData("White", "White")]
        public void GetRecipRaceOne_ShouldReturnExpectedResult(string recipRaceOne, string expectedResult)
        {
            // Arrange
            ImmunizationRow immunizationRow = GetMock_1_ImmunizationRow();
            immunizationRow.RecipRaceOne = recipRaceOne;

            // Act
            string? actualResult = _sut.GetRecipRaceOne(immunizationRow);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }
        private IEnumerable<ImmunizationQuestionnaireRow> GetImmunizationQuestionareDataForTests()
        {
            //NOTE: This is a sample data has one Order Nbr for three prescriptions and two refill requests.
            return [
                new ImmunizationQuestionnaireRow { StoreNbr = "012", PatientNum = 12,       RxNbr = "11111111",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "001", PatientNum = 2627232,  RxNbr = "11111111",  RefillNbr = 0, KeyDesc = "Ethnicity", KeyValue = "Hispanic or Latino", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "013", PatientNum = 817293,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "COVID Priority Group - NY Stores ONLY", KeyValue = "Healthcare Provider - Other", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "012", PatientNum = 123456,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "024", PatientNum = 199283,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "009", PatientNum = 0872791,  RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "074", PatientNum = 430139,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "224", PatientNum = 727182,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "Asian", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "016", PatientNum = 827182,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "012", PatientNum = 627392,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "Other", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "081", PatientNum = 842341,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "083", PatientNum = 648291,   RxNbr = "22222222",  RefillNbr = 0, KeyDesc = "Asian", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "017", PatientNum = 238493,   RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "002", PatientNum = 589324,   RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "014", PatientNum = 999999,   RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "035", PatientNum = 728492,   RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "Asian", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "019", PatientNum = 184895,   RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "022", PatientNum = 7164832,  RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "Asian", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "018", PatientNum = 817394,   RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "Native American or Alaska Native", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "013", PatientNum = 1234533,  RxNbr = "33333333",  RefillNbr = 0, KeyDesc = "White", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "054", PatientNum = null,     RxNbr = "RR4444444", RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "067", PatientNum = 638741,   RxNbr = "RR5555555", RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) },
                new ImmunizationQuestionnaireRow { StoreNbr = "022", PatientNum = 752811, RxNbr = "", RefillNbr = 0, KeyDesc = "Black or African American", KeyValue = "true", CreateDate = new DateTime(2024, 01, 01) }
            ];
        }

        private ImmunizationRow GetMock_1_ImmunizationRow()
        {
            return new ImmunizationRow
            {
                StrFirstName = "Michael",
                StrMiddleName = "F",
                StrLastName = "Bauer",
                DtBirthDate = "20240916",
                CGender = "M",
                StrAddressOne = "Wegmans",
                StrAddressTwo = null,
                StrCity = "Rochester",
                StrState = "Ny",
                StrZip = "14626",
                StrProductNumber = "1",
                StrProductNdc = "12345",
                DecDispensedQty = "3",
                CGenericIndentifier = null,
                StrVerifiedRphFirst = "4",
                StrVerifiedRphLast = "5",
                StrFacilityId = "012",
                StrRxNumber = "11111111",
                StrDosageForm = "9",
                StrStrength = "10",
                DtSoldDate = "20240915",
                DtCanceledDate = "20240915",
                CActionCode = "9",
                StrDea = "10",
                StrNpi = "11",
                DecInternalPtNum = 12,
                LotNumber = "13",
                ExpDate = "14",
                StrPatientMail = "15",
                VisPresentedDate = "16",
                AdministrationSite = "17",
                ProtectionIndicator = "18",
                RecipRaceOne = "19",
                RecipEthnicity = "20",
                VfcStatus = "21",
                NyPriorityGroup = "22",
                PhoneNumber = "23",
                PrescribedById = "24",
                RefillNumber = 0
            };
        }

        private ImmunizationRow GetMock_2_ImmunizationRow()
        {
            return new ImmunizationRow
            {
                StrFirstName = "Michael",
                StrMiddleName = "F",
                StrLastName = "Bauer",
                DtBirthDate = "20240916",
                CGender = "M",
                StrAddressOne = "Wegmans",
                StrAddressTwo = null,
                StrCity = "Rochester",
                StrState = "Ny",
                StrZip = "14626",
                StrProductNumber = "1",
                StrProductNdc = "12345",
                DecDispensedQty = "3",
                CGenericIndentifier = null,
                StrVerifiedRphFirst = "4",
                StrVerifiedRphLast = "5",
                StrFacilityId = "001",
                StrRxNumber = "11111111",
                StrDosageForm = "9",
                StrStrength = "10",
                DtSoldDate = "20240915",
                DtCanceledDate = "20240915",
                CActionCode = "9",
                StrDea = "10",
                StrNpi = "11",
                DecInternalPtNum = 2627232,
                LotNumber = "13",
                ExpDate = "14",
                StrPatientMail = "15",
                VisPresentedDate = "16",
                AdministrationSite = "17",
                ProtectionIndicator = "18",
                RecipRaceOne = "19",
                RecipEthnicity = "20",
                VfcStatus = "21",
                NyPriorityGroup = "22",
                PhoneNumber = "23",
                PrescribedById = "24",
                RefillNumber = 0
            };
        }

        private ImmunizationRow GetMock_3_ImmunizationRow()
        {
            return new ImmunizationRow
            {
                StrFirstName = "Michael",
                StrMiddleName = "F",
                StrLastName = "Bauer",
                DtBirthDate = "20240916",
                CGender = "M",
                StrAddressOne = "Wegmans",
                StrAddressTwo = null,
                StrCity = "Rochester",
                StrState = "Ny",
                StrZip = "14626",
                StrProductNumber = "1",
                StrProductNdc = "12345",
                DecDispensedQty = "3",
                CGenericIndentifier = null,
                StrVerifiedRphFirst = "4",
                StrVerifiedRphLast = "5",
                StrFacilityId = "013",
                StrRxNumber = "22222222",
                StrDosageForm = "9",
                StrStrength = "10",
                DtSoldDate = "20240915",
                DtCanceledDate = "20240915",
                CActionCode = "9",
                StrDea = "10",
                StrNpi = "11",
                DecInternalPtNum = 817293,
                LotNumber = "13",
                ExpDate = "14",
                StrPatientMail = "15",
                VisPresentedDate = "16",
                AdministrationSite = "17",
                ProtectionIndicator = "18",
                RecipRaceOne = "19",
                RecipEthnicity = "20",
                VfcStatus = "21",
                NyPriorityGroup = "22",
                PhoneNumber = "23",
                PrescribedById = "24",
                RefillNumber = 0
            };
        }

        private IEnumerable<NdcConversionRow> GetDataForNdcConversionRows()
        {
            return
            [
                new NdcConversionRow ("12345", "54321", "DrugA"),
                new NdcConversionRow ("67890", "09876", "DrugB"),
                new NdcConversionRow ("11111", "11111", "DrugC")
            ];
        }
    }
}
