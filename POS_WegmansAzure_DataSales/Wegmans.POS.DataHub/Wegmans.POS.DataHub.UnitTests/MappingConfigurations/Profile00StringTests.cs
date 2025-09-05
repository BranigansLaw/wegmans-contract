using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.MappingConfigurations;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class Profile00StringTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord00"/> to <see cref="Transaction"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(Profile00StringTestsCases))]
        public void Mapping_TransactionRecord00_To_Transaction_MapsAllFields(TransactionRecord00 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<Profile00String>());
            IMapper mapper = config.CreateMapper();

            // Act
            Transaction res = mapper.Map<TransactionRecord00, Transaction>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TerminalType", "TransactionType", "TransactionTypeId", "Reserved1", "TOF_Recovered", "TransactionCopiedAfterClose", "CVBPaper", "Reserved2", "SelfCheckoutTerminal", "DigitalReceiptCreated", "PaperReceiptSuppressed", "EatInTransaction", "PreferredCustomer", "GrossPositiveIsNegative", "GrossNegativeIsNegative", "AdditionalRecordsExist", "NotFirstRecord", "TillChangeSignoffRecord", "TermInitialized", "RollbackPriceItem", "Reserved3", "TransactionTransferred", "TransactionMonitored", "TenderRemoval", "TillContentsReportBeforeThisTransaction", "TillExchangedBeforeThisTransaction", "TenderVerificationBeforeTransaction", "NewPasswordUsed", "OperatorSignonPriorToTransaction", "TenderRejectedinTransaction", "SignoffIsFalse", "DataEntry", "OpenDrawer", "SpecialSignoff", "TerminalAccountability", "GrossPositive", "GrossNegative", "RingSeconds", "TenderSeconds", "InactiveSeconds", "SpecialTime", "AltPrice", "VoidTrc" });

            Assert.Equal(from.TerminalId.ConvertToTerminalType(), res.TerminalType);
            Assert.Equal(res.TransactionType, from.TransactionTypeEnum);
            Assert.Equal(int.Parse(from.TransactionType), res.TransactionTypeId);
            Assert.Equal(from.Indicat1.Bit1.ToNullableBoolean(), res.TOF_Recovered);
            Assert.Equal(from.Indicat1.Bit2.ToNullableBoolean(), res.TransactionCopiedAfterClose);
            Assert.Equal(from.Indicat1.Bit3.ToNullableBoolean(), res.CVBPaper);
            Assert.Equal(from.Indicat1.Bit4.ToNullableBoolean(), res.Reserved2);
            Assert.Equal(from.Indicat1.Bit5.ToNullableBoolean(), res.SelfCheckoutTerminal);
            Assert.Equal(from.Indicat1.Bit6.ToNullableBoolean(), res.DigitalReceiptCreated);
            Assert.Equal(from.Indicat1.Bit7.ToNullableBoolean(), res.PaperReceiptSuppressed);
            Assert.Equal(from.Indicat1.Bit8.ToNullableBoolean(), res.EatInTransaction);
            Assert.Equal(from.Indicat1.Bit9.ToNullableBoolean(), res.PreferredCustomer);
            Assert.Equal(from.Indicat1.Bit10.ToNullableBoolean(), res.GrossPositiveIsNegative);
            Assert.Equal(from.Indicat1.Bit11.ToNullableBoolean(), res.GrossNegativeIsNegative);
            Assert.Equal(from.Indicat1.Bit12.ToNullableBoolean(), res.AdditionalRecordsExist);
            Assert.Equal(from.Indicat1.Bit13.ToNullableBoolean(), res.NotFirstRecord);
            Assert.Equal(from.Indicat1.Bit14.ToNullableBoolean(), res.TillChangeSignoffRecord);
            Assert.Equal(from.Indicat1.Bit15.ToNullableBoolean(), res.TermInitialized);
            Assert.Equal(from.Indicat1.Bit16.ToNullableBoolean(), res.RollbackPriceItem);
            Assert.Equal(from.Indicat1.Bit17.ToNullableBoolean(), res.Reserved3);
            Assert.Equal(from.Indicat1.Bit18.ToNullableBoolean(), res.TransactionTransferred);
            Assert.Equal(from.Indicat1.Bit19.ToNullableBoolean(), res.TransactionMonitored);
            Assert.Equal(from.Indicat1.Bit20.ToNullableBoolean(), res.TenderRemoval);
            Assert.Equal(from.Indicat1.Bit21.ToNullableBoolean(), res.TillContentsReportBeforeThisTransaction);
            Assert.Equal(from.Indicat1.Bit22.ToNullableBoolean(), res.TillExchangedBeforeThisTransaction);
            Assert.Equal(from.Indicat1.Bit23.ToNullableBoolean(), res.TenderVerificationBeforeTransaction);
            Assert.Equal(from.Indicat1.Bit24.ToNullableBoolean(), res.NewPasswordUsed);
            Assert.Equal(from.Indicat1.Bit25.ToNullableBoolean(), res.OperatorSignonPriorToTransaction);
            Assert.Equal(from.Indicat1.Bit26.ToNullableBoolean(), res.TenderRejectedinTransaction);
            Assert.Equal(from.Indicat1.Bit27.ToNullableBoolean(), res.SignoffIsFalse);
            Assert.Equal(from.Indicat1.Bit28.ToNullableBoolean(), res.DataEntry);
            Assert.Equal(from.Indicat1.Bit29.ToNullableBoolean(), res.OpenDrawer);
            Assert.Equal(from.Indicat1.Bit30.ToNullableBoolean(), res.SpecialSignoff);
            Assert.Equal(from.Indicat1.Bit31.ToNullableBoolean(), res.TerminalAccountability);
            Assert.Equal(from.GrossPositive.Divide(), res.GrossPositive);
            Assert.Equal(from.GrossNegative.Divide(), res.GrossNegative);
            Assert.Equal(from.RingSeconds.getConvertedSeconds()?.ToString(), res.RingSeconds);
            Assert.Equal(from.TenderSeconds.getConvertedSeconds()?.ToString(), res.TenderSeconds);
            Assert.Equal(from.InactiveSeconds.getConvertedSeconds()?.ToString(), res.InactiveSeconds);
            Assert.Equal(from.SpecialTime.getConvertedSeconds()?.ToString(), res.SpecialTime);
            Assert.Equal(from.AltPrice00Enum.ToString(), res.AltPrice);
            Assert.Equal(from.VoidTransaction00Enum?.ToString(), res.VoidTrc);
        }

        public class Profile00StringTestsCases : TheoryData<TransactionRecord00>
        {
            public Profile00StringTestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord00
                {
                    StringType = 1,
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    DateTimeString = "2407261015",
                    TransactionType = "2",
                    NumberOfStrings = "ABC123",
                    Operator = "ABC123",
                    Password = "ABC123",
                    GrossPositive = 1.23M,
                    GrossNegative = 1.23M,
                    RingSeconds = "45",
                    TenderSeconds = "15",
                    SpecialTime = "26",
                    InactiveSeconds = "41",
                    Indicat1 = new Indicat1(),
                    AltPrice = "1",
                    VoidTrc = "2",
                    UserValue = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord00 t in Utility.GetTransactionDataFromXml<TransactionRecord00>("00"))
                {
                    AddRow(t);
                }
            }
        }
    }
}