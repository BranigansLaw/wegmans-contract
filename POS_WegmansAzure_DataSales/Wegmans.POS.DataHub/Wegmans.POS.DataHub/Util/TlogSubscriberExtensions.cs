using System;
using System.Xml.Linq;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.TranslationEnumModel;
using System.Linq;

namespace Wegmans.POS.DataHub.Util
{
    public static class TlogSubscriberExtensions
    {

        public static TerminalType? ConvertToTerminalType(this string id)
        {
            if (int.TryParse(id, out var val))
            {
                return val switch
                {
                    >= 1 and <= 35 => TerminalType.FrontEnd,
                    >= 36 and <= 37 => TerminalType.FrontEndWireless,
                    >= 40 and <= 41 => TerminalType.Wine,
                    >= 42 and <= 43 => TerminalType.CafeWireless,
                    >= 44 and <= 47 => TerminalType.ServiceDesk,
                    48 => TerminalType.Pizza,
                    49 => TerminalType.Floral,
                    52 => TerminalType.Coffee,
                    54 => TerminalType.DryCleaning,
                    55 => TerminalType.Skincare,
                    >= 56 and <= 60 => TerminalType.Cafe,
                    >= 61 and <= 64 => TerminalType.Wine,
                    >= 65 and <= 70 => TerminalType.Pharmacy,
                    71 => TerminalType.SubShop,
                    72 => TerminalType.Cafe,
                    75 => TerminalType.CartToCurb,
                    76 => TerminalType.OnlineCatering,
                    77 => TerminalType.OnlineCatering,
                    78 => TerminalType.Pharmacy,
                    79 => TerminalType.PharmacyMailOrder,
                    >= 80 and <= 89 => TerminalType.SelfCheckout,
                    >= 91 and <= 95 => TerminalType.FrontEndWireless,
                    96 => TerminalType.PharmacyAdjustments,
                    >= 97 and <= 99 => TerminalType.EftController,
                    >= 100 and <= 114 => TerminalType.ServiceDesk,
                    >= 115 and <= 135 => TerminalType.Wine,
                    >= 136 and <= 170 => TerminalType.FrontEnd,
                    >= 171 and <= 189 => TerminalType.Pharmacy,
                    >= 200 and <= 214 => TerminalType.CartToCurb,
                    250 => TerminalType.Meals2Go,
                    251 => TerminalType.OnlineCommerce,
                    >= 261 and <= 280 => TerminalType.CartToCurb,
                    >= 281 and <= 285 => TerminalType.BurgerBar,
                    >= 300 and <= 324 => TerminalType.FrontEndWireless,
                    >= 325 and <= 349 => TerminalType.Catering,
                    >= 390 and <= 399 => TerminalType.SelfCheckout,
                    >= 401 and <= 419 => TerminalType.SelfCheckout,
                    >= 420 and <= 439 => TerminalType.CartToCurb,
                    >= 700 and <= 749 => TerminalType.Payment,
                    >= 750 and <= 799 => TerminalType.ConsumerMobileController,
                    >= 800 and <= 899 => TerminalType.ConsumerMobile,
                    >= 901 and <= 979 => TerminalType.ConsumerMobile,
                    >= 981 and <= 989 => TerminalType.ConsumerMobile,
                    >= 991 and <= 998 => TerminalType.ConsumerMobile,
                    _ => null
                };
            }
            return null;
        }

        public static RefundReason? ConvertToRefundReason(this int id)
        {
            return id switch
            {
                1 => RefundReason.ReShopped,
                2 => RefundReason.NoReshopCredits,
                3 => RefundReason.NoReshopShrink,
                4 => RefundReason.NoReshopRecalls,
                94 => RefundReason.FullRefundRxChange,
                _ => null
            };
        }

        /// <summary>
        ///     Logic for nullable string
        /// </summary>
        /// <param name="i1">The string.</param>
        /// <returns>The nullable string.</returns>
        public static string? ToNullableString(this string i1)
        {
            var returnVar = default(string?);
            if (i1 != "0")
            {
                returnVar = i1;
            }
            return returnVar;
        }

        /// <summary>
        ///     Logic for nullable int
        /// </summary>
        /// <param name="i1">The int.</param>
        /// <returns>The nullable int.</returns>
        public static int? ToNullableInt(this int i1)
        {
            var returnVar = default(int?);
            if (i1 != 0)
            {
                returnVar = i1;
            }
            return returnVar;
        }

