namespace RX.PharmacyBusiness.ETL.CRX540.Business
{
    using System;
    using System.Collections.Generic;
    using RX.PharmacyBusiness.ETL.CRX540.Business.Rules;
    using RX.PharmacyBusiness.ETL.CRX540.Contracts;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public class RetailItemFactory : IRetailItemFactory
    {
        // TODO: Consolidate these
        public const string TransactionSignNegative = "-";
        public const string TransactionSignPositive = "+";
        public const string TransactionTypeRefund = "R";
        public const string TransactionTypeSale = "S";

        public GeneralAccountingRetailItem[] Create(PrescriptionSale sale, DateTime date)
        {
            var pharmacySaleRule = new IsItemPrescriptionSaleRule(date);
            var shippingChargeRule = new IsItemShippingChargeRule(date);
            var thirdPartyRule = new IsItemThirdPartyPaymentRule(date);
            var items = new List<GeneralAccountingRetailItem>();
            
            if (pharmacySaleRule.IsMetBy(sale))
            {
                items.Add(
                    this.Create(sale, GeneralAccountingRetailTypes.PrescriptionSale));
            }

            if (shippingChargeRule.IsMetBy(sale))
            {
                items.Add(
                    this.Create(sale, GeneralAccountingRetailTypes.ShippingCharge));
            }

            if (thirdPartyRule.IsMetBy(sale))
            {
                items.Add(
                    this.Create(sale, GeneralAccountingRetailTypes.ThirdPartyPayment));
            }

            return items.ToArray();
        }

        private GeneralAccountingRetailItem Create(PrescriptionSale sale, GeneralAccountingRetailTypes retailType)
        {
            return new GeneralAccountingRetailItem
            {
                GeneralAccountingRetailType = retailType,
                TransactionDate = DerivePrescriptionSaleData.DeriveSoldDate(sale),
                StoreNumber = Convert.ToInt32(sale.StoreNumber),
                OrderNumber = sale.OrderNumber,
                ItemUniversalProductNumber = this.DefineItemUniversalProductNumberNEW(sale, retailType),
                Retail = this.DefineRetail(sale, retailType),
                TenderType = this.DefineTenderType(sale),
                TransactionType = this.DefineTransactionType(sale, retailType),
                TransactionSign = this.DefineTransactionSign(sale, retailType),
                TenderAccountNumber = sale.LastFour,
                TrackingNumber = sale.TrackingNumber
            };            
        }

        private long DefineItemUniversalProductNumberNEW(PrescriptionSale sale, GeneralAccountingRetailTypes retailType)
        {
            switch (retailType)
            {
                case GeneralAccountingRetailTypes.PrescriptionSale:
                    return ItemUniversalProductNumbers.PrescriptionItem;
                case GeneralAccountingRetailTypes.ShippingCharge:
                    return ItemUniversalProductNumbers.ShippingChargeItem;
                case GeneralAccountingRetailTypes.ThirdPartyPayment:
                    if (sale.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
                    {
                        return ItemUniversalProductNumbers.ThirdPartyRefundItem;
                    }
                    else
                    {
                        return ItemUniversalProductNumbers.ThirdPartyPaymentItem;
                    }
                default:
                    return ItemUniversalProductNumbers.UndefinedItem;
            }
        }

        private decimal DefineRetail(PrescriptionSale sale, GeneralAccountingRetailTypes retailType)
        {
            decimal retail;

            switch (retailType)
            {
                case GeneralAccountingRetailTypes.PrescriptionSale:
                    retail = Math.Abs(DerivePrescriptionSaleData.DeriveTotalPricePaid(sale));
                    break;
                case GeneralAccountingRetailTypes.ShippingCharge:
                    retail = Math.Abs(DerivePrescriptionSaleData.DeriveShipHandleFee(sale));
                    break;
                case GeneralAccountingRetailTypes.ThirdPartyPayment:
                    retail = Math.Abs(DerivePrescriptionSaleData.DeriveInsurancePayment(sale));
                    break;
                default:
                    retail = 0;
                    break;
            }

            if (this.DefineTransactionSign(sale, retailType) == TransactionSignNegative)
            {
                retail = retail * -1;
            }

            return retail;
        }

        private int DefineTenderType(PrescriptionSale sale)
        {
            switch (sale.PaymentTypeName)
            {
                case PaymentTypes.CreditCard:
                    return 41;
                case PaymentTypes.CreditCardReversal:
                    return 41;
                case PaymentTypes.CreditCardPartialReversal:
                    return 41;
                case PaymentTypes.Check:
                    return 21;
                case PaymentTypes.ElectronicCheck:
                    return 46;
                case PaymentTypes.Cash:
                    return 11;
                default:
                    return 54;
            }
        }

        private bool IsNegativePaymentOrNegativeTotalPricePaid(PrescriptionSale sale)
        {
            decimal paymentAmount = DerivePrescriptionSaleData.DerivePaymentAmount(sale);
            decimal totalPricePaid = DerivePrescriptionSaleData.DeriveTotalPricePaid(sale);
            decimal insurancePayment = DerivePrescriptionSaleData.DeriveInsurancePayment(sale);
            decimal patientPricePaid = DerivePrescriptionSaleData.DerivePatientPricePaid(sale);

            if (paymentAmount == -0.01M) paymentAmount = 0.01M;
            if (totalPricePaid == -0.01M) totalPricePaid = 0.01M;
            if (insurancePayment == -0.01M) insurancePayment = 0.01M;
            if (patientPricePaid == -0.01M) patientPricePaid = 0.01M;

            return paymentAmount < 0 || totalPricePaid < 0 ||
                sale.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund;
        }

        private string DefineTransactionSign(PrescriptionSale sale, GeneralAccountingRetailTypes retailType)
        {
            decimal? totalPricePaid = sale.TotalPricePaid;
            decimal insurancePayment = DerivePrescriptionSaleData.DeriveInsurancePayment(sale);
            decimal? patientPricePaid = sale.PatientPricePaid;

            if (retailType == GeneralAccountingRetailTypes.ThirdPartyPayment)
            {
                if (sale.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
                {
                    string TransactionSign = "+";

                    if (insurancePayment < 0)
                    {
                        TransactionSign = "-";
                    }

                    if (!((totalPricePaid > 0 && insurancePayment < 0 && patientPricePaid > totalPricePaid) || (totalPricePaid < 0 && insurancePayment > 0 && patientPricePaid < totalPricePaid)))
                    {
                        TransactionSign = ReverseSign(TransactionSign);
                    }

                    return (TransactionSign == "+")
                              ? TransactionSignPositive
                              : TransactionSignNegative;
                }
                else
                {
                    return ((DerivePrescriptionSaleData.DeriveTotalPricePaid(sale) <= 0 &&
                            DerivePrescriptionSaleData.DeriveInsurancePayment(sale) >= 0) //a refund for negative insurance that was refunded
                            )
                                ? TransactionSignNegative
                                : ((this.IsNegativePaymentOrNegativeTotalPricePaid(sale)
                                    || DerivePrescriptionSaleData.DeriveInsurancePayment(sale) < 0)
                                       ? TransactionSignPositive
                                       : TransactionSignNegative)
                                ;
                }
            }
            else
            {
                if (sale.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
                {
                    string TransactionSign = "+";

                    if (totalPricePaid < 0)
                    {
                        TransactionSign = "-";
                    }

                    if (!((totalPricePaid > 0 && insurancePayment < 0 && patientPricePaid > totalPricePaid) || (totalPricePaid < 0 && insurancePayment > 0 && patientPricePaid < totalPricePaid)))
                    {
                        TransactionSign = ReverseSign(TransactionSign);
                    }

                    return (TransactionSign == "+")
                              ? TransactionSignPositive
                              : TransactionSignNegative;
                }
                else
                {
                    return this.IsNegativePaymentOrNegativeTotalPricePaid(sale)
                           ? TransactionSignNegative
                           : TransactionSignPositive;
                }
            }

        }


        private string DefineTransactionType(PrescriptionSale sale, GeneralAccountingRetailTypes retailType)
        {
            if (retailType == GeneralAccountingRetailTypes.ThirdPartyPayment)
            {
                if (sale.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
                {
                    return TransactionTypeRefund;
                }
                else
                {
                    return TransactionTypeSale;
                }
            }
            else
            {
                return this.IsNegativePaymentOrNegativeTotalPricePaid(sale)
                            ? TransactionTypeRefund
                            : TransactionTypeSale;
            }
        }

        private string ReverseSign(string sign)
        {
            return (sign == "+")
                        ? "-"
                        : "+";
        }
    }
}
