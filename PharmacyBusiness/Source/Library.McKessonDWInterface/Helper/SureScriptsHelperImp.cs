using Library.McKessonDWInterface.DataModel;
using System.Text;
using System.Text.RegularExpressions;

namespace Library.McKessonDWInterface.Helper
{
    public class SureScriptsHelperImp : ISureScriptsHelper
    {
        public string FormatString(string? text)
        {
            StringBuilder sb;
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            sb = new StringBuilder();
            foreach (char c in text)
            {
                int ascii = (int)c;
                if (ascii >= 32 && ascii <= 126 &&
                    ascii != 42 &&
                    ascii != 124 &&
                    ascii != 92 &&
                    ascii != 58 &&
                    ascii != 126)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public string LPad(string? text, int value, char valueToPadWith)
        {
            return (!string.IsNullOrEmpty(text)) ? text.PadLeft(value, valueToPadWith) : string.Empty;
        }

        /// <inheritdoc />
        public DateTime SetDefaultDate(DateTime? date)
        {
            return date ?? new DateTime(1900, 1, 1);
        }

        public string SetDefaultString(string? text)
        {
            return FormatString(text).Trim() ?? string.Empty;
        }

        public string SetDigitsOnly(string? text)
        {
            StringBuilder sb;
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            sb = new StringBuilder();
            foreach (char c in text)
            {
                if (char.IsDigit(c))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public string SetNullableValue(string? text, string defaultValue)
        {
            if (string.IsNullOrEmpty(text))
                return defaultValue;

            return text;
        }

        public decimal SetNullableValue(decimal? value, decimal defaultValue)
        {
            return value ?? defaultValue;
        }

        public long SetNullableValue(long? value, long defaultValue)
        {
            return value ?? defaultValue;
        }

        public decimal DispensedQuantityModifier(decimal? value)
        {
            return value * 1000 ?? 0;
        }

        public decimal SetRefillsRemaining(decimal? value)
        {
            if (value < 0)
                return 0;
            else if (value > 99)
                return 99;
            else
                return Math.Floor(SetNullableValue(value, 0));
        }

        /// <summary>
        /// reportState can be either PA or ALL. This method is checking if FacilityState == reportState and also reportState == "PA"
        /// </summary>
        /// <param name="state"></param>
        /// <param name="reportState"></param>
        /// <returns></returns>
        public bool IsStatePA(string? state, string reportState)
        {
            return reportState == "PA" && (state == reportState);
        }

        public string SetDefaultStringConsideringState(string? text, bool isStatePa)
        {
            return !isStatePa ? SetDefaultString(text) : string.Empty;
        }

        public string SetRouteOfAdministration(string? vaccineInformationStatementName, string? routeOfAdministrationDescription)
        {
            return (vaccineInformationStatementName == "INFLUENZA VIRUS VACCINES" && routeOfAdministrationDescription == "nasal") ? "NASAL SPRAY" : "INJECTION";
        }

        public string SetSiteOfAdministration(string? vaccineInformationStatementName, string? routeOfAdministrationDescription)
        {
            return (vaccineInformationStatementName == "INFLUENZA VIRUS VACCINES" && routeOfAdministrationDescription == "nasal") ? "NOSTRIL" : "UPPER ARM";
        }

        public string SetVaccineManufacturerName(string? drugNdc, string? drugLabeler)
        {
            string[] ndcs = ["00000000010",
                             "00000000024",
                             "00000000011",
                             "00000000012",
                             "00000000013",
                             "00000000014",
                             "00000000016",
                             "00000000017",
                             "00000000018",
                             "00000000020",
                             "00000000021",
                             "00000000025",
                             "00000000015",
                             "00000000026",
                             "00000000027",
                             "00000000036"];

            return ndcs.Contains(drugNdc) ? "SEQIRUS, INC." : SetDefaultString(drugLabeler);
        }

        public string SetVaccineName(string? drugNdc, string? drugLabelName)
        {
            string[] ndcs = ["00000000010",
                             "00000000011",
                             "00000000012",
                             "00000000013",
                             "00000000014",
                             "00000000016",
                             "00000000017",
                             "00000000018",
                             "00000000020",
                             "00000000021",
                             "00000000025",
                             "00000000015",
                             "00000000026",
                             "00000000027"];

            if (ndcs.Contains(drugNdc))
                return "AFLURIA TRI";
            else if (drugNdc == "00000000036")
                return "AFLURIA QUAD";
            else
                return SetDefaultString(drugLabelName);
        }

        public string SetVaccineCvxCode(string? cvxCode, string? vaccineInformationStatementName, string? drugLabeler, string? drugNdc)
        {
            if (!string.IsNullOrEmpty(cvxCode))
            {
                return Regex.Replace(cvxCode, "[^0-9A-Za-z]", "");
            }
            else if (vaccineInformationStatementName == "COVID-19 VACCINES" && drugLabeler is not null)
            {
                if (drugLabeler.Contains("MODERNA")) return "207";
                if (drugLabeler.Contains("PFIZER")) return "208";
                if (drugLabeler.Contains("ASTRAZENECA")) return "210";
                if (drugLabeler.Contains("JOHNSON") || drugLabeler.Contains("JANSSEN")) return "212";
            }
            else if (vaccineInformationStatementName == "INFLUENZA VIRUS VACCINES")
            {
                if (new[] { "66019030810", "66019030801" }.Contains(drugNdc)) return "149";
                if (new[] { "58160088752", "58160088741", "19515081852", "19515081841", "49281042188", "49281042150", "49281042110", "49281042158", "33332032101", "33332032102" }.Contains(drugNdc)) return "150";
                if (new[] { "49281072110", "49281072188" }.Contains(drugNdc)) return "185";
                if (new[] { "49281012165", "49281012188" }.Contains(drugNdc)) return "197";
                if (new[] { "49281063578", "49281063515", "33332042110", "33332042111" }.Contains(drugNdc)) return "158";
                if (new[] { "49281052100", "49281052125", "33332022120", "33332022121" }.Contains(drugNdc)) return "161";
                if (new[] { "70461012103", "70461012104" }.Contains(drugNdc)) return "205";
                if (new[] { "70461032103", "70461032104" }.Contains(drugNdc)) return "171";
                if (new[] { "70461042111", "70461042110" }.Contains(drugNdc)) return "186";
            }
            return string.Empty;
        }

        public SureScriptsMedicalHistoryTrailerRow GetMedHistoryTrailerRecord(int recordCount)
        {
            return new()
            {
                RecordType = "TRL",
                TotalRecords = $"{recordCount}"
            };
        }

        public SureScriptsMedicalHistoryHeaderRow GetMedHistoryHeaderRecord(DateOnly runfor)
        {
            return new()
            {
                RecordType = "HDR",
                VersionReleaseNumber = "6.2",
                SenderId = null,
                SenderParticipantPassword = null,
                ReceiverId = "S00000000000001",
                SourceName = null,
                TransmissionControlNumber = "0000001000",
                TransmissionDate = DateTime.Now.ToString("yyyyMMdd"),
                TransmissionTime = $"{DateTime.Now:HHmmss}01",
                TransmissionFileType = "PEF",
                TransmissionAction = "A",
                ExtractDate = runfor.ToString("yyyyMMdd"),
                FileType = "P"
            };
        }

        public string GetPNLTrailerRecord(int? recordCount, string delimeter)
        {
            return $"TRL{delimeter}{recordCount}";
        }

        public string GetPNLHeaderRecord(DateOnly runfor, string delimeter)
        {
            return $"HDR{delimeter}" +
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
        }

        /// <inheritdoc />
        public string FormatDateString(DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }

        /// <inheritdoc />
        public string? SetStringToMaxLength(string? input, int maxLength)
        {
            if (string.IsNullOrEmpty(input) ||
                input.Length <= maxLength)
                return input;

            return input[..maxLength];
        }

        /// <inheritdoc />
        public string? RemoveTrailingZeroFromDecimal(decimal? value)
        {
            if (value.HasValue)
                return value?.ToString("0.#");

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryRawDataRow> DeDuplicateMedHistoryRows(IEnumerable<SureScriptsMedicalHistoryRawDataRow> rawDataRows)
        {
            var allRowsWithTopRankedAddress = RankAndFilterMedHistoryByPatientAddress(rawDataRows);

            return RankAndFilterMedHistoryByPrescriberPhone(allRowsWithTopRankedAddress);
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryRawDataRow> RankAndFilterMedHistoryByPatientAddress(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows)
        {
            return dataRows
                .GroupBy(x => new
                {
                    x.Logic_FillFactKey,
                    x.Logic_PdPatientKey
                })
                .SelectMany(group =>
                {
                    var ordered = group.OrderBy(x => x.Logic_PatientAddressUsage).ThenByDescending(x => x.Logic_PatientAddressCreateDate).ToList();
                    int rank = 1;

                    return ordered.Select((x, index) =>
                    {
                        if (index > 0 &&
                        !(ordered[index - 1].Logic_PatientAddressUsage == x.Logic_PatientAddressUsage &&
                        ordered[index - 1].Logic_PatientAddressCreateDate == x.Logic_PatientAddressCreateDate))
                        {
                            rank = index + 1;
                        }
                        return new
                        {
                            x,
                            Rank = rank
                        };
                    });
                })
                .Where(x => x.Rank == 1)
                .Select(x => x.x);
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryRawDataRow> RankAndFilterMedHistoryByPrescriberPhone(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows)
        {
            return dataRows
                .GroupBy(x => new
                {
                    x.Logic_FillFactKey,
                    x.Logic_PresPhoneKey
                })
                .SelectMany(group => group
                .OrderBy(x => x.Logic_PresPhoneStatus)
                .ThenBy(x => x.Logic_PresPhoneSourceCode)
                .ThenByDescending(x => x.Logic_PresPhoneHLevel)
                .Select((x, index) => new
                {
                    x,
                    Rank = index + 1
                }))
                .Where(x => x.Rank == 1)
                .Select(x => x.x);
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryRawDataRow> RankMedHistoryByPrescriptionFill(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows)
        {
            return dataRows
                .GroupBy(x => new
                {
                    x.PrescriptionNumber,
                    x.FillNumber
                })
                .SelectMany(group =>
                {
                    var ordered = group.OrderBy(x => x.Logic_FillFactKey).ToList();
                    int rank = 1;

                    return ordered.Select((x, index) =>
                    {
                        if (index > 0 &&
                        !(ordered[index - 1].PrescriptionNumber == x.PrescriptionNumber &&
                        ordered[index - 1].FillNumber == x.FillNumber))
                        {
                            rank = index + 1;
                        }
                        x.Logic_PrescriptionFillRank = rank;
                        return x;
                    });
                });
        }

        /// <inheritdoc />
        public int DeriveCompoundIngredientSequenceNumber(string? ndc, int? rank)
        {
            if (rank.HasValue && ndc == "99999999999")
            {
                return rank.Value;
            }

            return 0;
        }
    }
}
