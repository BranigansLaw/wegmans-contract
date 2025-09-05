namespace Library.TenTenInterface
{
    public class ResultRowRange
    {
        public ResultRowRange(int startRowNumber, int endRowNumber)
        {
            StartRowNumber = startRowNumber;
            EndRowNumber = endRowNumber;
        }

        public int StartRowNumber { get; }

        public int EndRowNumber { get; }
    }
}
