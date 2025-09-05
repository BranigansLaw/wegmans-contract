using Cocona;
using INN.JobRunner.Commands.CommandHelpers.ExportImmunizationsFromMcKessonHelper;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using Library.DataFileInterface;
using Library.McKessonCPSInterface;
using Library.McKessonCPSInterface.DataModel;
using Library.McKessonDWInterface;
using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface;
using Library.TenTenInterface.DataModel;
using Library.TenTenInterface.DownloadsFromTenTen;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner.Commands
{
    public class ExportImmunizationsFromMcKesson : PharmacyCommandBase
    {
        private readonly IMcKessonDWInterface _mcKessonDWInterface;
        private readonly IMcKessonCPSInterface _mcKessonCPSInterface;
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileInterface _dataFileInterface;
        private readonly ILogger<ExportImmunizationsFromMcKesson> _logger;
        private readonly IExportImmunizationHelper _exportImmunizationHelper;

        public ExportImmunizationsFromMcKesson(
            IMcKessonDWInterface mcKessonDWInterface,
            IMcKessonCPSInterface mcKessonCPSInterface,
            ITenTenInterface tenTenInterface,
            IDataFileInterface dataFileInterface,
            ILogger<ExportImmunizationsFromMcKesson> logger,
            IExportImmunizationHelper exportImmunizationHelper,
            IGenericHelper genericHelper,
            ICoconaContextWrapper coconaContextWrapper) : base(genericHelper, coconaContextWrapper)
        {
            _mcKessonDWInterface = mcKessonDWInterface ?? throw new ArgumentNullException(nameof(mcKessonDWInterface));
            _mcKessonCPSInterface = mcKessonCPSInterface ?? throw new ArgumentNullException(nameof(mcKessonCPSInterface));
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exportImmunizationHelper = exportImmunizationHelper ?? throw new ArgumentNullException(nameof(exportImmunizationHelper));
        }

        [Command(
            "export-immunizations-from-mckesson",
            Description = "Export Immunizations From McKesson. Control-M job INN604",
            Aliases = ["INN604"]
        )]
        public async Task RunAsync(RunForParameter runFor)
        {
            _logger.LogInformation($"Starting to export Immunizations from McKesson to a data file for Run Date [{runFor.RunFor}].");

            _logger.LogDebug($"Begin download of NDC Conversion reference data from 1010data into collection .");
            TenTenDataQuery queryNdcConversion = new TenTenDataQuery(
                $"<import path=\"wegmans.devpharm.libs.data_conversions\"/><insert block=\"ndc_conversions\"/>",
                ["ndc_wo", "ndc_conversion_wo", "drug_name_conversion"],
                runFor.RunFor);
            IEnumerable<NdcConversionRow> ndcConversionRows =
                await _tenTenInterface.GetQueryResultsForTransformingToCollectionsAsync<NdcConversionRow>(queryNdcConversion, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection ndcConversionRows has: [{ndcConversionRows.Count()}] rows.");
            if (!ndcConversionRows.Any())
            {
                throw new Exception($"No rows found in the NDC Conversions from 1010data. Exiting the job.");
            }

            _logger.LogInformation($"Begin download of Immunization data from McKesson DW.");
            IEnumerable<ImmunizationRow> immunizationRows =
                await _mcKessonDWInterface.GetImmunizationRowsAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection immunizationRows has: {immunizationRows.Count()} rows.");

            _logger.LogInformation($"Begin download of Questionnaire data from McKesson CPS.");
            IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows =
                await _mcKessonCPSInterface.GetImmunizationQuestionnairesAsync(runFor.RunFor, CancellationToken).ConfigureAwait(false);
            _logger.LogDebug($"Collection immunizationQuestionnaireRows has: {immunizationQuestionnaireRows.Count()} rows.");

            _logger.LogInformation("Merge data sources into one list to be exported to CDC.");
            //See CDC requirements document for translations of data values to codes.
            foreach (var immunizationRow in immunizationRows)
            {
                //NDC Conversions
                if (ndcConversionRows.Where(r => r.NdcWo == immunizationRow.StrProductNdc).Count() > 0)
                {
                    //Note: Change Drug Name first and Drug NDC last so that the linq statement works.
                    immunizationRow.StrProductNumber = _exportImmunizationHelper.GetStoreProductNumberWhereNdcwoEqualsStrProductNdc(ndcConversionRows, immunizationRow);
                    immunizationRow.StrProductNdc = _exportImmunizationHelper.GetStoreProductNdcWhereNdcwoEqualsStrProductNdc(ndcConversionRows, immunizationRow);
                }

                //RECIP_RACE_1
                immunizationRow.RecipRaceOne = _exportImmunizationHelper.GetRaceRecipOneWherePatientNumberEqualsDecInternalPtNum(immunizationQuestionnaireRows, immunizationRow);

                //RECIP_ETHNICITY
                immunizationRow.RecipEthnicity = _exportImmunizationHelper.GetRecipEthnicityWherePatientNumberEqualsDecInternalPtNum(immunizationQuestionnaireRows, immunizationRow);

                //NY_PRIORITY_GROUP - on 8/1 STC confirmed that they want this field blank now, so we left this code and modified output record
                immunizationRow.NyPriorityGroup = _exportImmunizationHelper.GetNyPriorityGroupWherePatientNumberEqualsDecInternalPtNum(immunizationQuestionnaireRows, immunizationRow);

                //Enhancement requested by DC on 1/30/2021 to have any empty questionnaire strings set to "UNK".
                immunizationRow.RecipEthnicity = _exportImmunizationHelper.GetRecipEthnicity(immunizationRow);
                immunizationRow.RecipRaceOne = _exportImmunizationHelper.GetRecipRaceOne(immunizationRow);
                immunizationRow.PrimaryLanguage = _exportImmunizationHelper.GetPrimaryLanguage(immunizationRow);
            }


            _logger.LogInformation("Begin translatating merged data to only those data elements required by CDC.");
            var dataForExport = immunizationRows
                .Select(s => new
                {
                    STR_FIRST_NAME = s.StrFirstName,
                    STR_MIDDLE_NAME = s.StrMiddleName,
                    STR_LAST_NAME = s.StrLastName,
                    DT_BIRTH_DATE = s.DtBirthDate,
                    C_GENDER = s.CGender,
                    STR_ADDRESS_ONE = s.StrAddressOne,
                    STR_ADDRESS_TWO = s.StrAddressTwo,
                    STR_CITY = s.StrCity,
                    STR_STATE = s.StrState,
                    STR_ZIP = s.StrZip,
                    STR_PRODUCT_NAME = s.StrProductNumber,
                    STR_PRODUCT_NDC = s.StrProductNdc,
                    DEC_DISPENSED_QTY = s.DecDispensedQty,
                    C_GENERIC_IDENTIFIER = s.CGenericIndentifier,
                    STR_VERIFIED_RPH_FIRST = s.StrVerifiedRphFirst,
                    STR_VERIFIED_RPH_LAST = s.StrVerifiedRphLast,
                    STR_FACILITY_ID = s.StrFacilityId,
                    STR_RX_NUMBER = s.StrRxNumber,
                    STR_DOSAGE_FORM = s.StrDosageForm,
                    STR_STRENGTH = s.StrStrength,
                    DT_SOLD_DATE = s.DtSoldDate,
                    DT_CANCELLED_DATE = s.DtCanceledDate,
                    C_ACTION_CODE = s.CActionCode,
                    STR_DEA = s.StrDea,
                    STR_NPI = s.StrNpi,
                    DEC_INTERNALPTNUM = s.DecInternalPtNum,
                    LOT_NUMBER = s.LotNumber,
                    EXP_DATE = s.ExpDate,
                    STR_PATIENTEMAIL = s.StrPatientMail,
                    VIS_PRESENTED_DATE = s.VisPresentedDate,
                    ADMINISTRATION_SITE = s.AdministrationSite,
                    PROTECTION_INDICATOR = s.ProtectionIndicator,
                    RECIP_RACE_1 = s.RecipRaceOne,
                    RECIP_ETHNICITY = s.RecipEthnicity,
                    VFC_STATUS = s.VfcStatus,
                    NY_PRIORITY_GROUP = string.Empty,
                    PHONE_NUMBER = s.PhoneNumber,
                    PRESCRIBED_BY_ID = s.PrescribedById,
                    GENDER_IDENTITY = s.GenderIdentity,
                    PRIMARY_LANGUAGE = s.PrimaryLanguage,
                    STR_VERIFIED_RPH_TITLE = s.StrVerifiedRphTitle
                })
            .ToList();

            await _dataFileInterface.WriteListToFileAsync(
                dataForExport,
                "STCdata_" + runFor.RunFor.ToString("yyyyMMdd") + ".txt",
                true,
                "|",
                string.Empty,
                true,
                CancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Finished exporting Immunizations.");
        }
    }
}
