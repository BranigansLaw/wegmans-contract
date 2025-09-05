using Library.LibraryUtilities.Extensions;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data;
using System.Data.Common;
using ZZZZTest.TestHelpers;


namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations
{
    public class RxTransferQueryTests
    {


        /// <summary>
        /// Tests that <see cref="RxTransferQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(MappingTests))]
        public void MapFromDataReaderToTypeReturnsMappedObject((object[], RxTransferRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, RxTransferRow expectedResult) = testParameters;
            RxTransferQuery query = new RxTransferQuery
            {
                RunDate = DateOnly.FromDateTime(DateTime.Now)
            };
            DbDataReader mockedReader = Substitute.For<DbDataReader>();

            mockedReader.GetString(0).Returns(readerIndex[0]);
            mockedReader.GetString(1).Returns(readerIndex[1]);
            mockedReader.GetString(2).Returns(readerIndex[2]);
            mockedReader.GetString(3).Returns(readerIndex[3]);
            mockedReader.GetString(4).Returns(readerIndex[4]);
            mockedReader.GetString(5).Returns(readerIndex[5]);
            mockedReader.GetString(6).Returns(readerIndex[6]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 7);                       
            mockedReader.GetString(8).Returns(readerIndex[8]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 9);
            Util.SetupNullableReturn(mockedReader, readerIndex, 10);
            Util.SetupNullableReturn(mockedReader, readerIndex, 11);
            Util.SetupNullableReturn(mockedReader, readerIndex, 12);
            Util.SetupNullableReturn(mockedReader, readerIndex, 13);
            Util.SetupNullableReturn(mockedReader, readerIndex, 14);
            mockedReader.GetString(15).Returns(readerIndex[15]);
            mockedReader.GetString(16).Returns(readerIndex[16]);
            mockedReader.GetString(17).Returns(readerIndex[17]);
            mockedReader.GetString(18).Returns(readerIndex[18]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 19);
            mockedReader.GetString(20).Returns(readerIndex[20]);
            mockedReader.GetString(21).Returns(readerIndex[21]);
            mockedReader.GetString(22).Returns(readerIndex[22]);
            mockedReader.GetString(23).Returns(readerIndex[23]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 24);
            Util.SetupNullableReturn(mockedReader, readerIndex, 25);
            Util.SetupNullableReturn(mockedReader, readerIndex, 26);
            Util.SetupNullableReturn(mockedReader, readerIndex, 27);
            Util.SetupNullableReturn(mockedReader, readerIndex, 28);
            Util.SetupNullableReturn(mockedReader, readerIndex, 29);
            Util.SetupNullableReturn(mockedReader, readerIndex, 30);
            Util.SetupNullableReturn(mockedReader, readerIndex, 31);
            Util.SetupNullableReturn(mockedReader, readerIndex, 32);
            Util.SetupNullableReturn(mockedReader, readerIndex, 33);
            mockedReader.GetString(34).Returns(readerIndex[34]);
            mockedReader.GetString(35).Returns(readerIndex[35]);
            mockedReader.GetString(36).Returns(readerIndex[36]);
            mockedReader.GetString(37).Returns(readerIndex[37]);
            mockedReader.GetString(38).Returns(readerIndex[38]);
            mockedReader.GetString(39).Returns(readerIndex[39]);
            mockedReader.GetString(40).Returns(readerIndex[40]);
            mockedReader.GetString(41).Returns(readerIndex[41]);
            mockedReader.GetString(42).Returns(readerIndex[42]);
            Util.SetupNullableReturn(mockedReader, readerIndex, 43);
            mockedReader.GetString(44).Returns(readerIndex[44]);
            mockedReader.GetString(45).Returns(readerIndex[45]);

            // Act  
            // we need to mapp 45 values to the object - this is a lot of work
            // we need to find a way to do this in a more efficient way 
            RxTransferRow res = query.MapFromDataReaderToType(mockedReader, s => { });


            // Assert
           Assert.NotNull(res);
           ComparePropertiesForUnitTesting.AreAllPropertiesEqual(expectedResult, res);
        }


        public class MappingTests : TheoryData<(object[], RxTransferRow)>
        {
            public MappingTests()
            {

            var dat1 = new DateTime();

            AddRow((
            new object[] {
              "198", "SPECIALTY RX","198", "SPECIALTY RX" , "199", "HOME DELIVERY", "In-Chain",(long)3456, "60328903",  (long)123, dat1  , dat1,dat1,dat1, (long)13199, "50458057930", "XARELTO 20 MG TABLET" , "123", "NA",  (long)2,  "0",   "IN", "Electronic", "SIG", (long)166929941, (long)10310097177,(long) 1, (decimal)0.86, (decimal)0.96,(decimal)0.77, (decimal)0.66, (decimal)0.33, dat1, dat1, "DARRYL GIES", "DARRYL GIES", "2873 BROADWAY ", "SUITE 100", "CHEEKTOWAGA", "NY","14227","8009344797", "11", (long)22,"33","44"

            },
            new RxTransferRow
            {
                BaseStoreNum = "198",
                BaseStoreName = "SPECIALTY RX",
                ToStoreNum = "198",
                ToStore = "SPECIALTY RX",
                FromStoreNum = "199",
                FromStore = "HOME DELIVERY",
                TransferDest = "In-Chain",
                PatientNum = 3456,
                OrigRxNum = "60328903",
                RefillNum = 123,
                TransferDate = dat1,
                SoldDate = dat1,
                ReadyDate = dat1,
                CancelDate = dat1,
                TransferTimeKey = 13199,
                WrittenNdcWo = "50458057930",
                WrittenDrugName = "XARELTO 20 MG TABLET",
                DispNdcWo = "123",
                DispDrugName = "NA",
                QtyDispensed = 2,
                Daw = "0",
                TransType = "IN",
                TransMethod = "Electronic",
                SigText = "SIG",
                PrescriberKey = 166929941,
                RxRecordNum = 10310097177,
                RxFillSeq = 1,
                PatientPay = 0.86m,
                TpPay = 0.96m,
                TxPrice = 0.77m,
                AcqCos = 0.66m,
                UCPrice = 0.33m,
                FirstFillDate = dat1,
                LastFillDate = dat1,
                SendingRph = "DARRYL GIES",
                ReceiveRph = "DARRYL GIES",
                XferAddr = "2873 BROADWAY ",
                XferAddre = "SUITE 100",
                XferCity = "CHEEKTOWAGA",
                XferSt = "NY",
                XferZip = "14227",
                XferPhone = "8009344797",
                TransferReason = "11",
                NewRxRecordNum = 22,
                CompetitorGroup = "33",
                CompetitorStoreNum = "44"
            }
            ));



            }
        }

    }
}


