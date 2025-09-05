using INN.JobRunner.Commands.CommandHelpers.Generic;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers
{
    public abstract class PharmacyCommandBaseTests
    {
        protected readonly IGenericHelper _mockGenericHelper = Substitute.For<IGenericHelper>();
        protected readonly ICoconaContextWrapper _mockCoconaContextWrapper = Substitute.For<ICoconaContextWrapper>();

        private readonly CancellationToken _cancellationToken = new();

        public PharmacyCommandBaseTests()
        {
            _mockCoconaContextWrapper.CancellationToken.Returns(_cancellationToken);
        }

        protected CancellationToken TestCancellationToken => _cancellationToken;
    }
}
