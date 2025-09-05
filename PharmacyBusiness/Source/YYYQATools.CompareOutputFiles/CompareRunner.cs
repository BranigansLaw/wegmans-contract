using YYYQATools.CompareOutputFiles.Helper;

namespace YYYQATools.CompareOutputFiles
{
    public class CompareRunner
    {
        private ICompareCollectionsHelper _helper;
        private string _compareResultsFileName;
        private string _compareResultsPath;

        public CompareRunner(string newFileNameAndPath, string oldFileNameAndPath)
        {
            _helper = new CompareCollectionsHelperImp(newFileNameAndPath, oldFileNameAndPath);
            _compareResultsFileName = $"CompareResults_{Path.GetFileName(newFileNameAndPath)}.txt";
            CreateDirIfNotExists(@"%BATCH_ROOT%\ArchiveForQA\");
            _compareResultsPath = CreateDirIfNotExists(@"%BATCH_ROOT%\ArchiveForQA\CompareResults\");
        }

        public int Run()
        {
            int returnCode = _helper.CompareCollections();

            WriteCompareResultsReport(_helper.GetCompareSummaryReport());

            return returnCode;
        }

        private void WriteCompareResultsReport(string summaryReport)
        {
            List<string> resultsReport = new List<string>();
            resultsReport.Add("=========================================================================");
            resultsReport.Add($"File Comparison Results Summary Report created on {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}");
            resultsReport.Add("_________________________________________________________________________");
            resultsReport.Add(summaryReport);
            resultsReport.Add(string.Empty);
            resultsReport.Add("_________________________________________________________________________");
            
            using (StreamWriter writerOutputData = new StreamWriter(Path.Combine(_compareResultsPath, _compareResultsFileName), true))
            {
                foreach (string line in resultsReport)
                {
                    writerOutputData.WriteLine(line);
                }
            }
        }

        public string CreateDirIfNotExists(string dir)
        {
            dir = Environment.ExpandEnvironmentVariables(dir);

            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
