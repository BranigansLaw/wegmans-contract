using Cocona.Application;
using INN.JobRunner.ErrorFormatter;
using INN.JobRunner.Utility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace INN.JobRunner
{
    public class CoconaRunner
    {
        public const int ApplicationInsightsFlushWaitSeconds = 5;

        private readonly ILogger<CoconaRunner> _logger;
        private readonly TelemetryClient _telemetryClient;
        private readonly IUtility _utility;
        private readonly IConfiguration _configuration;
        private readonly ICoconaConsoleProvider _coconaConsoleProvider;
        private readonly IExceptionMapper _exceptionMapper;

        public CoconaRunner(
            ICoconaConsoleProvider coconaConsoleProvider,
            IExceptionMapper exceptionMapper,
            IConfiguration configuration,
            TelemetryClient telemetryClient,
            IUtility utility,
            ILogger<CoconaRunner> logger
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
            _utility = utility ?? throw new ArgumentNullException(nameof(utility));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _coconaConsoleProvider = coconaConsoleProvider ?? throw new ArgumentNullException(nameof(coconaConsoleProvider));
            _exceptionMapper = exceptionMapper ?? throw new ArgumentNullException(nameof(exceptionMapper));
        }

        /// <summary>
        /// A wrapper function for all commands to be run through that handles logging, exception handling, and return codes
        /// </summary>
        public async ValueTask<int> TaskRunnerAsync(Func<ValueTask<int>> del, string commandName, IReadOnlyList<string> aliases, string[] args)
        {
            int returnCode = 0;
            IDictionary<string, string> telemetryProperties = new Dictionary<string, string>
            {
                { "CommandName", commandName },
                { "CommandLineArguments", string.Join(" ", args) }
            };

            int aliasCount = 1;
            foreach (string alias in aliases)
            {
                telemetryProperties.Add($"Alias-{aliasCount}", alias);
                aliasCount++;
            }

            await _coconaConsoleProvider.Output.WriteLineAsync(
                $"To view detailed logs, visit Azure Application Insights: {_configuration.GetValue<string>("AppInsightsLink")}").ConfigureAwait(false);

            using (IOperationHolder<RequestTelemetry> operationTelemetry = _telemetryClient.StartOperation<RequestTelemetry>(commandName))
            {
                // This value must always be set to true or the RequestTelemetry does not show up in ApplicationInsights
                operationTelemetry.Telemetry.Success = true;

                telemetryProperties.ToList().ForEach(operationTelemetry.Telemetry.Properties.Add);
                try
                {
                    returnCode = await del().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await _exceptionMapper.ExceptionToConsoleErrorAsync(commandName, ex);
                    operationTelemetry.Telemetry.Success = false;
                    returnCode = 1; //Set a positive return code to indicate failure due to limitations within NFM (aka TPS).
                }

                operationTelemetry.Telemetry.ResponseCode = returnCode.ToString();
            }

            await _telemetryClient.FlushAsync(CancellationToken.None).ConfigureAwait(false);
            _logger.LogInformation($"Waiting {ApplicationInsightsFlushWaitSeconds} seconds before exiting");
            await _utility.Delay(TimeSpan.FromSeconds(ApplicationInsightsFlushWaitSeconds)).ConfigureAwait(false);
            _logger.LogInformation("Flush completed");

            return returnCode;
        }
    }
}
