namespace RX.PharmacyBusiness.ETL.CRX540.Business
{
    using System;
    using System.Globalization;
    using RX.PharmacyBusiness.ETL.CRX540.Core;

    public static class DerivePrescriptionSaleData
    {
        /// <summary>
        /// Derives SoldDate based on the entity
        /// </summary>
        public static DateTime DeriveSoldDate(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                return 
                    DateTime.ParseExact(entity.SoldDateKey.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture)
                            .AddSeconds(entity.SoldDateSeconds);
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund &&
                entity.CompletionDate.HasValue)
            {
                //return entity.CompletionDate.Value; //Dates from credit card transaction replaced with AUD Fill Fact dates on 7/21/2022 for INC000002520410. Commenting out to help with future potential tickets.
                return 
                    DateTime.ParseExact(entity.SoldDateKey.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture)
                            .AddSeconds(entity.SoldDateSeconds);
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund &&
                entity.CompletionDate.HasValue)
            {
                return entity.CompletionDate.Value;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund &&
                entity.CancelledDate.HasValue)
            {
                return entity.CancelledDate.Value;
            }

            return entity.FillFactTablePartitionDate;
        }

        /// <summary>
        /// Derives PaymentAmount based on the entity
        /// </summary>
        public static decimal DerivePaymentAmount(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            decimal paymentAmount = entity.PaymentAmount ?? 0;

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                return paymentAmount;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
            {
                return paymentAmount;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund)
            {
                return paymentAmount;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund)
            {
                return (paymentAmount * -1);
            }

            return paymentAmount;
        }

        /// <summary>
        /// Derives TotalPricePaid based on the entity
        /// </summary>
        public static decimal DeriveTotalPricePaid(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            decimal totalUsualAndCustomary = entity.TotalUsualAndCustomary ?? 0;
            decimal patientPricePaid = entity.PatientPricePaid ?? 0;
            decimal totalPricePaid = entity.TotalPricePaid ?? 0;
            decimal paymentAmount = entity.PaymentAmount ?? 0;

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                if (totalUsualAndCustomary == 0.01M &&
                    patientPricePaid == 0.01M)
                {
                    return 0;
                }
                else
                {
                    return totalPricePaid;
                }
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
            {
                if (totalUsualAndCustomary == 0.01M &&
                    patientPricePaid == 0.01M)
                {
                    return 0;
                }
                else
                {
                    return (totalPricePaid * -1);
                }
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund)
            {
                return paymentAmount;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund)
            {
                if (totalUsualAndCustomary == 0.01M &&
                    patientPricePaid == 0.01M)
                {
                    return 0;
                }
                else
                {
                    return (totalPricePaid * -1);
                }
            }

            return 0;
        }

        /// <summary>
        /// Derives PatientPricePaid based on the entity
        /// </summary>
        public static decimal DerivePatientPricePaid(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            decimal totalUsualAndCustomary = entity.TotalUsualAndCustomary ?? 0;
            decimal patientPricePaid = entity.PatientPricePaid ?? 0;
            decimal totalPricePaid = entity.TotalPricePaid ?? 0;
            decimal paymentAmount = entity.PaymentAmount ?? 0;

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                if (totalUsualAndCustomary == 0.01M &&
                    patientPricePaid == 0.01M)
                {
                    return 0;
                }
                else
                {
                    return patientPricePaid;
                }
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
            {
                if (totalUsualAndCustomary == 0.01M &&
                    patientPricePaid == 0.01M)
                {
                    return 0;
                }
                else
                {
                    return (patientPricePaid * -1);
                }
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund)
            {
                return paymentAmount;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund)
            {
                if (totalUsualAndCustomary == 0.01M &&
                    patientPricePaid == 0.01M)
                {
                    return 0;
                }
                else
                {
                    return (patientPricePaid * -1);
                }
            }

            return 0;
        }

        /// <summary>
        /// Derives InsurancePayment based on the entity
        /// </summary>
        public static decimal DeriveInsurancePayment(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return DeriveTotalPricePaid(entity) - DerivePatientPricePaid(entity);
        }

        /// <summary>
        /// Derives ShipHandleFee based on the entity
        /// </summary>
        public static decimal DeriveShipHandleFee(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            decimal shipHandleFee = entity.ShipHandleFee ?? 0;

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                return shipHandleFee;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
            {
                return (shipHandleFee * -1);
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund)
            {
                return 0;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund)
            {
                return (shipHandleFee * -1);
            }

            return 0;
        }

        /// <summary>
        /// Derives PaymentTypeName based on the entity
        /// </summary>
        public static string DerivePaymentTypeName(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                return entity.PaymentTypeName;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
            {
                return entity.PaymentTypeName;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund)
            {
                return PaymentTypes.CreditCardPartialReversal;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund)
            {
                return entity.PaymentTypeName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Derives CourierShipCharge based on the entity
        /// </summary>
        public static decimal DeriveCourierShipCharge(PrescriptionSale entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            decimal courierShipCharge = entity.CourierShipCharge ?? 0;

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.Sold)
            {
                return courierShipCharge;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardFullRefund)
            {
                return (courierShipCharge * -1);
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.CreditCardPartialRefund)
            {
                return 0;
            }

            if (entity.PrescriptionSaleType == PrescriptionSaleTypes.AlternativePaymentRefund)
            {
                return (courierShipCharge * -1);
            }

            return 0;
        }
    }
}
