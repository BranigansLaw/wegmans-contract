using System.Linq;
using System.Text.RegularExpressions;
using XXXDeveloperTools.NfmPlansAndFilesets.DataModel;
using XXXDeveloperTools.NfmPlansAndFilesets.Helper;
using static System.Net.Mime.MediaTypeNames;

namespace XXXDeveloperTools.NfmPlansAndFilesets
{
    public class FileMoveTracker
    {
        private PopulateCollectionsHelperImp _helper;
        private string _exportPath;

        public FileMoveTracker(string exportPath)
        {
            if (string.IsNullOrEmpty(exportPath))
                throw new ArgumentException("Export path cannot be null or empty.");

            _exportPath = exportPath;
            if (Directory.Exists(_exportPath) == false)
                Directory.CreateDirectory(_exportPath);

            _helper = new PopulateCollectionsHelperImp();
            _helper.NfmFilesetRows = _helper.PopulateNfmFileset();
            _helper.NfmPlanRows = _helper.PopulateNfmPlan();
            _helper.ControlMRows = _helper.PopulateControlM();
            _helper.PgpLogRows = _helper.PopulatePgpLog();
        }

        public int Run()
        {
            List<string> resultsReport = [];
            List<NfmFileTrackingRow> nfmFileTrackingRows = new List<NfmFileTrackingRow>();
            resultsReport.Add("JobNbr,NfmPlanName,NfmFilesetName,NfmDeploymentDate,SourceNodeName,SourcePath,SourceFileNamePattern,TargetNodeName,TargetPath,TargetFileNamePattern,ExampleRecentDecryptionLogs,ExampleRecentEncryptionLogs");

            foreach (var nfmPlan in _helper.NfmPlanRows)
            {
                string jobNbr = _helper.ControlMRows.Where(x => x.NfmPlan == nfmPlan.PlanName || x.NfmPlan == nfmPlan.ParentPlanName).Count() >= 1 ?
                    _helper.ControlMRows.Where(x => x.NfmPlan == nfmPlan.PlanName || x.NfmPlan == nfmPlan.ParentPlanName).First().JobNbr : "UNKNOWN";
                List<NfmFilesetRow> nfmFilesets = _helper.NfmFilesetRows.Where(x => x.FilesetName == nfmPlan.FilesetName).ToList();

                foreach (var nfmFileset in nfmFilesets)
                {
                    string deploymentDateString = nfmFileset.DeploymentDate.HasValue ? nfmFileset.DeploymentDate.Value.ToShortDateString() : string.Empty;
                    string regexPattern = @"^" + nfmFileset.SourceFileNamePattern.Replace(" + ", "").Replace("*", ".*");
                    IEnumerable<string> recentDecryptedFileNames = _helper.PgpLogRows.Where(x => x.IsDecryption && x.PgpFilePath == nfmFileset.TargetPath && Regex.IsMatch(x.PgpFileName, regexPattern, RegexOptions.IgnoreCase)).OrderByDescending(o => o.PgpDate).Select(s => s.PgpFileName).Distinct();
                    IEnumerable<string> recentEncryptedFileNames = _helper.PgpLogRows.Where(x => !x.IsDecryption && x.PgpFilePath == nfmFileset.SourcePath && Regex.IsMatch(x.PgpFileName, regexPattern, RegexOptions.IgnoreCase)).OrderByDescending(o => o.PgpDate).Select(s => s.PgpFileName).Distinct();

                    resultsReport.Add($"{jobNbr},{nfmPlan.PlanName},{nfmPlan.FilesetName},{deploymentDateString},{nfmPlan.SourceNodeName},{nfmFileset.SourcePath},{nfmFileset.SourceFileNamePattern},{nfmPlan.TargetNodeName},{nfmFileset.TargetPath},{nfmFileset.TargetFileNamePattern},{string.Join("| ", recentDecryptedFileNames.Take(10))},{string.Join(" | ", recentEncryptedFileNames.Take(10))}");

                    nfmFileTrackingRows.Add(new NfmFileTrackingRow
                    {
                        JobNbr = jobNbr,
                        PlanName = nfmPlan.PlanName,
                        FilesetName = nfmPlan.FilesetName, 
                        DeploymentDate = nfmFileset.DeploymentDate.HasValue ? nfmFileset.DeploymentDate.Value : default, 
                        SourceNodeName = nfmPlan.SourceNodeName, 
                        SourcePath = nfmFileset.SourcePath, 
                        SourceFileNamePattern = nfmFileset.SourceFileNamePattern, 
                        TargetNodeName = nfmPlan.TargetNodeName, 
                        TargetPath = nfmFileset.TargetPath, 
                        TargetFileNamePattern = nfmFileset.TargetFileNamePattern ,
                        RecentDecryptedFileNames = recentDecryptedFileNames,
                        RecentEncryptedFileNames = recentEncryptedFileNames
                    });
                }
            }

            string resultsFileName = $"NfmFileMoves_{DateTime.Now.ToString("yyyyMMdd")}.csv";
            using (StreamWriter writerOutputData = new StreamWriter(Path.Combine(_exportPath, resultsFileName), false))
            {
                foreach (string line in resultsReport)
                {
                    writerOutputData.WriteLine(line);
                }
            }

            List<string> distinctDecryptPaths = nfmFileTrackingRows
                .Where(x => x.TargetNodeName.ToUpper() == "TPSNODE" || x.TargetNodeName.ToUpper() == "TPSPROD")
                .Select(x => x.TargetPath)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            List<string> distinctEncryptPaths = nfmFileTrackingRows
                .Where(x => x.SourceNodeName.ToUpper() == "TPSNODE" || x.SourceNodeName.ToUpper() == "TPSPROD")
                .Select(x => x.SourcePath)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            //NOTE: If a grep does not produce any results, it is because either the file was not encrypted/decrypted, and/or because logs get deleted after a couple of weeks.
            string grepFileCommand = $"GrepLogs_{DateTime.Now.ToString("yyyyMMdd")}.txt";
            using (StreamWriter writerOutputData = new StreamWriter(Path.Combine(_exportPath, grepFileCommand), false))
            {
                string grepDecryptFileName = $"/u/a141780/PharmacyGrepDecrypt_{DateTime.Now.ToString("yyyyMMdd")}.txt";
                string grepOutputOperator = ">";
                foreach (var decryptPath in distinctDecryptPaths)
                {
                    writerOutputData.WriteLine($"grep \"  File to decrypt:   {decryptPath}\" /weglog/wegpgpd.*.* {grepOutputOperator} {grepDecryptFileName}");

                    if (grepOutputOperator == ">")
                        grepOutputOperator = ">>";
                }

                string grepEncryptFileName = $"/u/a141780/PharmacyGrepEncrypt_{DateTime.Now.ToString("yyyyMMdd")}.txt";
                grepOutputOperator = ">";
                foreach (var encryptPath in distinctEncryptPaths)
                {
                    writerOutputData.WriteLine($"grep \"   Encrypted file:  {encryptPath}\" /weglog/wegpgpe.*.* {grepOutputOperator} {grepEncryptFileName}");

                    if (grepOutputOperator == ">")
                        grepOutputOperator = ">>";
                }
            }

            return 0;
        }
    }
}
