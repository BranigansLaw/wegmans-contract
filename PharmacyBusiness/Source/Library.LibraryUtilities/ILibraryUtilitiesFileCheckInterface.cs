namespace Library.LibraryUtilities
{
    public interface ILibraryUtilitiesFileCheckInterface
    {
        /// <summary>
        /// Copy file to archive for the QA
        /// </summary>
        /// <param name="sourceNameAndPath"></param>
        void CopyFileToArchiveForQA(string sourceNameAndPath);

        /// <summary>
        /// Move file to archive for the QA
        /// </summary>
        /// <param name="sourceNameAndPath"></param>
        void MoveFileToArchiveForQA(string sourceNameAndPath);
    }
}
