using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.Helper;

namespace ZZZTest.Library.McKessonDWInterface.Helper;

public class SureScriptsHelperTests
{
    private readonly SureScriptsHelperImp _sut;

    public SureScriptsHelperTests()
    {
        _sut = new SureScriptsHelperImp();
    }

    [Theory]
    [InlineData("PA", "NY", false)]
    [InlineData("PA", "PA", true)]
    [InlineData("", "PA", false)]
    [InlineData("PA", "MA", false)]
    public void IsStatePa_ShouldReturnExpectedResult(string? state, string reportState, bool expectedResult)
    {
        // Arrange

        // Act
        bool actualResult = _sut.IsStatePA(state, reportState);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }


    [Theory]
    [InlineData("PA", "")]
    [InlineData("162739-82-10001", "1627398210001")]
    [InlineData("10/01/2003", "10012003")]
    [InlineData("121-223-920", "121223920")]
    [InlineData("121-223*920....", "121223920")]
    [InlineData("", "")]
    [InlineData("1829 182", "1829182")]
    public void SetDigitsOnly_ShouldReturnExpectedResult(string? text, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetDigitsOnly(text);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void SetDefaultDate_ShouldReturnExpectedResult()
    {
        // Arrange
        DateTime? date = null;
        DateTime expectedResult = new DateTime(1900, 1, 1);

        // Act
        DateTime actualResult = _sut.SetDefaultDate(date);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void SetNullableValueString_ShouldReturnExpectedResult()
    {
        // Arrange
        string? text = null;
        string expectedResult = "NotAvailable";

        // Act
        string actualResult = _sut.SetNullableValue(text, expectedResult);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void SetNullableValueDecimal_ShouldReturnExpectedResult()
    {
        // Arrange
        decimal? value = null;
        decimal expectedResult = 0m;

        // Act
        decimal actualResult = _sut.SetNullableValue(value, expectedResult);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void SetNullableValueLong_ShouldReturnExpectedResult()
    {
        // Arrange
        long? value = null;
        long expectedResult = 0;

        // Act
        long actualResult = _sut.SetNullableValue(value, expectedResult);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("Some|Data*~", "SomeData")]
    [InlineData("Testing:123", "Testing123")]
    [InlineData("Testing", "Testing")]
    public void SetDefaultString_ShouldReturnExpectedResult(string? text, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetDefaultString(text);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData(null, 5, '0', "")]
    [InlineData("510", 2, '1', "510")]
    [InlineData("10", 0, '0', "10")]
    [InlineData("5", 3, '4', "445")]
    public void LPad_ShouldReturnExpectedResult(string? text, int value, char valueToPadWith, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.LPad(text, value, valueToPadWith);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void DispensedQuantityModifier_ShouldReturnExpectedResult()
    {
        // Arrange
        decimal value = 1m;
        decimal expectedResult = 1000m;

        // Act
        decimal actualResult = _sut.DispensedQuantityModifier(value);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("100.00", 99)]
    [InlineData("-1.32", 0)]
    [InlineData("5.65", 5)]
    public void SetRefillsRemaining_ShouldReturnExpectedResult(string? value, decimal expectedResult)
    {
        // Arrange
        decimal? decimalValue = Convert.ToDecimal(value);

        // Act
        decimal actualResult = _sut.SetRefillsRemaining(decimalValue);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("testing", false, "testing")]
    [InlineData("testing", true, "")]
    [InlineData("PatientName", true, "")]
    public void SetDefaultStringConsideringState_ShouldReturnExpectedResult(string? text, bool isPa, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetDefaultStringConsideringState(text, isPa);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("INFLUENZA VIRUS VACCINES", "nasal", "NASAL SPRAY")]
    [InlineData("INFLUENZA VIRUS VACCINES", "anything", "INJECTION")]
    public void SetRouteOfAdministration_ShouldReturnExpectedResult(string? vaccineInformationStatementName, string? routeOfAdministrationDescription, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetRouteOfAdministration(vaccineInformationStatementName, routeOfAdministrationDescription);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("INFLUENZA VIRUS VACCINES", "nasal", "NOSTRIL")]
    [InlineData("INFLUENZA VIRUS VACCINES", "anything", "UPPER ARM")]
    public void SetSiteOfAdministration_ShouldReturnExpectedResult(string? vaccineInformationStatementName, string? routeOfAdministrationDescription, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetSiteOfAdministration(vaccineInformationStatementName, routeOfAdministrationDescription);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("00000000010", "", "SEQIRUS, INC.")]
    [InlineData("5", "Anything", "Anything")]
    [InlineData("00000000018", "something", "SEQIRUS, INC.")]
    public void SetVaccineManufacturerName_ShouldReturnExpectedResult(string? drugNdc, string? drugLabeler, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetVaccineManufacturerName(drugNdc, drugLabeler);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("00000000012", "", "AFLURIA TRI")]
    [InlineData("5", "Anything", "Anything")]
    [InlineData("00000000018", "something", "AFLURIA TRI")]
    [InlineData("00000000036", "something", "AFLURIA QUAD")]
    public void SetVaccineName_ShouldReturnExpectedResult(string? drugNdc, string? drugLabelName, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetVaccineName(drugNdc, drugLabelName);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("Something-to-test-192", "", "", "", "Somethingtotest192")]
    [InlineData(null, "COVID-19 VACCINES", "PFIZER", "", "208")]
    [InlineData(null, "INFLUENZA VIRUS VACCINES", "", "66019030801", "149")]
    [InlineData(null, "DoesntFitCriteria", "Maker", "1000", "")]
    public void SetVaccineCvxCode_ShouldReturnExpectedResult(string? cvxCode, string? vaccineInformationStatementName, string? drugLabeler, string? drugNdc, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.SetVaccineCvxCode(cvxCode, vaccineInformationStatementName, drugLabeler, drugNdc);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void GetMedHistoryTrailerRecord_ShouldReturnExpectedResult()
    {
        // Arrange
        var recordCount = 12;
        var expectedResult = new SureScriptsMedicalHistoryTrailerRow
        {
            RecordType = "TRL",
            TotalRecords = recordCount.ToString()
        };

        // Act
        var actualResult = _sut.GetMedHistoryTrailerRecord(recordCount);

        // Assert
        Assert.Equivalent(actualResult, expectedResult);
    }

    [Fact]
    public void GetMedHistoryHeaderRecord_ShouldReturnExpectedResult()
    {
        // Arrange
        DateTime runDate = DateTime.Now;
        DateOnly runfor = DateOnly.FromDateTime(runDate);

        var expectedResult = new SureScriptsMedicalHistoryHeaderRow
        {
            RecordType = "HDR",
            VersionReleaseNumber = "6.2",
            SenderId = null,
            SenderParticipantPassword = null,
            ReceiverId = "S00000000000001",
            SourceName = null,
            TransmissionControlNumber = "0000001000",
            TransmissionDate = runDate.ToString("yyyyMMdd"),
            TransmissionTime = $"{runDate:HHmmss}01",
            TransmissionFileType = "PEF",
            TransmissionAction = "A",
            ExtractDate = runfor.ToString("yyyyMMdd"),
            FileType = "P"
        };

        // Act
        var actualResult = _sut.GetMedHistoryHeaderRecord(runfor);

        // Assert
        Assert.Equivalent(actualResult, expectedResult);
    }

    [Theory]
    [InlineData(5, "|", "TRL|5")]
    [InlineData(null, ",", "TRL,")]
    [InlineData(2, "|", "TRL|2")]
    public void GetPNLTrailerRecord_ShouldReturnExpectedResult(int? recordCount, string delimeter, string expectedResult)
    {
        // Arrange

        // Act
        string actualResult = _sut.GetPNLTrailerRecord(recordCount, delimeter);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void GetPNLHeaderRecord_ShouldReturnExpectedResult()
    {
        // Arrange
        DateOnly runfor = DateOnly.FromDateTime(DateTime.Now);
        string delimeter = "|";
        string expectedResult = $"HDR{delimeter}" +
               $"020{delimeter}" +
               $"P00000000022610{delimeter}" +
               $"{delimeter}" +
               $"S00000000000001{delimeter}" +
               $"{delimeter}" +
               $"0000001000{delimeter}" +
               $"{DateTime.Now.ToString("yyyyMMdd")}{delimeter}" +
               $"{DateTime.Now.ToString("HH24mmss")}{delimeter}" +
               $"IMM{delimeter}" +
               $"A{delimeter}" +
               $"{runfor.ToString("yyyyMMdd")}{delimeter}" +
               $"P";

        // Act
        string actualResult = _sut.GetPNLHeaderRecord(runfor, delimeter);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void FormatDateString_ShouldReturnExpectedResult()
    {
        // Arrange
        DateTime date = DateTime.Now;
        string expectedResult = date.ToString("yyyyMMdd");

        // Act
        string actualResult = _sut.FormatDateString(date);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData(null, 5, null)]
    [InlineData("Testing", 5, "Testi")]
    [InlineData("Testing", 0, "")]
    [InlineData("Testing", 10, "Testing")]
    public void SetStringToMaxLength_ShouldReturnExpectedResult(string? input, int maxLength, string? expectedResult)
    {
        // Arrange

        // Act
        var actualResult = _sut.SetStringToMaxLength(input, maxLength);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void RemoveTrailingZeroFromDecimal_ShouldReturnExpectedResult()
    {
        // Arrange
        decimal? input1 = null;
        string? expectedResult1 = null;

        decimal? input2 = 1000;
        string? expectedResult2 = "1000";

        decimal? input3 = 2000.0m;
        string? expectedResult3 = "2000";

        // Act
        var actualResult1 = _sut.RemoveTrailingZeroFromDecimal(input1);
        var actualResult2 = _sut.RemoveTrailingZeroFromDecimal(input2);
        var actualResult3 = _sut.RemoveTrailingZeroFromDecimal(input3);

        // Assert
        Assert.Equal(actualResult1, expectedResult1);
        Assert.Equal(actualResult2, expectedResult2);
        Assert.Equal(actualResult3, expectedResult3);
    }

    [Theory]
    [InlineData("99999999999", 1, 1)]
    [InlineData(null, 1, 0)]
    [InlineData("99999999999", null, 0)]
    [InlineData("11111111111", 1, 0)]
    public void DeriveCompoundIngredientSequenceNumber_ShouldReturnExpectedResult(string? ndc, int? rank, int? expectedResult)
    {
        // Arrange

        // Act
        var actualResult = _sut.DeriveCompoundIngredientSequenceNumber(ndc, rank);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Fact]
    public void DeDuplicateMedHistoryRows_ShouldReturnUniqueRowsByTopRankedPatientAddressAndPrescriberPhone()
    {
        // Arrange
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "16",
                Logic_PatientAddressCreateDate = new DateTime(2018, 10, 16, 21, 40, 39),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "A",
                Logic_PresPhoneSourceCode = "I",
                Logic_PresPhoneHLevel = 1
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "16",
                Logic_PatientAddressCreateDate = new DateTime(2018, 10, 16, 21, 40, 39),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "A",
                Logic_PresPhoneSourceCode = "I",
                Logic_PresPhoneHLevel = 1
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "1",
                Logic_PatientAddressCreateDate = new DateTime(2018, 6, 22, 16, 48, 39),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "A",
                Logic_PresPhoneSourceCode = "I",
                Logic_PresPhoneHLevel = 1
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "16",
                Logic_PatientAddressCreateDate = new DateTime(2018, 10, 16, 21, 40, 39),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "I",
                Logic_PresPhoneSourceCode = "I",
                Logic_PresPhoneHLevel = 2
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "16",
                Logic_PatientAddressCreateDate = new DateTime(2018, 10, 16, 21, 40, 39),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "I",
                Logic_PresPhoneSourceCode = "I",
                Logic_PresPhoneHLevel = 2
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "1",
                Logic_PatientAddressCreateDate = new DateTime(2018, 6, 22, 16, 48, 39),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "I",
                Logic_PresPhoneSourceCode = "I",
                Logic_PresPhoneHLevel = 2
            },
        };

        // Act
        var result = _sut.DeDuplicateMedHistoryRows(rawDataRows).ToList();

        // Assert
        Assert.Single(result);
        Assert.Contains(result,
            r => r.Logic_PatientAddressCreateDate == new DateTime(2018, 6, 22, 16, 48, 39) &&
            r.Logic_PatientAddressUsage == "1" &&
            r.Logic_PresPhoneStatus == "A" &&
            r.Logic_PresPhoneHLevel == 1);
    }

    [Fact]
    public void DeDuplicateMedHistoryRows_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        // Arrange
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow>();

        // Act
        var result = _sut.DeDuplicateMedHistoryRows(rawDataRows).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void DeDuplicateMedHistoryRows_ShouldReturnSingleRow_WhenAllRowsAreIdentical()
    {
        // Arrange
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "Home",
                Logic_PatientAddressCreateDate = new DateTime(2023, 1, 1),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "Active",
                Logic_PresPhoneSourceCode = "Source1",
                Logic_PresPhoneHLevel = 1
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "Home",
                Logic_PatientAddressCreateDate = new DateTime(2023, 1, 1),
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "Active",
                Logic_PresPhoneSourceCode = "Source1",
                Logic_PresPhoneHLevel = 1
            }
        };

        // Act
        var result = _sut.DeDuplicateMedHistoryRows(rawDataRows).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void RankMedHistoryByPrescriptionFill_ShouldReturnRankedRows()
    {
        // Arrange
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                PrescriptionNumber = "RX001",
                FillNumber = 1,
                Logic_FillFactKey = 1
            },
            new() {
                PrescriptionNumber = "RX001",
                FillNumber = 1,
                Logic_FillFactKey = 2
            },
            new() {
                PrescriptionNumber = "RX002",
                FillNumber = 1,
                Logic_FillFactKey = 3
            },
            new() {
                PrescriptionNumber = "RX002",
                FillNumber = 2,
                Logic_FillFactKey = 4
            }
        };

        // Act
        var result = _sut.RankMedHistoryByPrescriptionFill(rawDataRows).ToList();

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(1, result[0].Logic_PrescriptionFillRank);
        Assert.Equal(1, result[1].Logic_PrescriptionFillRank);
        Assert.Equal(1, result[2].Logic_PrescriptionFillRank);
        Assert.Equal(1, result[3].Logic_PrescriptionFillRank);
    }

    [Fact]
    public void RankMedHistoryByPrescriptionFill_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        // Arrange
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow>();

        // Act
        var result = _sut.RankMedHistoryByPrescriptionFill(rawDataRows).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void RankMedHistoryByPrescriptionFill_ShouldHandleSingleRow()
    {
        // Arrange
        var rawDataRow = new SureScriptsMedicalHistoryRawDataRow
        {
            PrescriptionNumber = "RX001",
            FillNumber = 1,
            Logic_FillFactKey = 1
        };
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow> { rawDataRow };

        // Act
        var result = _sut.RankMedHistoryByPrescriptionFill(rawDataRows).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result[0].Logic_PrescriptionFillRank);
    }

    [Fact]
    public void RankMedHistoryByPrescriptionFill_ShouldHandleMultipleFills()
    {
        // Arrange
        var rawDataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                PrescriptionNumber = "RX001",
                FillNumber = 1,
                Logic_FillFactKey = 1
            },
            new() {
                PrescriptionNumber = "RX001",
                FillNumber = 2,
                Logic_FillFactKey = 2
            },
            new() {
                PrescriptionNumber = "RX001",
                FillNumber = 3,
                Logic_FillFactKey = 3
            }
        };

        // Act
        var result = _sut.RankMedHistoryByPrescriptionFill(rawDataRows).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Logic_PrescriptionFillRank);
        Assert.Equal(1, result[1].Logic_PrescriptionFillRank);
        Assert.Equal(1, result[2].Logic_PrescriptionFillRank);
    }