        /// <summary>
        ///     Divides two specified  values.
        /// </summary>
        /// <param name="d1">The dividend.</param>
        /// <returns>The result of dividing  by .</returns>
        public static decimal? Divide(this decimal d1)
        {
            var returnVar = default(decimal?);
            if (d1 != 0)
            {
                returnVar = decimal.Divide(d1, 100);
            }
            return returnVar;
        }

        /// <summary>
        ///     Divides two specified  values.
        /// </summary>
        /// <param name="d1">The dividend.</param>
        /// <returns>The result of dividing  by .</returns>
        public static double? DivideDouble(this double d1)
        {
            var returnVar = default(double?);
            if (d1 != 0)
            {
                returnVar = (d1 / (double)100);
            }
            return returnVar;
        }
        /// <summary>
        /// Negate the value if present otherwise return default
        /// </summary>
        /// <param name="d1"></param>
        /// <returns></returns>
        public static decimal? Negate(this decimal? d1)
        {
           if (d1.HasValue)
           {
                return decimal.Negate(d1.Value);
           }
           
           return default(decimal?);
        }

        /// <summary>
        ///  Returns the first 4 characters as the store number from fileName
        /// </summary>
        public static int getStoreNumber(this string stringContainingStoreNumber)
        {
            string storeNumber;
            if (!String.IsNullOrEmpty(stringContainingStoreNumber) && stringContainingStoreNumber.Length > 4)
            {
                storeNumber = stringContainingStoreNumber.Substring(0, 4);
            }
            else
                throw new ArgumentNullException(nameof(stringContainingStoreNumber));

            return Int32.Parse(storeNumber);
        }

        /// <summary>
        ///  Returns the Seconds from a string
        /// </summary>
        public static TimeSpan? getConvertedSeconds(this string timeString)
        {
            if (Double.TryParse(timeString, out var myseconds))
            {
                return TimeSpan.FromSeconds(myseconds);
            }
            else
            {
                return null;
            }

        }

        /// <Summary>
        /// Extension to bool object 
        /// </Summary>
        /// <returns> returns true if bool object is true 
        ///  otherwise it returns bool? (null) </returns>
        public static bool? ToNullableBoolean(this bool value)
        {
            return value ? true : default(bool?);
        }

        public static string SubAreaFormated(this XElement xElement, string elementName, string formatSpacer)
        {
            var element = xElement.Element(elementName);
            if (element is null)
                return null;
            else
                return $"{formatSpacer}{(string)element}";
        }
        public static string getTransactionRecordName(this XElement doc)
        {
            return $"TransactionRecord{doc.SubAreaFormated("StringType", "_")}{doc.SubAreaFormated("Originator", ".")}{doc.SubAreaFormated("SubType", ".")}{doc.SubAreaFormated("SubStringType", ".")}";
        }

        public static Transaction removeCustomerData(this Transaction doc)
        {
            if(doc.CustomerIdentification is not null)
            {
                doc.CustomerIdentification.LoyaltyNumber = "0";
                doc.CustomerIdentification.CustomerNumberAsEntered = "0";
                doc.CustomerIdentification.LoyaltyNumber = "0";
                doc.TenderExchanges?.ToList().ForEach((var) => var.SignatureName = "");
                doc.CustomerIdentification.LoyaltyNumber = "0";
            }
            return doc;
        }
        public static String05TenderTypeNameEnum getTenderTypeName(this string var)
        {
            if (int.TryParse(var, out var number))
            {
                return (String05TenderTypeNameEnum)number;
            }
            else
            {
                return String05TenderTypeNameEnum.NotAvailable;
            }

        }

        /// <Summary>
        /// Extension to bool object 
        /// </Summary>
        /// <returns> returns quantity value if QtyKey was pressed otherwise return 1 </returns>
        public static int getQuantityValue(this bool? value, TransactionRecord02 src)
        {
            int quantityValue = value.HasValue ? Int32.Parse(src.QuantityOrWeightOrVolume) : 1;

            return quantityValue;
        }

        /// <Summary>
        /// Extension to bool object 
        /// </Summary>
        /// <returns> returns weight value if QtyKey was pressed otherwise return 1 </returns>
        public static decimal? getWeightValue(this bool? value, TransactionRecord02 src)
        {
            var returnVar = default(decimal?);

            returnVar = value.HasValue ? Decimal.Divide(Int32.Parse(src.QuantityOrWeightOrVolume), 100) : returnVar;

            return returnVar;
        }

        public static string convertDiscountGroupToCouponCode(this string var)
        {
            switch(var)
            {
                case "05":
                case "11":
                    return "2505";
                case "62":
                    return "6262";
                case "20":
                    return "2371";
                default:   
                    return "unknown";
            }

        }
    }
}