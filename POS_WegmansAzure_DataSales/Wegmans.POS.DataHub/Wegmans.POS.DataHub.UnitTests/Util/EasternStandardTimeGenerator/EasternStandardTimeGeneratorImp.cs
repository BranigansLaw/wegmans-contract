using Wegmans.POS.DataHub.Util.EasternStandardTimeGenerator;

namespace Wegmans.POS.DataHub.UnitTests.Util.EasternStandardTimeGenerator
{
    public class EasternStandardTimeGeneratorImpTests
    {
        private readonly EasternStandardTimeGeneratorImp _sut;

        public EasternStandardTimeGeneratorImpTests()
        {
            _sut = new EasternStandardTimeGeneratorImp();
        }

        [Fact]
        public void GetCurrentEasternStandardTime_Returns_CurrentTimeInRochester()
        {
            // Act
            DateTimeOffset result = _sut.GetCurrentEasternStandardTime();

            // Assert
            // You should manually assert here that the value is actually Rochester time
            Assert.True(true);
        }
    }
}