    [Fact]
    public void RankAndFilterMedHistoryByPatientAddress_ShouldReturnCorrectRankedRows()
    {
        // Arrange
        var dataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "1",
                Logic_PatientAddressCreateDate = new DateTime(2023, 1, 1)
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "1",
                Logic_PatientAddressCreateDate = new DateTime(2023, 1, 1)
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PdPatientKey = 1,
                Logic_PatientAddressUsage = "2",
                Logic_PatientAddressCreateDate = new DateTime(2023, 7, 1)
            },
            new() {
                Logic_FillFactKey = 2,
                Logic_PdPatientKey = 2,
                Logic_PatientAddressUsage = "2",
                Logic_PatientAddressCreateDate = new DateTime(2023, 6, 1)
            }
        };

        // Act
        var result = _sut.RankAndFilterMedHistoryByPatientAddress(dataRows).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(2, result.Where(r => r.Logic_PdPatientKey == 1 && r.Logic_PatientAddressUsage == "1" && r.Logic_PatientAddressCreateDate == new DateTime(2023, 1, 1)).Count());
        Assert.Contains(result, r => r.Logic_PdPatientKey == 2 && r.Logic_PatientAddressUsage == "2" && r.Logic_PatientAddressCreateDate == new DateTime(2023, 6, 1));
        Assert.DoesNotContain(result, r => r.Logic_PdPatientKey == 1 && r.Logic_PatientAddressUsage == "2" && r.Logic_PatientAddressCreateDate == new DateTime(2023, 7, 1));
    }

    [Fact]
    public void RankAndFilterMedHistoryByPatientAddress_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        // Arrange
        var dataRows = new List<SureScriptsMedicalHistoryRawDataRow>();

        // Act
        var result = _sut.RankAndFilterMedHistoryByPatientAddress(dataRows).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void RankAndFilterMedHistoryByPrescriberPhone_ShouldReturnEmpty_WhenInputIsEmpty()
    {
        // Arrange
        var dataRows = new List<SureScriptsMedicalHistoryRawDataRow>();

        // Act
        var result = _sut.RankAndFilterMedHistoryByPrescriberPhone(dataRows).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void RankAndFilterMedHistoryByPrescriberPhone_ShouldReturnSingleRow_WhenAllRowsAreIdentical()
    {
        // Arrange
        var dataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                Logic_FillFactKey = 1,
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "Active",
                Logic_PresPhoneSourceCode = "Source1",
                Logic_PresPhoneHLevel = 1
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "Active",
                Logic_PresPhoneSourceCode = "Source1",
                Logic_PresPhoneHLevel = 1
            }
        };

        // Act
        var result = _sut.RankAndFilterMedHistoryByPrescriberPhone(dataRows).ToList();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public void RankAndFilterMedHistoryByPrescriberPhone_ShouldReturnCorrectRankedRows()
    {
        // Arrange
        var dataRows = new List<SureScriptsMedicalHistoryRawDataRow>
        {
            new() {
                Logic_FillFactKey = 1,
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "Active",
                Logic_PresPhoneSourceCode = "Source1",
                Logic_PresPhoneHLevel = 1
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PresPhoneKey = 1,
                Logic_PresPhoneStatus = "Inactive",
                Logic_PresPhoneSourceCode = "Source2",
                Logic_PresPhoneHLevel = 2
            },
            new() {
                Logic_FillFactKey = 1,
                Logic_PresPhoneKey = 2,
                Logic_PresPhoneStatus = "Active",
                Logic_PresPhoneSourceCode = "Source1",
                Logic_PresPhoneHLevel = 1
            }
        };

        // Act
        var result = _sut.RankAndFilterMedHistoryByPrescriberPhone(dataRows).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, r => r.Logic_FillFactKey == 1 && r.Logic_PresPhoneStatus == "Active" && r.Logic_PresPhoneKey == 1);
        Assert.Contains(result, r => r.Logic_FillFactKey == 1 && r.Logic_PresPhoneStatus == "Active" && r.Logic_PresPhoneKey == 2);
    }
}
