using INN.JobRunner.CommonParameters;

namespace INN.JobRunner.Commands.CommandHelpers.Generic
{
    public interface IGenericHelper
    {
        /// <summary>
        /// Check the given the <see cref="RunForParameter"/> <paramref name="runFor"/> to see if it is valid based on the given <paramref name="cannotRunBeforeOrOn"/> date. If the
        /// check passes, the method returns without error. Otherwise, it throws an <see cref="InvalidRunDateException"/>.
        /// </summary>
        void CheckRunForDate(RunForParameter runFor, DateOnly cannotRunBeforeOrOn);

        /// <summary>
        /// Runs <paramref name="toRun"/> for the dates specified in <paramref name="runFor"/>
        /// </summary>
        Task RunFromToRunTillAsync(RunForParameter runFor, Func<DateOnly, Task> toRun);
    }
}
