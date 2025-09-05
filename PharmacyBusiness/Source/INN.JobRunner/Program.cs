using Azure.Core;
using Azure.Identity;
using Cocona;
using INN.JobRunner;
using INN.JobRunner.ApplicationInsights;
using INN.JobRunner.Commands;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifyFieldConverter;
using INN.JobRunner.Commands.CommandHelpers.ExportImmunizationsFromMcKessonHelper;
using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeIqviaAndImiExportHelper;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMedicareFeedsExportHelper;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeMailSalesExportHelper;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeRxTransferExportHelper;
using INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper;
using INN.JobRunner.ErrorFormatter;
using INN.JobRunner.Extensions;
using INN.JobRunner.LibraryUtilitiesImplementations;
using INN.JobRunner.Utility;
using Library.DataFileInterface.Extensions;
using Library.EmplifiInterface.Extensions;
using Library.EmplifiInterface.Helper;
using Library.InformixInterface.Extensions;
using Library.LibraryUtilities.Extensions;
using Library.McKessonCPSInterface.Extensions;
using Library.McKessonDWInterface.Extensions;
using Library.NetezzaInterface.Extensions;
using Library.SnowflakeInterface.Extensions;
using Library.TenTenInterface.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging.Configuration;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        string archivePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileArchives");
        string exportPath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports");
        string importPath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileImports");
        string rejectPath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileRejects");
        string archiveForQaPath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\ArchiveForQA\");
        string archiveInnovationForQaPath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\ArchiveForQA\Innovation\");
        string imagePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\ImageFileImports");

        if (Directory.Exists(archivePath) == false)
            Directory.CreateDirectory(archivePath);
        if (Directory.Exists(exportPath) == false)
            Directory.CreateDirectory(exportPath);
        if (Directory.Exists(importPath) == false)
            Directory.CreateDirectory(importPath);
        if (Directory.Exists(rejectPath) == false)
            Directory.CreateDirectory(rejectPath);
        if (Directory.Exists(archiveForQaPath) == false)
            Directory.CreateDirectory(archiveForQaPath);
        if (Directory.Exists(archiveInnovationForQaPath) == false)
            Directory.CreateDirectory(archiveInnovationForQaPath);
        if (Directory.Exists(imagePath) == false)
            Directory.CreateDirectory(imagePath);

        var builder = CoconaApp.CreateBuilder(args, opts =>
        {
            opts.TreatPublicMethodsAsCommands = false;
        });

        builder.Services.AddTransient<ICoconaContextWrapper, CoconaContextWrapperImp>();
        builder.Services.AddTransient<CoconaRunner>();
        builder.Services.AddTransient<IUtility, UtilityImp>();

        builder.Services.AddSingleton<TokenCredential>(s =>
        {
            string? tenantId = s.GetRequiredService<IConfiguration>().GetValue<string>("TenantId");
            string? clientId = s.GetRequiredService<IConfiguration>().GetValue<string>("ClientId");
            string? clientSecret = s.GetRequiredService<IConfiguration>().GetValue<string>("ClientSecret");

            if (
                string.IsNullOrWhiteSpace(tenantId) ||
                string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret)
            )
            {
                return new DefaultAzureCredential();
            }

            return new ClientSecretCredential(tenantId, clientId, clientSecret);
        });

        string fileContents = string.Empty;
        using (StreamReader stream = new(File.OpenRead($"{ControlM.GetCurrentExecutableDirectory()}appsettings.json")))
        {
            fileContents = stream.ReadToEnd();
        }
        builder.Configuration.AddConfiguration(
            new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(fileContents)))
                .AddJsonFile("local.settings.json", optional: true)
                .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .Build());

        if (Uri.TryCreate(builder.Configuration.GetValue<string>("AzureVaultUrl"), UriKind.Absolute, out Uri? vaultPath))
        {
            builder.Configuration.AddAzureKeyVault(
                vaultPath,
                builder.Services.BuildServiceProvider().GetRequiredService<TokenCredential>()
            );
        }

        builder.Services.AddOptions();
        builder.Services.AddOptions<SnowflakeDataOutputDirectories>()
            .Configure<IConfiguration>((settings, configration) => 
                configration.GetSection("SnowflakeDataOutputDirectories").Bind(settings));
        builder.Services.AddOptions<ApplicationInsightsConfig>()
            .Configure<IConfiguration>((settings, configration) => 
                configration.GetSection("ApplicationInsightsSettings").Bind(settings));

        builder.Logging.AddSimpleConsole()
            .SetMinimumLevel(EnumExtensions.ParseOrReturnDefault(builder.Configuration.GetValue<string>("ConsoleLogLevel"), LogLevel.Error))
            .HideDefaultHostingLogs();

        string? aiConnectionString = builder.Configuration.GetValue<string>("ApplicationInsightsConnectionString");
        if (!string.IsNullOrEmpty(aiConnectionString))
        {
            builder.Services.AddTransient<ITelemetryInitializer, SensitivityRedactionTelemetryInitializer>();
            builder.Services.AddApplicationInsightsTelemetryProcessor<ChangeWarningExceptionsToEvents>();
            builder.Services.AddApplicationInsightsTelemetryProcessor<RemoveNoisyDependencyTelemetry>();
            builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.EnableSqlCommandTextInstrumentation = true;
            });
            builder.Services.AddApplicationInsightsTelemetryWorkerService(opts =>
            {
                opts.ConnectionString = aiConnectionString;
                opts.EnableDependencyTrackingTelemetryModule = true;
            });

            builder.Logging.AddApplicationInsights(
                configureTelemetryConfiguration: c => c.ConnectionString = aiConnectionString,
                configureApplicationInsightsLoggerOptions: o =>
                {
                    o.FlushOnDispose = true;
                }
            ).AddFilter<ApplicationInsightsLoggerProvider>(
                category: null,
                level: EnumExtensions.ParseOrReturnDefault(builder.Configuration.GetValue<string>("AppInsightsLogLevel"), LogLevel.Error)
            ).HideDefaultHostingLogsForApplicationInsights();
        }

        builder.Services.AddTransient<IExceptionMapper, ExceptionMapperImp>();

        builder.Services.AddEmplifiInterface(config =>
        {
            config.ArchiveFileLocation = archivePath;
            config.Url = builder.Configuration.GetValueOrThrowException("EmplifiURL");
            config.Username = builder.Configuration.GetValueOrThrowException("EmplifiUserName");
            config.Password = builder.Configuration.GetValueOrThrowException("EmplifiPassword");
            config.InputFileLocation = importPath;
            config.OutputFileLocation = exportPath;
            config.RejectFileLocation = rejectPath;
            config.NotificationEmailTo = builder.Configuration.GetValue<string>("EmplifiNotificationEmailTo");
            config.EmplifiDispenseNotificationEmailTo = builder.Configuration.GetValue<string>("EmplifiDispenseNotificationEmailTo");
            config.EmplifiEligibilityNotificationEmailTo = builder.Configuration.GetValue<string>("EmplifiEligibilityNotificationEmailTo");
            config.ImageFileLocation = imagePath;
            config.EmplifiTriageNotificationEmailTo = builder.Configuration.GetValue<string>("EmplifiTriageNotificationEmailTo");
        });

        builder.Services.AddDataFileInterface(config =>
        {
            config.ExecutableRootDirectory = ControlM.GetCurrentExecutableDirectory();
            config.InputFileLocation = importPath;
            config.OutputFileLocation = exportPath;
            config.ArchiveFileLocation = archivePath;
            config.RejectFileLocation = rejectPath;
            config.NotificationEmailTo = builder.Configuration.GetValue<string>("NotificationEmailTo");
            config.ImageFileLocation = imagePath;
        });

        builder.Services.AddLibraryInterface<TelemetryWrapperImp>(config =>
        {
            config.ExecutableRootDirectory = ControlM.GetCurrentExecutableDirectory();
            config.InputFileLocation = importPath;
            config.OutputFileLocation = exportPath;
            config.ArchiveFileLocation = archivePath;
            config.RejectFileLocation = rejectPath;
            config.ArchiveForQaFileLocation = archiveInnovationForQaPath;
        });

        builder.Services.AddNetezzaInterface(config =>
        {
            config.OracleDatabaseConnection = builder.Configuration.GetValueOrThrowException("NetezzaOleConnectionString");
            config.ExecutableBasePath = ControlM.GetCurrentExecutableDirectory();
        });

        builder.Services.AddTenTenDataUpload(config =>
        {
            config.Url = builder.Configuration.GetValueOrThrowException("TenTenUrl");
            config.Username = builder.Configuration.GetValueOrThrowException("TenTenUsername");
            config.Password = builder.Configuration.GetValueOrThrowException("TenTenPassword");
            config.ExecutableBasePath = ControlM.GetCurrentExecutableDirectory();
            config.TenTenFolderPath = builder.Configuration.GetValueOrThrowException("TenTenTablePath");
            config.InputFileLocation = importPath;
            config.OutputFileLocation = exportPath;
            config.ArchiveFileLocation = archivePath;
            config.RejectFileLocation = rejectPath;
            config.OverrideTenTenFullTablePath = builder.Configuration.GetValue<bool?>("OverrideTenTenFullTablePath") ?? throw new ArgumentNullException("OverrideTenTenFullTablePath");
            config.SamOwnerId = builder.Configuration.GetValueOrThrowException("TenTenSamOwnerId");
            config.SamGroupId = builder.Configuration.GetValueOrThrowException("TenTenSamGroudId");
            config.SamPassword = builder.Configuration.GetValueOrThrowException("TenTenSamPassword");
            config.AzureBlobConnectionString = builder.Configuration.GetValueOrThrowException("TenTenAzureBlobConnectionString");
            config.AzureBlobEnvironmentFolderName = builder.Configuration.GetValueOrThrowException("TenTenAzureBlobEnvironmentFolderName");
            config.ParquetUploadNotificationEmailTo = builder.Configuration.GetValueOrThrowException("ParquetUploadNotificationEmailTo");
            config.DeveloperMode = builder.Configuration.GetValue<bool?>("TenTenAzureBlobUploadDeveloperMode");
        }, getTenTenAzureConnectionString: () => builder.Configuration.GetValueOrThrowException("TenTenAzureBlobConnectionString"));

        builder.Services.AddInformixInterface(config =>
        {
            config.Username = builder.Configuration.GetValueOrThrowException("InformixUsername");
            config.Password = builder.Configuration.GetValueOrThrowException("InformixPassword");
        });

        builder.Services.AddMcKessonCPSInterface(config =>
        {
            config.SqlServerDatabaseConnection = builder.Configuration.GetValueOrThrowException("McKessonCPSConnectionString");
            config.DataFileArchivePath = archivePath;
            config.DataFileExportPath = exportPath;
            config.DataFileImportPath = importPath;
        });

        builder.Services.AddMcKessonDWInterface(config =>
        {
            config.TnsDescriptor = builder.Configuration.GetValueOrThrowException("McKessonDWTnsDescriptor");
            config.Username = builder.Configuration.GetValueOrThrowException("McKessonDWUserName");
            config.Password = builder.Configuration.GetValueOrThrowException("McKessonDWPassword");
            config.DataFileArchivePath = archivePath;
            config.DataFileExportPath = exportPath;
            config.DataFileImportPath = importPath;
        });

        builder.Services.AddSnowflakeInterface(config =>
        {
            config.User = builder.Configuration.GetValueOrThrowException("SnowflakeUser");
            config.Account = builder.Configuration.GetValueOrThrowException("SnowflakeAccount");
            config.Role = builder.Configuration.GetValueOrThrowException("SnowflakeRole");
            config.Warehouse = builder.Configuration.GetValueOrThrowException("SnowflakeWarehouse");
            config.SnowflakeDWDatabase = builder.Configuration.GetValueOrThrowException("SnowflakeDWDatabase");
            config.SnowflakeAUDDatabase = builder.Configuration.GetValueOrThrowException("SnowflakeAUDDatabase");
            config.Scopes = builder.Configuration.GetSection("SnowflakeScopes").Get<string[]>() ?? throw new ArgumentNullException("SnowflakeScopes");
        });

        SetupCommandHelpers(builder.Services);

        var app = builder.Build();

        CoconaRunner runner = app.Services.GetRequiredService<CoconaRunner>();
        app.UseFilter((ctx, del) => runner.TaskRunnerAsync(() => del(ctx), ctx.Command.Name, ctx.Command.Aliases, args));

        app.AddCommands<IntegrationTests>();
        app.AddCommands<TenTenImportNetSalesFromNetezza>();
        app.AddCommands<TenTenImportCallStateDetailFromCisco>();
        app.AddCommands<TenTenImportCallRecordDetailFromCisco>();
        app.AddCommands<TenTenImportNewTagPatientGroupsFromMcKessonDW>();
        app.AddCommands<TenTenImportRxErpFromMcKessonDW>();
        app.AddCommands<TenTenImportSoldDetailFromMcKessonDW>();
        app.AddCommands<TenTenImportConversationFactFromMcKessonCPS>();
        app.AddCommands<TenTenImportEnrollmentFactFromMcKessonCPS>();
        app.AddCommands<TenTenImportMeasureFactFromMcKessonCPS>();
        app.AddCommands<TenTenImportMessageFactFromMcKessonCPS>();
        app.AddCommands<TenTenImportPCSPrefFromMcKessonCPS>();
        app.AddCommands<TenTenImportPCSPrefFullFromMcKessonCPS>();
        app.AddCommands<TenTenImportQuestionnaireFactFromMcKessonCPS>();
        app.AddCommands<TenTenImportStoreInventoryHistoryFromMcKessonDW>();
        app.AddCommands<ExportOmnisysClaimFromMcKessonDW>();
        app.AddCommands<TenTenImportIvrOutboundCallsFromOmnicellFile>();
        app.AddCommands<TenTenImportAdm340BEligibleClaimsFromTCGFile>();
        app.AddCommands<TenTenImportAdm340BInvoicingFromTCGFile>();
        app.AddCommands<TenTenImportAdm340BOpportunityFromTCGFile>();
        app.AddCommands<TenTenImportAdm340BPurchasesFromTCGFile>();
        app.AddCommands<TenTenImportVaccineAppointmentBookingFromOmnicellFile>();
        app.AddCommands<ExportTurnaroundTimesFromMcKessonDW>();
        app.AddCommands<ExportDelayAndDenialStatusFromEmplifi>();
        app.AddCommands<ExportSureScriptsReportsFromMcKessonDW>();
        app.AddCommands<TenTenExportAdm340BOrderToTCGFile>();
        app.AddCommands<ScriptsWorkloadBalancing>();
        app.AddCommands<ThirdPartyClaims>();
        app.AddCommands<SnowflakeOmnisysClaimExport>();
        app.AddCommands<SnowflakeSelectSoldDetailExport>();
        app.AddCommands<SnowflakeDeceasedExport>();
        app.AddCommands<ExportImmunizationsFromMcKesson>();
        app.AddCommands<ExportAstuteAdherenceCallsFromMcKesson>();
        app.AddCommands<SnowflakeGetSmartOrderPointsMinMaxExport>();
        app.AddCommands<DeveloperToolsGenerateSnowflakeMappings>();
        app.AddCommands<EmplifiImportJpapTriageFromIbm>();
        app.AddCommands<TenTenImportMedicareAutoshipFromEnlivenHealth>();
        app.AddCommands<EmplifiImportOncologyTriageFromIbm>();
        app.AddCommands<TenTenImportProjectionDetail>();
        app.AddCommands<EmplifiImportJpapEligibilityFromIbm>();
        app.AddCommands<SnowflakeToTenTenDataSftpExport>();
        app.AddCommands<SnowflakeMedicareFeedsExport>();
        app.AddCommands<SnowflakeMailSalesExport>();
        app.AddCommands<SnowflakeAlternativePaymentsExport>();
        app.AddCommands<SnowflakeAtebMasterExport>();
        app.AddCommands<SnowflakeCreditCardPaymentsExport>();
        app.AddCommands<SnowflakeDailyDrugExport>();
        app.AddCommands<SnowflakeErxUnauthorizedExport>();
        app.AddCommands<SnowflakeGetClaimsExport>();
        app.AddCommands<SnowflakeGetEnterpriseRxDataByPatientNumExport>();
        app.AddCommands<SnowflakeGetPOAuditExport>();
        app.AddCommands<SnowflakeGetPOFactExport>();
        app.AddCommands<SnowflakeMcKessonNaloxoneExport>();
        app.AddCommands<SnowflakePatientAddressesExport>();
        app.AddCommands<SnowflakePatientsExport>();
        app.AddCommands<SnowflakeSuperDuperClaimExport>();
        app.AddCommands<SnowflakeSuperWorkflowExport>();
        app.AddCommands<SnowflakeVerifonePaymentsExport>();
        app.AddCommands<SnowflakeWorkersCompMonthlyExport>();
        app.AddCommands<SnowflakeIqviaAndImiExport>();
        app.AddCommands<JanssenImportOncologyVoucherTriageFromIbm>();
        app.AddCommands<EmplifiImportVerificationOfBenefitsFromIbm>();
        app.AddCommands<ExportJpapStatusFromEmplifi>();

        app.Run(); 
    }

    /// <summary>
    /// Setup all the command helpers in INN.JobRunner.Commands.CommandHelpers
    /// </summary>
    /// <param name="services"></param>
    private static void SetupCommandHelpers(IServiceCollection services)
    {
        services.AddTransient<IGenericHelper, GenericHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportNetSalesFromNetezzaHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportNetSalesFromNetezzaHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportCallStateDetailFromCiscoHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportCallStateDetailFromCiscoHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportNewTagPatientGroupsFromMcKessonDWHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportNewTagPatientGroupsFromMcKessonDWHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportRxErpFromMcKessonDWHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportRxErpFromMcKessonDWHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportSoldDetailFromMcKessonDWHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportSoldDetailFromMcKessonDWHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportConversationFactFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportConversationFactFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportEnrollmentFactFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportEnrollmentFactFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportMeasureFactFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportMeasureFactFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportMessageFactFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportMessageFactFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFullFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportPCSPrefFullFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportQuestionnaireFactFromMcKessonCPSHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportQuestionnaireFactFromMcKessonCPSHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportStoreInventoryHistoryFromMcKessonDWHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportStoreInventoryHistoryFromMcKessonDWHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportIvrOutboundCallsFromOmnicellFileHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportIvrOutboundCallsFromOmnicellFileHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BEligibleClaimsFromTCGFileHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BEligibleClaimsFromTCGFileHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BInvoicingFromTCGFileHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BInvoicingFromTCGFileHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BOpportunityFromTCGFileHelper.MapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BPurchasesFromTCGFileHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportAdm340BPurchasesFromTCGFileHelper.MapperImp>();
        services.AddTransient<IEmplifyFieldConverter, EmplifyFieldConverterImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.HelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifiMapping.IEmplifiMapper,
            INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifiMapping.EmplifiMapperImp>();
        services.AddTransient<IJpapTriageHelper, JpapTriageHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapTriageFromIbmHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapTriageFromIbmHelper.HelperImp>();
        services.AddTransient<IExportImmunizationHelper, ExportImmunizationHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportMedicareAutoshipFromEnlivenHealthHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportMedicareAutoshipFromEnlivenHealthHelper.MapperImp>();
        services.AddTransient<IOncologyTriageHelper, OncologyTriageHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportOncologyTriageFromIbmHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportOncologyTriageFromIbmHelper.HelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.TenTenImportProjectionDetailHelper.MapperImp>();
        services.AddTransient<IVerificationOfBenefitsHelper, VerificationOfBenefitsHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper.HelperImp>();
        services.AddTransient<IJpapEligibilityHelper, JpapEligibilityHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapEligibilityFromIbmHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapEligibilityFromIbmHelper.HelperImp>();
        services.AddTransient<IAstuteAdherenceDispenseHelper, AstuteAdherenceHelperImp>();
        services.AddTransient<ISnowflakeToTenTenDataSftpExportHelper, SnowflakeToTenTenDataSftpExportHelperImp>();
        services.AddTransient<ISnowflakeMedicareFeedsExportHelper, SnowflakeMedicareFeedsExportHelperImp>();
        services.AddTransient<IOncologyVoucherTriageHelper, OncologyVoucherTriageHelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.JanssenImportOncologyVoucherTriageFromIbmHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.JanssenImportOncologyVoucherTriageFromIbmHelper.HelperImp>();
        services.AddTransient<ISnowflakeIqviaAndImiExportHelper, SnowflakeIqviaAndImiExportHelperImp>();
        services.AddTransient<ISnowflakeMailSalesExportHelper, SnowflakeMailSalesExportHelperImp>();
        services.AddTransient<IRxTransferRowTransformations, RxTransferRowTransformationsImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper.HelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiFieldConverter.IEmplifiFieldConverter,
            INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiFieldConverter.EmplifiFieldConverterImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.IHelper,
            INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.HelperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiMapping.IEmplifiMapper,
            INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiMapping.EmplifiMapperImp>();
        services.AddTransient<
            INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper.IMapper,
            INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper.MapperImp>();
    }
}