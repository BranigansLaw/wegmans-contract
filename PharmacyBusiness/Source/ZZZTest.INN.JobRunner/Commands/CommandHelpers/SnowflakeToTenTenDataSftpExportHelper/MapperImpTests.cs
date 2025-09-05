using INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper;
using Library.SnowflakeInterface.Data;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper
{
    public class MapperImpTests
    {
        private readonly MapperImp _sut;

        public MapperImpTests()
        {
            _sut = new MapperImp();
        }

        /// <summary>
        /// Tests that <see cref="MapperImp.MapFromDurConflictRow(IEnumerable{DurConflictRow})"/> maps all items passed to it from <see cref="DurConflictRow"/> to <see cref="DurConflictTenTenRow"/>
        /// </summary>
        [Fact]
        public void MapFromDurConflictRow_Returns_InputMappedToNewClass()
        {
            // Arrange
            DurConflictRow input = new()
            {
                ConflictCode = "sklgjdlkgfjg",
                ConflictDesc = "dflkgjdgjfd",
                ConflictType = "dklfgjdflkgjfdg",
                DeaClass = "fskjgsdgdfg",
                DrugName = "slkgjdlkgjdfg",
                DurDate = new DateTime(2025, 1, 28),
                Gcn = "sflgkdfljkgfd",
                GcnSequenceNumber = "38298423",
                IsCritical = "db8df90",
                IsException = "0d98gfgf8",
                LevelOfEffort = 903458530986,
                NdcWo = "d90fg8dgf908df",
                PartSeqNumber = 49308034856,
                PatientNumber = 90383408534,
                PrescriberKey = 9583098436,
                ProfService = "9dgdfg80df89gf",
                ReasonForService = "89dg09df8g890g",
                RefillNumber = 34095834,
                ResultOfService = "d90f8d0fg",
                RxFillSequence = 9504358456,
                RxNumber = "d90fg8df908dfg",
                RxRecordNumber = 094386085645,
                Sdgi = "d9f0g8df08gg",
                SeverityDesc = "d8g9d8fg789dfg",
                StoreNumber = "d9g79d8fg7dg",
                UserKey = 409854098654
            };

            // Act
            IEnumerable<DurConflictTenTenRow> res = _sut.MapFromDurConflictRow([input]);

            // Assert
            DurConflictTenTenRow resultObj = Assert.Single(res);
            Assert.Equal(input.ConflictCode, resultObj.ConflictCode);
            Assert.Equal(input.ConflictDesc, resultObj.ConflictDesc);
            Assert.Equal(input.ConflictType, resultObj.ConflictType);
            Assert.Equal(input.DeaClass, resultObj.DeaClass);
            Assert.Equal(input.DrugName, resultObj.DrugName);
            Assert.Equal(input.DurDate, resultObj.DurDate);
            Assert.Equal(input.Gcn, resultObj.Gcn);
            Assert.Equal(input.GcnSequenceNumber, resultObj.GcnSequenceNumber);
            Assert.Equal(input.IsCritical, resultObj.IsCritical);
            Assert.Equal(input.IsException, resultObj.IsException);
            Assert.Equal(input.LevelOfEffort, resultObj.LevelOfEffort);
            Assert.Equal(input.NdcWo, resultObj.NdcWo);
            Assert.Equal(input.PartSeqNumber, resultObj.PartSeqNumber);
            Assert.Equal(input.PatientNumber, resultObj.PatientNumber);
            Assert.Equal(input.PrescriberKey, resultObj.PrescriberKey);
            Assert.Equal(input.ProfService, resultObj.ProfService);
            Assert.Equal(input.ReasonForService, resultObj.ReasonForService);
            Assert.Equal(input.RefillNumber, resultObj.RefillNumber);
            Assert.Equal(input.ResultOfService, resultObj.ResultOfService);
            Assert.Equal(input.RxFillSequence, resultObj.RxFillSequence);
            Assert.Equal(input.RxNumber, resultObj.RxNumber);
            Assert.Equal(input.RxRecordNumber, resultObj.RxRecordNumber);
            Assert.Equal(input.Sdgi, resultObj.Sdgi);
            Assert.Equal(input.SeverityDesc, resultObj.SeverityDesc);
            Assert.Equal(input.StoreNumber, resultObj.StoreNumber);
            Assert.Equal(input.UserKey, resultObj.UserKey);
        }
    }
}
