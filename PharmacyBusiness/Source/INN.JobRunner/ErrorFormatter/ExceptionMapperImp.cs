using Cocona.Application;

namespace INN.JobRunner.ErrorFormatter
{
    public class ExceptionMapperImp : IExceptionMapper
    {
        private readonly ICoconaConsoleProvider _coconaConsoleProvider;

        /// <summary>
        /// This dictionary contains the mappings from the command name (taken from the <see cref="CommandAttribute"/> for the command) to a dictionary of exception types that
        /// map to human-readable error messages.
        /// 
        /// Mapping the the generic <see cref="Exception"/> type will mean that if no other exception type is matched, then the generic <see cref="Exception"/> message will be used.
        /// </summary>
        private static readonly IDictionary<string, IDictionary<Type, string>> CommandExceptionToErrorMessageMap = new Dictionary<string, IDictionary<Type, string>>
        {
            {
                "tenten-import-net-sales-from-netezza",
                new Dictionary<Type, string>
                {
                    {
                        typeof(Exception),
                        "An error occurred during the running of this job. If this is the first attempted run of this task, please try running again. This job can be run " +
                        "several times without creating issues."
                    }
                }
            },
            {
                "integration-tests",
                new Dictionary<Type, string>
                {
                    {
                        typeof(DivideByZeroException),
                        "This is a test message that occurs when a DivideByZeroException is thrown. It is logged here for testing purposes."
                    },
                    {
                        typeof(Exception),
                        "This is the generic catch for an Exception."
                    }
                }
            },
        };

        public ExceptionMapperImp(ICoconaConsoleProvider coconaConsoleProvider)
        {
            _coconaConsoleProvider = coconaConsoleProvider ?? throw new ArgumentNullException(nameof(coconaConsoleProvider));
        }

        /// <inheritdoc />
        public async Task ExceptionToConsoleErrorAsync(string jobName, Exception ex)
        {
            if (CommandExceptionToErrorMessageMap.ContainsKey(jobName))
            {
                IDictionary<Type, string> commandDictionary = CommandExceptionToErrorMessageMap[jobName];

                if (commandDictionary.ContainsKey(ex.GetType()))
                {
                    await _coconaConsoleProvider.Error.WriteLineAsync(commandDictionary[ex.GetType()]);
                }
                else if (commandDictionary.ContainsKey(typeof(Exception)))
                {
                    await _coconaConsoleProvider.Error.WriteLineAsync(commandDictionary[typeof(Exception)]);
                }
            }
        }
    }
}
