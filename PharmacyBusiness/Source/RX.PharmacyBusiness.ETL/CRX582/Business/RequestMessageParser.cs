namespace RX.PharmacyBusiness.ETL.CRX582.Business
{
    using System;

    public static class RequestMessageParser
    {
        private const string NegativeOverpunchOperators = "}JKLMNOPQR";
        private const string PositiveOverpunchOperators = "{ABCDEFGHI";

        public static string GetValue(string requestMessage, string code)
        {
            var outValue = string.Empty;
            var substringStartPosition = 0;
            const char controlCharacter1 = (char)28;
            const char controlCharacter2 = (char)29;
            const char controlCharacter3 = (char)30;

            if (string.IsNullOrEmpty(requestMessage))
            {
                return outValue;
            }

            substringStartPosition = requestMessage.IndexOf(controlCharacter1 + code, 0);

            if (substringStartPosition > -1)
            {
                var end1 = requestMessage.IndexOf(
                    controlCharacter1, substringStartPosition + controlCharacter1.ToString().Length);
                var end2 = requestMessage.IndexOf(
                    controlCharacter2, substringStartPosition + controlCharacter2.ToString().Length);
                var end3 = requestMessage.IndexOf(
                    controlCharacter3, substringStartPosition + controlCharacter3.ToString().Length);

                var substringEndPosition = 0;
                if (end1 > -1 && end2 > -1)
                {
                    substringEndPosition = (end1 > end2) ? end2 : end1;
                }
                else
                {
                    substringEndPosition = (end1 == -1) ? end2 : end1;
                }

                if (substringEndPosition > -1 && end3 > -1)
                {
                    substringEndPosition = (substringEndPosition > end3) ? end3 : substringEndPosition;
                }
                else
                {
                    substringEndPosition = (substringEndPosition == -1) ? end3 : substringEndPosition;
                }

                substringStartPosition += (controlCharacter1 + code).Length;

                if (substringEndPosition == -1)
                {
                    substringEndPosition = ((requestMessage.Length - substringStartPosition) > 9)
                                               ? 9
                                               : (requestMessage.Length - substringStartPosition);

                    // 9 is the max length possible.
                    outValue = requestMessage.Substring(substringStartPosition, substringEndPosition);
                }
                else
                {
                    outValue = requestMessage.Substring(
                        substringStartPosition, substringEndPosition - substringStartPosition);
                }
            }

            return outValue.Trim();
        }

        public static decimal ToMoney(string inValue)
        {
            decimal outValue = 0;

            if (string.IsNullOrEmpty(inValue))
            {
                return outValue;
            }

            var overpunch = string.IsNullOrEmpty(inValue) ? string.Empty : inValue;

            var negativeOverpunchOperatorPosition = 0;
            var cent = default(int?);

            var overpunchOperator = inValue.Substring(inValue.Length - 1, 1);
            var positiveOverpunchOperatorPosition = PositiveOverpunchOperators.IndexOf(overpunchOperator);
            negativeOverpunchOperatorPosition = NegativeOverpunchOperators.IndexOf(overpunchOperator);

            if (positiveOverpunchOperatorPosition > -1)
            {
                cent = positiveOverpunchOperatorPosition;
            }

            if (negativeOverpunchOperatorPosition > -1)
            {
                cent = negativeOverpunchOperatorPosition;
            }

            if (cent.HasValue)
            {
                overpunch = overpunch.PadLeft(3, '0');
                var money = string.Format(
                    "{0}.{1}{2}", 
                    overpunch.Substring(0, overpunch.Length - 2), 
                    overpunch.Substring(overpunch.Length - 2, 1), 
                    cent);
                outValue = Convert.ToDecimal(money);

                // This is how Production code is now...it does not catch exceptions.

                // if (!Decimal.TryParse(money, out outValue))
                // {
                // outValue = 0M; //TODO: catch exception???
                // }
            }

            return negativeOverpunchOperatorPosition > 0 ? outValue * -1 : outValue;
        }
    }
}