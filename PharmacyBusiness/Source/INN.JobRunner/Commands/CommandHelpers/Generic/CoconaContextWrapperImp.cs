using Cocona.Application;

namespace INN.JobRunner.Commands.CommandHelpers.Generic
{
    internal class CoconaContextWrapperImp : ICoconaContextWrapper
    {
        private readonly ICoconaAppContextAccessor _coconaAppContextAccessor;

        public CoconaContextWrapperImp(
            ICoconaAppContextAccessor coconaAppContextAccessor
        )
        {
            _coconaAppContextAccessor = coconaAppContextAccessor ?? throw new ArgumentNullException(nameof(coconaAppContextAccessor));
        }

        /// <inheritdoc />
        public CancellationToken CancellationToken
        {
            get
            {

                if (_coconaAppContextAccessor.Current == null)
                {
                    throw new InvalidOperationException(nameof(ICoconaAppContextAccessor.Current));
                }

                return _coconaAppContextAccessor.Current.CancellationToken;
            }
        }
    }
}
