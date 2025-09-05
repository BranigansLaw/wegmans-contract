namespace Library.LibraryUtilities.Extensions
{
    public static class DecimalExtensions
    {
        public static decimal? TrimTrailingZeroes(this decimal? inputValue)
        {
            if (!inputValue.HasValue)
                return inputValue;

            return TrimTrailingZeroes(inputValue.Value);
        }

        public static decimal TrimTrailingZeroes(this decimal inputValue)
        {
            string newValueAsSting = StringExtensions.TrimTrailingZeroes(inputValue.ToString());

            return decimal.Parse(newValueAsSting);
        }
    }
}
