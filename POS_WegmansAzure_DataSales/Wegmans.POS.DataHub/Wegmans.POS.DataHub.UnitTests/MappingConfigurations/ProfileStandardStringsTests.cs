using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.POS.DataHub.MappingConfigurations;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class ProfileStandardStringsTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord03"/> to <see cref="Discount"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord03TestsCases))]
        public void Mapping_TransactionRecord03_To_Discount_MapsAllFields(TransactionRecord03 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            Discount res = mapper.Map<TransactionRecord03, Discount>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "AssociatedCouponCode", "Amount" });

            Assert.Equal(from.GroupId.convertDiscountGroupToCouponCode(), res.AssociatedCouponCode);
            Assert.Equal(from.Amount.Divide(), res.Amount);
        }

        public class TransactionRecord03TestsCases : TheoryData<TransactionRecord03>
        {
            public TransactionRecord03TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord03()
                {
                    StringType = 1,
                    GroupId = "ABC123",
                    Percent = 1.23M,
                    Amount = 1.23M
                });

                // Read from file
                foreach (TransactionRecord03 t in Utility.GetTransactionDataFromXml<TransactionRecord03>("03"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord04"/> to <see cref="Discount"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord04TestsCases))]
        public void Mapping_TransactionRecord04_To_Discount_MapsAllFields(TransactionRecord04 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            Discount res = mapper.Map<TransactionRecord04, Discount>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "AssociatedCouponCode", "Amount" });

            Assert.Equal(from.GroupId.convertDiscountGroupToCouponCode(), res.AssociatedCouponCode);
            Assert.Equal(from.Amount.Divide().Negate(), res.Amount);
        }

        public class TransactionRecord04TestsCases : TheoryData<TransactionRecord04>
        {
            public TransactionRecord04TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord04()
                {
                    StringType = 1,
                    GroupId = "ABC123",
                    Percent = 1.23M,
                    Amount = 1.23M
                });

                // Read from file
                foreach (TransactionRecord04 t in Utility.GetTransactionDataFromXml<TransactionRecord04>("04"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord05"/> to <see cref="TenderExchange"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord05TestsCases))]
        public void Mapping_TransactionRecord05_To_TenderExchange_MapsAllFields(TransactionRecord05 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            TenderExchange res = mapper.Map<TransactionRecord05, TenderExchange>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TenderType", "TenderAmount", "TenderCashingFee" });

            Assert.Equal(from.TenderType.getTenderTypeName().ToString(), res.TenderType);
            Assert.Equal(from.TenderAmount.Divide() ?? 0, res.TenderAmount);
            Assert.Equal(from.TenderCashingFee.Divide(), res.TenderCashingFee);
        }

        public class TransactionRecord05TestsCases : TheoryData<TransactionRecord05>
        {
            public TransactionRecord05TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord05()
                {
                    StringType = 1,
                    TenderEntrySequenceNumber = 1,
                    TenderType = "ABC123",
                    TenderAmount = 1.23M,
                    TenderCashingFee = 1.23M,
                    CustomerAccount = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord05 t in Utility.GetTransactionDataFromXml<TransactionRecord05>("05"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord06"/> to <see cref="TenderVoid"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord06TestsCases))]
        public void Mapping_TransactionRecord06_To_TenderVoid_MapsAllFields(TransactionRecord06 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            TenderVoid res = mapper.Map<TransactionRecord06, TenderVoid>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TenderType", "TenderAmount", "TenderCashingFee" });

            Assert.Equal(from.TenderType.getTenderTypeName().ToString(), res.TenderType);
            Assert.Equal(from.TenderAmount.Divide(), res.TenderAmount);
            Assert.Equal(from.TenderCashingFee.Divide(), res.TenderCashingFee);
        }

        public class TransactionRecord06TestsCases : TheoryData<TransactionRecord06>
        {
            public TransactionRecord06TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord06()
                {
                    StringType = 1,
                    TenderEntrySequenceNumber = 1,
                    TenderType = "ABC123",
                    TenderAmount = 1.23M,
                    TenderCashingFee = 1.23M,
                    LoyaltyNumber = "ABC123",
                    Status = 1
                });

                // Read from file
                foreach (TransactionRecord06 t in Utility.GetTransactionDataFromXml<TransactionRecord06>("06"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord07"/> to <see cref="TaxData"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord07TestsCases))]
        public void Mapping_TransactionRecord07_To_TaxData_MapsAllFields(TransactionRecord07 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            TaxData res = mapper.Map<TransactionRecord07, TaxData>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TaxAAmount", "TaxBAmount", "TaxCAmount", "TaxASales", "TaxBSales", "TaxCSales", "TaxDSales", "TaxDAmount", "TaxESales", "TaxEAmount", "TaxFSales", "TaxFAmount", "TaxGSales", "TaxGAmount", "TaxHSales", "TaxHAmount", "FinalAmount", "SubTotalAmount" });

            Assert.Equal(from.TaxDSales.Divide(), res.TaxDSales);
            Assert.Equal(from.TaxDAmount.Divide(), res.TaxDAmount);
            Assert.Equal(from.TaxESales.Divide(), res.TaxESales);
            Assert.Equal(from.TaxEAmount.Divide(), res.TaxEAmount);
            Assert.Equal(from.TaxFSales.Divide(), res.TaxFSales);
            Assert.Equal(from.TaxFAmount.Divide(), res.TaxFAmount);
            Assert.Equal(from.TaxGSales.Divide(), res.TaxGSales);
            Assert.Equal(from.TaxGAmount.Divide(), res.TaxGAmount);
            Assert.Equal(from.TaxHSales.Divide(), res.TaxHSales);
            Assert.Equal(from.TaxHAmount.Divide(), res.TaxHAmount);
            Assert.Equal(from.FinalAmount.Divide(), res.FinalAmount);
            Assert.Equal(from.SubTotalAmount.Divide(), res.SubTotalAmount);
        }

        public class TransactionRecord07TestsCases : TheoryData<TransactionRecord07>
        {
            public TransactionRecord07TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord07()
                {
                    StringType = 1.23M,
                    TaxBAmount = 1.23M,
                    TaxBSales = 1.23M,
                    TaxAAmount = 1.23M,
                    TaxCAmount = 1.23M,
                    TaxASales = 1.23M,
                    TaxCSales = 1.23M,
                    TaxDAmount = 1.23M,
                    TaxDSales = 1.23M,
                    TaxEAmount = 1.23M,
                    TaxESales = 1.23M,
                    TaxGAmount = 1.23M,
                    TaxGSales = 1.23M,
                    TaxHAmount = 1.23M,
                    TaxHSales = 1.23M,
                    TaxFAmount = 1.23M,
                    TaxFSales = 1.23M,
                });

                // Read from file
                foreach (TransactionRecord07 t in Utility.GetTransactionDataFromXml<TransactionRecord07>("07"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord08"/> to <see cref="TaxRefund"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord08TestsCases))]
        public void Mapping_TransactionRecord08_To_TaxRefund_MapsAllFields(TransactionRecord08 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            TaxRefund res = mapper.Map<TransactionRecord08, TaxRefund>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TaxAAmount", "TaxBAmount", "TaxCAmount", "TaxASales", "TaxBSales", "TaxCSales", "TaxDSales", "TaxDAmount", "TaxESales", "TaxEAmount", "TaxFSales", "TaxFAmount", "TaxGSales", "TaxGAmount", "TaxHSales", "TaxHAmount", "FinalAmount", "SubTotalAmount" });

            Assert.Equal(from.TaxAAmount.Divide(), res.TaxAAmount);
            Assert.Equal(from.TaxBAmount.Divide(), res.TaxBAmount);
            Assert.Equal(from.TaxCAmount.Divide(), res.TaxCAmount);
            Assert.Equal(from.TaxASales.Divide(), res.TaxASales);
            Assert.Equal(from.TaxBSales.Divide(), res.TaxBSales);
            Assert.Equal(from.TaxCSales.Divide(), res.TaxCSales);
            Assert.Equal(from.TaxDSales.Divide(), res.TaxDSales);
            Assert.Equal(from.TaxDAmount.Divide(), res.TaxDAmount);
            Assert.Equal(from.TaxESales.Divide(), res.TaxESales);
            Assert.Equal(from.TaxEAmount.Divide(), res.TaxEAmount);
            Assert.Equal(from.TaxFSales.Divide(), res.TaxFSales);
            Assert.Equal(from.TaxFAmount.Divide(), res.TaxFAmount);
            Assert.Equal(from.TaxGSales.Divide(), res.TaxGSales);
            Assert.Equal(from.TaxGAmount.Divide(), res.TaxGAmount);
            Assert.Equal(from.TaxHSales.Divide(), res.TaxHSales);
            Assert.Equal(from.TaxHAmount.Divide(), res.TaxHAmount);
            Assert.Equal(from.FinalAmount.Divide(), res.FinalAmount);
            Assert.Equal(from.SubTotalAmount.Divide(), res.SubTotalAmount);
        }

        public class TransactionRecord08TestsCases : TheoryData<TransactionRecord08>
        {
            public TransactionRecord08TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord08()
                {
                    StringType = 1.23M,
                    TaxBAmount = 1.23M,
                    TaxBSales = 1.23M,
                    TaxAAmount = 1.23M,
                    TaxCAmount = 1.23M,
                    TaxASales = 1.23M,
                    TaxCSales = 1.23M,
                    TaxDAmount = 1.23M,
                    TaxDSales = 1.23M,
                    TaxEAmount = 1.23M,
                    TaxESales = 1.23M,
                    TaxGAmount = 1.23M,
                    TaxGSales = 1.23M,
                    TaxHAmount = 1.23M,
                    TaxHSales = 1.23M,
                    TaxFAmount = 1.23M,
                    TaxFSales = 1.23M,
                });

                // Read from file
                foreach (TransactionRecord08 t in Utility.GetTransactionDataFromXml<TransactionRecord08>("08"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord09"/> to <see cref="TenderExchange"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord09TestsCases))]
        public void Mapping_TransactionRecord09_To_TenderExchange_MapsAllFields(TransactionRecord09 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            TenderExchange res = mapper.Map<TransactionRecord09, TenderExchange>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TenderAmount", "TenderType" });

            Assert.Equal(from.TenderAmount.Divide().Negate(), res.TenderAmount);
            Assert.Equal(from.TenderType.getTenderTypeName().ToString(), res.TenderType);
        }

        public class TransactionRecord09TestsCases : TheoryData<TransactionRecord09>
        {
            public TransactionRecord09TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord09()
                {
                    StringType = 1,
                    TenderType = "ABC123",
                    TenderAmount = 1.23M
                });

                // Read from file
                foreach (TransactionRecord09 t in Utility.GetTransactionDataFromXml<TransactionRecord09>("09"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord10"/> to <see cref="ManagerOverride"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord10TestsCases))]
        public void Mapping_TransactionRecord10_To_ManagerOverride_MapsAllFields(TransactionRecord10 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            ManagerOverride res = mapper.Map<TransactionRecord10, ManagerOverride>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord10TestsCases : TheoryData<TransactionRecord10>
        {
            public TransactionRecord10TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord10()
                {
                    StringType = 1,
                    Override = 1,
                    Reason = "ABC123",
                    Index = 1,
                    Initials = "ABC123",
                });

                // Read from file
                foreach (TransactionRecord10 t in Utility.GetTransactionDataFromXml<TransactionRecord10>("10"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord16"/> to <see cref="PaymentProcessorRequest"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord16TestsCases))]
        public void Mapping_TransactionRecord16_To_PaymentProcessorRequest_MapsAllFields(TransactionRecord16 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            PaymentProcessorRequest res = mapper.Map<TransactionRecord16, PaymentProcessorRequest>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "ActionCode", "EntryMethod", "ApprovalCodeSource", "TotalAmount", "CashBack", "TotalsType", "MessageType", "EpsFailureReason", "TenderType", "HostResponseIndicator" });

            Assert.Equal(from.TotalAmount.Divide(), res.TotalAmount);
            Assert.Equal(from.CashBack.Divide(), res.CashBack);
            Assert.Equal(from.TotalType16Enum.ToString(), res.TotalsType);
            Assert.Equal(from.MessageType16Enum.ToString(), res.MessageType);
            Assert.Equal(from.ReasonCode16Enum.ToString(), res.EpsFailureReason);
            Assert.Equal(from.TenderType16Enum.ToString(), res.TenderType);
            Assert.Equal(from.getHostResponseIndicator(), res.HostResponseIndicator);
            Assert.Equal(from.getActionCode(), res.ActionCode);
            Assert.Equal(from.getEntryMethod(), res.EntryMethod);
            Assert.Equal(from.getApprovalCodeSource(), res.ApprovalCodeSource);
        }

        public class TransactionRecord16TestsCases : TheoryData<TransactionRecord16>
        {
            public TransactionRecord16TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord16()
                {
                    StringType = 1,
                    HostId = "ABC123",
                    TotalsType = 1,
                    TotalAmount = 1.23M,
                    DateTimeString16 = new DateTime(2024, 7, 26, 10, 41, 56).ToString("yyMMddHHmmss"),
                    ActionCode = "ABC123",
                    RelativeRecordNumber = "ABC123",
                    Sequence = 1,
                    MessageType = 1,
                    ResponseCode = 1,
                    EpsFailureReason = 1,
                    ExpirationDate = 1,
                    TenderType = 1,
                    CashBack = 1.23M,
                    Account16 = "ABC123",
                    EntryMethod = "ABC123",
                    TrackTwo = "ABC123",
                    ApprovalCode = "ABC123",
                    ResultCodeDescription = "ABC123",
                    ApprovalCodeSource = "ABC123",
                    OriginalHostResponseCode = 1,
                    HostResponseIndicator = 1,
                    Flags1 = 1
                });

                // Read from file
                foreach (TransactionRecord16 t in Utility.GetTransactionDataFromXml<TransactionRecord16>("16"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord11BD"/> to <see cref="CouponDataEntry"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord11BDTestsCases))]
        public void Mapping_TransactionRecord11BD_To_CouponDataEntry_MapsAllFields(TransactionRecord11BD from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            CouponDataEntry res = mapper.Map<TransactionRecord11BD, CouponDataEntry>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "CouponPrice", "UnitPrice", "DepartmentNumber", "CouponDepartment" });

            Assert.Equal(from.CouponPrice.Divide(), res.CouponPrice);
            Assert.Equal(from.UnitPrice.Divide(), res.UnitPrice);
            Assert.Equal(from.DepartmentNumber.ToNullableInt(), res.DepartmentNumber);
            Assert.Equal(from.CouponDepartment.ToNullableInt(), res.CouponDepartment);
            Assert.Equal(from.CouponDepartment.ToNullableInt(), res.CouponDepartment);
        }

        public class TransactionRecord11BDTestsCases : TheoryData<TransactionRecord11BD>
        {
            public TransactionRecord11BDTestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord11BD()
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    SubStringType = "ABC123",
                    Indicator = "ABC123",
                    ItemNumber = "ABC123",
                    UnitPrice = 1.23M,
                    DepartmentNumber = 1,
                    CouponItemCode = 123456,
                    CouponPrice = 1.23M,
                    CouponDepartment = 1
                });

                // Read from file
                foreach (TransactionRecord11BD t in Utility.GetTransactionDataFromXml<TransactionRecord11BD>("11", null, null, "BD"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord11DB"/> to <see cref="UsedTargetedCoupon"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord11DBTestsCases))]
        public void Mapping_TransactionRecord11DB_To_UsedTargetedCoupon_MapsAllFields(TransactionRecord11DB from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            UsedTargetedCoupon res = mapper.Map<TransactionRecord11DB, UsedTargetedCoupon>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord11DBTestsCases : TheoryData<TransactionRecord11DB>
        {
            public TransactionRecord11DBTestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord11DB()
                {
                    StringType = 1,
                    Identifier = 1,
                    LoyaltyNumber = "ABC123",
                    TargetedCoupon = 1
                });

                // Read from file
                foreach (TransactionRecord11DB t in Utility.GetTransactionDataFromXml<TransactionRecord11DB>("11", null, null, "DB"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord11DD"/> to <see cref="CouponDataEntry"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord11DDTestsCases))]
        public void Mapping_TransactionRecord11DD_To_CouponDataEntry_MapsAllFields(TransactionRecord11DD from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            CouponDataEntry res = mapper.Map<TransactionRecord11DD, CouponDataEntry>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "CampaignId", "ManufacturerId", "PromotionCode", "AssociatedItemId", "Unused", "HasCouponRequiredMultipleItemsInOrderToBeIssued", "HasKeyedValueLimitCheck", "HasCouponValueExceedsItemValue", "HasQuantityNotSatisfiedForCoupon", "HasTooManyCouponsRelativeToSales", "HasTooManyLikeCouponsForTransaction", "HasCouponHasExpired", "HasMinimumPurchaseNotSatisfied", "HasCouponGoodForFreeItem", "HasOperatorOverrideRequired", "HasManagerOverrideRequired", "HasCouponDidNotRequireValidation", "HasOnlyFamilySuperGroupOrFamilyGroupIsValid", "HasOnlyManufacturerIsValid", "HasOnlyDepartmentIsValid", "IsNoMatchFound" });

            Assert.Equal(from.CampaignId.ToNullableString(), res.CampaignId);
            Assert.Equal(from.ManufacturerId.ToNullableString(), res.ManufacturerId);
            Assert.Equal(from.PromotionCode.ToNullableString(), res.PromotionCode);
            Assert.Equal(from.AssociatedItemId.ToNullableString(), res.AssociatedItemId);
            Assert.Equal(from.Unused.ToNullableInt(), res.Unused);
            Assert.Equal(from.LogFlags.Bit0.ToNullableBoolean(), res.HasCouponRequiredMultipleItemsInOrderToBeIssued);
            Assert.Equal(from.LogFlags.Bit1.ToNullableBoolean(), res.HasKeyedValueLimitCheck);
            Assert.Equal(from.LogFlags.Bit2.ToNullableBoolean(), res.HasCouponValueExceedsItemValue);
            Assert.Equal(from.LogFlags.Bit3.ToNullableBoolean(), res.HasQuantityNotSatisfiedForCoupon);
            Assert.Equal(from.LogFlags.Bit4.ToNullableBoolean(), res.HasTooManyCouponsRelativeToSales);
            Assert.Equal(from.LogFlags.Bit5.ToNullableBoolean(), res.HasTooManyLikeCouponsForTransaction);
            Assert.Equal(from.LogFlags.Bit6.ToNullableBoolean(), res.HasCouponHasExpired);
            Assert.Equal(from.LogFlags.Bit7.ToNullableBoolean(), res.HasMinimumPurchaseNotSatisfied);
            Assert.Equal(from.LogFlags.Bit8.ToNullableBoolean(), res.HasCouponGoodForFreeItem);
            Assert.Equal(from.LogFlags.Bit9.ToNullableBoolean(), res.HasOperatorOverrideRequired);
            Assert.Equal(from.LogFlags.Bit10.ToNullableBoolean(), res.HasManagerOverrideRequired);
            Assert.Equal(from.LogFlags.Bit11.ToNullableBoolean(), res.HasCouponDidNotRequireValidation);
            Assert.Equal(from.LogFlags.Bit12.ToNullableBoolean(), res.HasOnlyFamilySuperGroupOrFamilyGroupIsValid);
            Assert.Equal(from.LogFlags.Bit13.ToNullableBoolean(), res.HasOnlyManufacturerIsValid);
            Assert.Equal(from.LogFlags.Bit14.ToNullableBoolean(), res.HasOnlyDepartmentIsValid);
            Assert.Equal(from.LogFlags.Bit15.ToNullableBoolean(), res.IsNoMatchFound);
        }

        public class TransactionRecord11DDTestsCases : TheoryData<TransactionRecord11DD>
        {
            public TransactionRecord11DDTestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord11DD()
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    Unused = 1,
                    SubStringType = "ABC123",
                    LogFlags = new LogFlags(),
                    CampaignId = "ABC123",
                    ManufacturerId = "ABC123",
                    PromotionCode = "ABC123",
                    AssociatedItemId = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord11DD t in Utility.GetTransactionDataFromXml<TransactionRecord11DD>("11", null, null, "DD"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord9904"/> to <see cref="TenderExchange"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord9904TestsCases))]
        public void Mapping_TransactionRecord9904_To_TenderExchange_MapsAllFields(TransactionRecord9904 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            TenderExchange res = mapper.Map<TransactionRecord9904, TenderExchange>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "SignatureSource", "SignatureFormat", "SignatureName" });

            Assert.Equal(from.getSignatureSource(), res.SignatureSource);
            Assert.Equal(from.SignatureFormat, res.SignatureFormat);
            Assert.Equal(from.SignatureName, res.SignatureName);
        }

        public class TransactionRecord9904TestsCases : TheoryData<TransactionRecord9904>
        {
            public TransactionRecord9904TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord9904()
                {
                    StringType = 1,
                    TenderEntrySequenceNumber = 1,
                    Originator = 1,
                    SubType = 1,
                    SignatureSource = "ABC123",
                    SignatureFormat = "ABC123",
                    SignatureName = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord9904 t in Utility.GetTransactionDataFromXml<TransactionRecord9904>("99" ,"04"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99050"/> to <see cref="AceItem"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99050TestsCases))]
        public void Mapping_TransactionRecord99050_To_AceItem_MapsAllFields(TransactionRecord99050 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            AceItem res = mapper.Map<TransactionRecord99050, AceItem>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "RefundReason" });

            Assert.Equal(from.RefundReason.ConvertToRefundReason(), res.RefundReason);
        }

        public class TransactionRecord99050TestsCases : TheoryData<TransactionRecord99050>
        {
            public TransactionRecord99050TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99050()
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    RefundReason = 1
                });

                // Read from file
                foreach (TransactionRecord99050 t in Utility.GetTransactionDataFromXml<TransactionRecord99050>("99" ,"0" ,"50"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99104"/> to <see cref="PharmacyItem"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99104TestsCases))]
        public void Mapping_TransactionRecord99104_To_PharmacyItem_MapsAllFields(TransactionRecord99104 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            PharmacyItem res = mapper.Map<TransactionRecord99104, PharmacyItem>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TotalAmount", "InsuranceAmount", "NetDue" });

            Assert.Equal(from.TotalAmount.Divide(), res.TotalAmount);
            Assert.Equal(from.InsuranceAmount.Divide(), res.InsuranceAmount);
            Assert.Equal(from.NetDue.Divide(), res.NetDue);
        }

        public class TransactionRecord99104TestsCases : TheoryData<TransactionRecord99104>
        {
            public TransactionRecord99104TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99104()
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    RxTransactionNumber = "ABC123",
                    CustomerEsignatureType = "ABC123",
                    HostIndicator = "ABC123",
                    PharmacyItemSequenceNumber = "ABC123",
                    DateTimeString = new DateTime(2024, 7, 26, 10, 45, 34).ToString("yyyyMMddHHmmss"),
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    Operator = "ABC123",
                    ElectronicSignatureType = "ABC123",
                    ElectronicSignatureName = "ABC123",
                    HostResponseCode = "ABC123",
                    TotalAmount = 1.23M,
                    InsuranceAmount = 1.23M,
                    NetDue = 1.23M,
                    Barcode = "ABC123",
                    RefillNumber = "ABC123",
                    PartialFillSequenceNumber = "ABC123",
                    StoreNumber = 1
                });

                // Read from file
                foreach (TransactionRecord99104 t in Utility.GetTransactionDataFromXml<TransactionRecord99104>("99", "10", "4"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991012"/> to <see cref="CustomArrangement"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991012TestsCases))]
        public void Mapping_TransactionRecord991012_To_CustomArrangement_MapsAllFields(TransactionRecord991012 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            CustomArrangement res = mapper.Map<TransactionRecord991012, CustomArrangement>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "Void" });

            Assert.Equal(from.Void.ToNullableBoolean(), res.Void);
        }

        public class TransactionRecord991012TestsCases : TheoryData<TransactionRecord991012>
        {
            public TransactionRecord991012TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991012()
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    UniversalProductCode = "ABC123",
                    Void = true,
                    ServiceDeptUPC = "ABC123",
                    ServiceDept = "ABC123",
                    NumberOfItems = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991012 t in Utility.GetTransactionDataFromXml<TransactionRecord991012>("99", "10", "12"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991014"/> to <see cref="CustomArrangement"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991014TestsCases))]
        public void Mapping_TransactionRecord991014_To_CustomArrangement_MapsAllFields(TransactionRecord991014 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            CustomArrangement res = mapper.Map<TransactionRecord991014, CustomArrangement>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991014"/> to <see cref="AceCustomArrangementItem"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991014TestsCases))]
        public void Mapping_TransactionRecord991014_To_AceCustomArrangementItem_MapsAllFields(TransactionRecord991014 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            AceCustomArrangementItem res = mapper.Map<TransactionRecord991014, AceCustomArrangementItem>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991014TestsCases : TheoryData<TransactionRecord991014>
        {
            public TransactionRecord991014TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991014()
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    UniversalProductCode = "ABC123",
                    ItemNumber = "ABC123",
                    Quantity = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991014 t in Utility.GetTransactionDataFromXml<TransactionRecord991014>("99", "10", "14"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991026"/> to <see cref="InstacartQR"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991026TestsCases))]
        public void Mapping_TransactionRecord991026_To_InstacartQR_MapsAllFields(TransactionRecord991026 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            InstacartQR res = mapper.Map<TransactionRecord991026, InstacartQR>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991026TestsCases : TheoryData<TransactionRecord991026>
        {
            public TransactionRecord991026TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991026()
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    Originator = 1,
                    SubType = 1,
                    QRCodeInTransaction = 1,
                    BIN = "ABC123",
                    LastFour = "ABC123",
                    Bypass = "ABC123",
                    BypassOrderId = "ABC123",
                    BypassDeliveryId = "ABC123",
                    ShopperType = "ABC123",
                    OrderingPlatform = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991026 t in Utility.GetTransactionDataFromXml<TransactionRecord991026>("99", "10", "26"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991028"/> to <see cref="Meals2GoQR"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991028TestsCases))]
        public void Mapping_TransactionRecord991028_To_Meals2GoQR_MapsAllFields(TransactionRecord991028 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            Meals2GoQR res = mapper.Map<TransactionRecord991028, Meals2GoQR>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991028TestsCases : TheoryData<TransactionRecord991028>
        {
            public TransactionRecord991028TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991028()
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    Originator = 1,
                    SubType = 1,
                    Id = "ABC123",
                    Status = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991028 t in Utility.GetTransactionDataFromXml<TransactionRecord991028>("99", "10", "28"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991029"/> to <see cref="ShopicQR"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991029TestsCases))]
        public void Mapping_TransactionRecord991029_To_ShopicQR_MapsAllFields(TransactionRecord991029 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            ShopicQR res = mapper.Map<TransactionRecord991029, ShopicQR>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991029TestsCases : TheoryData<TransactionRecord991029>
        {
            public TransactionRecord991029TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991029()
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    Originator = 1,
                    SubType = 1,
                    Id = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991029 t in Utility.GetTransactionDataFromXml<TransactionRecord991029>("99", "10", "29"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991031"/> to <see cref="AmazonDashCartQR"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991031TestsCases))]
        public void Mapping_TransactionRecord991031_To_AmazonDashCartQR_MapsAllFields(TransactionRecord991031 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            AmazonDashCartQR res = mapper.Map<TransactionRecord991031, AmazonDashCartQR>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991031TestsCases : TheoryData<TransactionRecord991031>
        {
            public TransactionRecord991031TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991031()
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    Originator = 1,
                    SubType = 1,
                    Id = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991031 t in Utility.GetTransactionDataFromXml<TransactionRecord991031>("99", "10", "31"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991096"/> to <see cref="ValueCard"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991096TestsCases))]
        public void Mapping_TransactionRecord991096_To_ValueCard_MapsAllFields(TransactionRecord991096 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            ValueCard res = mapper.Map<TransactionRecord991096, ValueCard>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991096TestsCases : TheoryData<TransactionRecord991096>
        {
            public TransactionRecord991096TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991096()
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Plan = "ABC123",
                    AccountType = 1,
                    RequestType = 1,
                    Amount = 1,
                    Sequence = "ABC123",
                    DateTimeString = new DateTime(2024, 7, 26, 10, 46, 12).ToString("yyMMddHHmmss"),
                    LoyaltyNumber = "ABC123",
                    EntryMethod = 1,
                    UserField = "ABC123",
                    Track1 = "ABC123",
                    Track2 = "ABC123",
                    Track3 = "ABC123",
                    ApprovalCode = 1,
                    ApprovalCodeSource = "ABC123",
                    ResponseCode = 1,
                    Flags = "ABC123",
                    PromotionServiceID = "ABC123",
                    PromotionServiceGiftCardID = "ABC123",
                    LotteryItemRRN = "ABC123",
                    LotteryPlayerNumber = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991096 t in Utility.GetTransactionDataFromXml<TransactionRecord991096>("99", "10", "96"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99111"/> to <see cref="AddItemButtonPressed"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99111TestsCases))]
        public void Mapping_TransactionRecord99111_To_AddItemButtonPressed_MapsAllFields(TransactionRecord99111 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            AddItemButtonPressed res = mapper.Map<TransactionRecord99111, AddItemButtonPressed>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "MinimumAgeRestriction", "CurrentScaleWeight" });

            Assert.Equal(from.MinimumAgeRestriction.ToNullableInt(), res.MinimumAgeRestriction);
            Assert.Equal(from.CurrentScaleWeight.DivideDouble(), res.CurrentScaleWeight);
        }

        public class TransactionRecord99111TestsCases : TheoryData<TransactionRecord99111>
        {
            public TransactionRecord99111TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99111()
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Operator = "ABC123",
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    ExceptionType = "ABC123",
                    ExceptionCode = "ABC123",
                    ItemCode = "ABC123",
                    CurrentScaleWeight = 1.23456f,
                    MinimumAgeRestriction = 1
                });

                // Read from file
                foreach (TransactionRecord99111 t in Utility.GetTransactionDataFromXml<TransactionRecord99111>("99", "11", "1"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99112"/> to <see cref="VoidItemButtonPressed"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99112TestsCases))]
        public void Mapping_TransactionRecord99112_To_VoidItemButtonPressed_MapsAllFields(TransactionRecord99112 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            VoidItemButtonPressed res = mapper.Map<TransactionRecord99112, VoidItemButtonPressed>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "MinimumAgeRestriction", "CurrentScaleWeight" });

            Assert.Equal(from.MinimumAgeRestriction.ToNullableInt(), res.MinimumAgeRestriction);
            Assert.Equal(from.CurrentScaleWeight.DivideDouble(), res.CurrentScaleWeight);
        }

        public class TransactionRecord99112TestsCases : TheoryData<TransactionRecord99112>
        {
            public TransactionRecord99112TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99112
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Operator = "ABC123",
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    ExceptionType = "ABC123",
                    ExceptionCode = "ABC123",
                    ItemCode = "ABC123",
                    CurrentScaleWeight = 1.23456f,
                    MinimumAgeRestriction = 1
                });

                // Read from file
                foreach (TransactionRecord99112 t in Utility.GetTransactionDataFromXml<TransactionRecord99112>("99", "11", "2"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99113"/> to <see cref="VoidItemButtonPressedDuringException"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99113TestsCases))]
        public void Mapping_TransactionRecord99113_To_VoidItemButtonPressedDuringException_MapsAllFields(TransactionRecord99113 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            VoidItemButtonPressedDuringException res = mapper.Map<TransactionRecord99113, VoidItemButtonPressedDuringException>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "MinimumAgeRestriction", "CurrentScaleWeight" });

            Assert.Equal(from.MinimumAgeRestriction.ToNullableInt(), res.MinimumAgeRestriction);
            Assert.Equal(from.CurrentScaleWeight.DivideDouble(), res.CurrentScaleWeight);
        }

        public class TransactionRecord99113TestsCases : TheoryData<TransactionRecord99113>
        {
            public TransactionRecord99113TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99113
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Operator = "ABC123",
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    ExceptionType = "ABC123",
                    ExceptionCode = "ABC123",
                    UniversalProductCode = "ABC123",
                    CurrentScaleWeight = 1.23456f,
                    MinimumAgeRestriction = 1
                });

                // Read from file
                foreach (TransactionRecord99113 t in Utility.GetTransactionDataFromXml<TransactionRecord99113>("99", "11", "3"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99115"/> to <see cref="RemoveFromListButtonPressed"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99115TestsCases))]
        public void Mapping_TransactionRecord99115_To_RemoveFromListButtonPressed_MapsAllFields(TransactionRecord99115 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            RemoveFromListButtonPressed res = mapper.Map<TransactionRecord99115, RemoveFromListButtonPressed>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "MinimumAgeRestriction", "CurrentScaleWeight" });

            Assert.Equal(from.MinimumAgeRestriction.ToNullableInt(), res.MinimumAgeRestriction);
            Assert.Equal(from.CurrentScaleWeight.DivideDouble(), res.CurrentScaleWeight);
        }

        public class TransactionRecord99115TestsCases : TheoryData<TransactionRecord99115>
        {
            public TransactionRecord99115TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99115
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Operator = "ABC123",
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    ExceptionType = "ABC123",
                    ExceptionCode = "ABC123",
                    UniversalProductCode = "ABC123",
                    CurrentScaleWeight = 1.23456f,
                    MinimumAgeRestriction = 1
                });

                // Read from file
                foreach (TransactionRecord99115 t in Utility.GetTransactionDataFromXml<TransactionRecord99115>("99", "11", "5"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99116"/> to <see cref="BypassAuditButtonPressed"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99116TestsCases))]
        public void Mapping_TransactionRecord99116_To_BypassAuditButtonPressed_MapsAllFields(TransactionRecord99116 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            BypassAuditButtonPressed res = mapper.Map<TransactionRecord99116, BypassAuditButtonPressed>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "MinimumAgeRestriction", "CurrentScaleWeight" });

            Assert.Equal(from.MinimumAgeRestriction.ToNullableInt(), res.MinimumAgeRestriction);
            Assert.Equal(from.CurrentScaleWeight.DivideDouble(), res.CurrentScaleWeight);
        }

        public class TransactionRecord99116TestsCases : TheoryData<TransactionRecord99116>
        {
            public TransactionRecord99116TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99116
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Operator = "ABC123",
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    ExceptionType = "ABC123",
                    ExceptionCode = "ABC123",
                    ItemCode = "ABC123",
                    CurrentScaleWeight = 1.23456f,
                    MinimumAgeRestriction = 1
                });

                // Read from file
                foreach (TransactionRecord99116 t in Utility.GetTransactionDataFromXml<TransactionRecord99116>("99", "11", "6"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord99117"/> to <see cref="ItemAddedDuringAudit"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord99117TestsCases))]
        public void Mapping_TransactionRecord99117_To_ItemAddedDuringAudit_MapsAllFields(TransactionRecord99117 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            ItemAddedDuringAudit res = mapper.Map<TransactionRecord99117, ItemAddedDuringAudit>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord99117TestsCases : TheoryData<TransactionRecord99117>
        {
            public TransactionRecord99117TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord99117
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    Operator = "ABC123",
                    TerminalId = "ABC123",
                    TransactionId = "ABC123",
                    ItemNumber = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord99117 t in Utility.GetTransactionDataFromXml<TransactionRecord99117>("99", "11", "7"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991110"/> to <see cref="MobileTransactionStarted"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991110TestsCases))]
        public void Mapping_TransactionRecord991110_To_MobileTransactionStarted_MapsAllFields(TransactionRecord991110 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            MobileTransactionStarted res = mapper.Map<TransactionRecord991110, MobileTransactionStarted>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991110TestsCases : TheoryData<TransactionRecord991110>
        {
            public TransactionRecord991110TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991110
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    TerminalId = "ABC123",
                    LoyaltyNumber = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord991110 t in Utility.GetTransactionDataFromXml<TransactionRecord991110>("99", "11", "10"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord995010"/> to <see cref="InmarCoupon"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord995010TestsCases))]
        public void Mapping_TransactionRecord995010_To_InmarCoupon_MapsAllFields(TransactionRecord995010 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            InmarCoupon res = mapper.Map<TransactionRecord995010, InmarCoupon>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord995010TestsCases : TheoryData<TransactionRecord995010>
        {
            public TransactionRecord995010TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord995010
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    InmarCouponIdentifier = 1,
                    NumberOfAssociatedItems = 1,
                    AssociatedItemNumbers = new string[] { "ABC123", "DEF456" }
                });

                // Read from file
                foreach (TransactionRecord995010 t in Utility.GetTransactionDataFromXml<TransactionRecord995010>("99", "50", "10"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord995012"/> to <see cref="InmarCoupon"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord995012TestsCases))]
        public void Mapping_TransactionRecord995012_To_InmarCoupon_MapsAllFields(TransactionRecord995012 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileStandardStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            InmarCoupon res = mapper.Map<TransactionRecord995012, InmarCoupon>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "InmarCouponShortDescription" });

            Assert.Equal(from.InmarCouponShortDescription, res.InmarCouponShortDescription);
        }

        public class TransactionRecord995012TestsCases : TheoryData<TransactionRecord995012>
        {
            public TransactionRecord995012TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord995012
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    InmarCouponShortDescription = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord995012 t in Utility.GetTransactionDataFromXml<TransactionRecord995012>("99", "50", "12"))
                {
                    AddRow(t);
                }
            }
        }
    }
}