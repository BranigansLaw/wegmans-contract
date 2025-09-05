namespace INN.JobRunner.Commands.CommandHelpers.Generic
{
    public abstract class PharmacyCommandBase
    {
        protected readonly IGenericHelper _genericHelper;
        protected readonly ICoconaContextWrapper _appContextAccessor;

        public PharmacyCommandBase(
            IGenericHelper genericHelper,
            ICoconaContextWrapper appContextAccessor
        )
        {
            _genericHelper = genericHelper ?? throw new ArgumentNullException(nameof(genericHelper));
            _appContextAccessor = appContextAccessor ?? throw new ArgumentNullException(nameof(appContextAccessor));
        }

        protected CancellationToken CancellationToken => _appContextAccessor.CancellationToken;
    }
}
