using INN.JobRunner.CommonParameters;
using INN.JobRunner.Exceptions;

namespace INN.JobRunner.Commands.CommandHelpers.Generic
{
    public class GenericHelperImp : IGenericHelper
    {
        /// <inheritdoc />
        public void CheckRunForDate(RunForParameter runFor, DateOnly cannotRunBeforeOrOn)
        {
            if (runFor.RunFor <= cannotRunBeforeOrOn)
            {
                throw new InvalidRunDateException(runFor.RunFor);
            }
        }

        /// <inheritdoc />
        public async Task RunFromToRunTillAsync(RunForParameter runFor, Func<DateOnly, Task> toRun)
        {
            DateOnly runTill = runFor.RunFor;
            if (runFor.RunTill.HasValue)
            {
                runTill = runFor.RunTill.Value;
            }

            for (DateOnly curr = runFor.RunFor; curr <= runTill; curr = curr.AddDays(1))
            {
                await toRun(curr);
            }
        }
    }
}
