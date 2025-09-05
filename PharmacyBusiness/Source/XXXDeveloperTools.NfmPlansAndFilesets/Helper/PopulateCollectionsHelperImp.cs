using System.Diagnostics;
using XXXDeveloperTools.NfmPlansAndFilesets.DataModel;

namespace XXXDeveloperTools.NfmPlansAndFilesets.Helper
{
    public class PopulateCollectionsHelperImp : IPopulateCollectionsHelper
    {
        public IEnumerable<NfmFilesetRow> NfmFilesetRows;
        public IEnumerable<NfmPlanRow> NfmPlanRows;
        public IEnumerable<ControlMRow> ControlMRows;
        public IEnumerable<PgpLogRow> PgpLogRows;

        /// <summary>
        /// NFM prefixes to filter for Pharmacy file moves.
        /// </summary>
        private string[] _filterNfmPrefixes = new string[] { "CRX", "INN", "RXS", "SEC" };

        public PopulateCollectionsHelperImp()
        {
        }

        /// <inheritdoc />
        public IEnumerable<ControlMRow> PopulateControlM()
        {
            List<ControlMRow> controlMRows = new List<ControlMRow>();

            //NOTE: There is no download from Control-Mbut rather it is manually generated.
            //Fortunately, once this list is created it is easily updated by looking at CRQs in SmartIt since this list was last updated.
            //Last Updated: 2024-10-17
            controlMRows.Add(new ControlMRow { JobNbr = "CRX101", NfmPlan = "CRXSalesAdjQA" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX509", NfmPlan = "CRXSmartRxMV" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX511", NfmPlan = "CRXCDScansFTP" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX515", NfmPlan = "CRXOmnisys" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX516", NfmPlan = "CRXOmnisysMV" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX526", NfmPlan = "CRXRGSWC" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX527", NfmPlan = "CRXWISWC" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX529", NfmPlan = "RXSSec5FileSnd" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX541", NfmPlan = "RXS1010_CRX541" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX543", NfmPlan = "CRX543" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX552", NfmPlan = "CRX1010Drugs" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX553", NfmPlan = "CRX553" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX554", NfmPlan = "CRX1010SRx" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX561", NfmPlan = "CRXWCBEXPORT" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX562", NfmPlan = "CRX1010EDI" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX585", NfmPlan = "RXSdCallCenter" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX586", NfmPlan = "CRXDEE_TenTen" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX591", NfmPlan = "CRX591" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX592", NfmPlan = "CRX592" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX593", NfmPlan = "CRX593" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX595", NfmPlan = "CRX595" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX598", NfmPlan = "RXSdHPOne" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX609", NfmPlan = "CRXSalesAdj" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX610", NfmPlan = "RXS1010_CRX610" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX631", NfmPlan = "CRX631" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX632", NfmPlan = "CRX632" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX633", NfmPlan = "CRX633" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX634", NfmPlan = "CRX634" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX635", NfmPlan = "CRX635" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX650", NfmPlan = "RXS1010_CRX650" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX703", NfmPlan = "RXS1010_CRX703" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX706", NfmPlan = "CRXCSNYM" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX712", NfmPlan = "RXS1010_CRX712" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX720", NfmPlan = "RXS1010_CRX720" });
            controlMRows.Add(new ControlMRow { JobNbr = "CRX811", NfmPlan = "CRXDEE_WorkComp" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN522", NfmPlan = "INN522" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN523", NfmPlan = "INN523" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN524", NfmPlan = "INN524" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN525", NfmPlan = "INN525" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN526", NfmPlan = "INN526" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN536", NfmPlan = "INN536" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN540", NfmPlan = "INN540" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN602", NfmPlan = "INN602" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN606", NfmPlan = "INN606" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN607", NfmPlan = "INN607" });
            controlMRows.Add(new ControlMRow { JobNbr = "INN702", NfmPlan = "INN702" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS129", NfmPlan = "RXSPCD" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS502", NfmPlan = "RXS1010_RXS502" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS505", NfmPlan = "SECXarGet" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS516", NfmPlan = "RXS1010_RXS516" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS516.12", NfmPlan = "RXS1010_RXS516" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS518", NfmPlan = "RXS1010_RXS518" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS524", NfmPlan = "RXS524" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS527", NfmPlan = "RXSIVRObCall" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS528", NfmPlan = "RXS1010_RXS528" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS529", NfmPlan = "RXSeTrueNGet" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS530", NfmPlan = "SECXarSD" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS540", NfmPlan = "RXS540" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS541", NfmPlan = "SECCF2Get" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS550", NfmPlan = "RXSCSRDaily" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS551", NfmPlan = "RXSCSRDlyNZR" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS552", NfmPlan = "RXSmsa" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS553", NfmPlan = "RXSOmniFlu" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS555.WEEKDAY", NfmPlan = "RXSCSRManual" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS564", NfmPlan = "RXS564" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS565", NfmPlan = "RXS565" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS571", NfmPlan = "RXS571" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS578", NfmPlan = "RXSWLBData" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS579", NfmPlan = "RXS1010_RXS579" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS580", NfmPlan = "RXSIVRMDFeed" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS582", NfmPlan = "RXSSTCcovid" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS587", NfmPlan = "RXSdSureScripts" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS588", NfmPlan = "RXSdEQUIP" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS589", NfmPlan = "RXSdSureScrGet" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS590", NfmPlan = "RXS590" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS611", NfmPlan = "RXS1010_RXS611" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS612", NfmPlan = "RXS1010_RXS612" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS615", NfmPlan = "RXS1010_RXS615" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS616", NfmPlan = "RXS1010_RXS616" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS618", NfmPlan = "RXS1010_RXS618" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS619", NfmPlan = "RXS1010_RXS619" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS620", NfmPlan = "RXS1010_RXS620" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS623", NfmPlan = "RXS1010_RXS623" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS624", NfmPlan = "RXS1010_RXS624" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS625", NfmPlan = "RXSXarTaxJns" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS644", NfmPlan = "RXS644" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS647", NfmPlan = "RXS647" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS648", NfmPlan = "RXS648" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS650", NfmPlan = "RXS1010_RXS650" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS653", NfmPlan = "RXS653" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS656", NfmPlan = "RXS656" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS660", NfmPlan = "RXS1010_RXS660" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS661", NfmPlan = "RXS1010_RXS661" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS666", NfmPlan = "RXS1010_RXS666" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS702", NfmPlan = "RXS1010_RXS702" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS711", NfmPlan = "RXS1010_Cleanup" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS720", NfmPlan = "RXSIVRDeceased" });
            controlMRows.Add(new ControlMRow { JobNbr = "RXS812", NfmPlan = "RXS812" });

            return controlMRows;
        }

        /// <inheritdoc />
        public IEnumerable<NfmPlanRow> PopulateNfmPlan()
        {
            List<NfmPlanRow> nfmPlanRows = new List<NfmPlanRow>();

            string nfmDataFilePath = "../../../../XXXDeveloperTools.NfmPlansAndFilesets/NfmData/";
            DirectoryInfo nfmDataDirectory = new DirectoryInfo(nfmDataFilePath);
            FileInfo[] nfmDataFiles = nfmDataDirectory.GetFiles("plndump.dat");
            List<NfmPlanRow> plansThatCallOtherPlans = new List<NfmPlanRow>();

            foreach (FileInfo nfmDataFile in nfmDataFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(nfmDataFilePath + nfmDataFile.Name));
                string[] lines = fileContents.Split("\n");

                foreach (string line in lines)
                {
                    string[] columns = line.Split(",");

                    if (columns.Count() == 47 &&
                        columns[1].Length >= 3 &&
                        columns[4] == "RUNPLAN")
                    {
                        NfmPlanRow nfmPlanRow = new NfmPlanRow
                        {
                            PlanName = columns[1],
                            FilesetName = string.Empty,
                            SourceNodeName = string.Empty,
                            TargetNodeName = string.Empty,
                            ParentPlanName = string.Empty,
                            ChildPlanNames = [columns[5]]
                        };

                        plansThatCallOtherPlans.Add(nfmPlanRow);
                    }
                }

                foreach (string line in lines)
                {
                    string[] columns = line.Split(",");

                    if (columns.Count() == 47 &&
                        columns[1].Length >= 3 &&
                        columns[4] == "TRANSFER")
                    {
                        string nfmPrefix = columns[1].Substring(0, 3);
                        if (!_filterNfmPrefixes.Contains(nfmPrefix))
                        {
                            continue;
                        }

                        string parentPlanName = string.Empty;
                        if (plansThatCallOtherPlans.Where(x => x.ChildPlanNames.Contains(columns[1])).Count() >= 1)
                        {
                            parentPlanName = plansThatCallOtherPlans.Where(x => x.ChildPlanNames.Contains(columns[1])).Select(x => x.PlanName).First();
                        }
                        NfmPlanRow nfmPlanRow = new NfmPlanRow
                        {
                            PlanName = columns[1],
                            FilesetName = columns[5],
                            SourceNodeName = columns[7],
                            TargetNodeName = columns[9],
                            ParentPlanName = parentPlanName,
                            ChildPlanNames = []
                        };

                        nfmPlanRows.Add(nfmPlanRow);
                    }
                }
            }

            return nfmPlanRows;
        }

        /// <inheritdoc />
        public IEnumerable<NfmFilesetRow> PopulateNfmFileset()
        {
            List<NfmFilesetRow> nfmFilesetRows = new List<NfmFilesetRow>();

            string nfmDataFilePath = "../../../../XXXDeveloperTools.NfmPlansAndFilesets/NfmData/";
            DirectoryInfo nfmDataDirectory = new DirectoryInfo(nfmDataFilePath);
            FileInfo[] nfmDataFiles = nfmDataDirectory.GetFiles("fsdump.dat");
            foreach (FileInfo nfmDataFile in nfmDataFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(nfmDataFilePath + nfmDataFile.Name));
                string[] lines = fileContents.Split("\n");
                string filesetName = string.Empty;
                string sourcePath = string.Empty;
                string targetPath = string.Empty;
                string sourceFileNamePattern = string.Empty;
                string targetFileNamePattern = string.Empty;
                DateTime? deploymentDate = default;
                string previousFilesetName = string.Empty;
                int previousSpecsId = 0;

                foreach (string line in lines)
                {
                    string[] columns = line.Split(",");

                    if (columns.Count() < 4)
                    {
                        continue;
                    }

                    if (!int.TryParse(columns[3], out int tempSpecsId))
                    {
                        continue;
                    }

                    if (tempSpecsId == 13)
                    {
                        continue;
                    }

                    if (tempSpecsId < previousSpecsId)
                    {
                        if (!string.IsNullOrEmpty(filesetName) &&
                            !string.IsNullOrEmpty(sourcePath) &&
                            !string.IsNullOrEmpty(targetPath) &&
                            !string.IsNullOrEmpty(sourceFileNamePattern))
                        {
                            NfmFilesetRow nfmFilesetRow = new NfmFilesetRow
                            {
                                FilesetName = filesetName,
                                SourcePath = sourcePath,
                                TargetPath = targetPath,
                                SourceFileNamePattern = sourceFileNamePattern,
                                TargetFileNamePattern = targetFileNamePattern,
                                DeploymentDate = deploymentDate
                            };
                            nfmFilesetRows.Add(nfmFilesetRow);
                        }
                    }

                    previousSpecsId = tempSpecsId;

                    if (columns.Count() == 10 && tempSpecsId == 1)
                    {
                        filesetName = columns[1];

                        if (previousFilesetName != filesetName)
                        {
                            targetPath = string.Empty;
                            sourceFileNamePattern = string.Empty;
                            targetFileNamePattern = string.Empty;
                            previousFilesetName = filesetName;
                        }

                        string nfmPrefix = columns[1].Substring(0, 3);
                        if (!_filterNfmPrefixes.Contains(nfmPrefix))
                        {
                            continue;
                        }

                        sourcePath = columns[6];
                        string tempDateString = columns[7].Substring(0, 10);
                        if (DateTime.TryParse(tempDateString, out DateTime tempDate))
                        {
                            deploymentDate = tempDate;
                        }
                        else
                        {
                            deploymentDate = default;
                        }

                        continue;
                    }

                    if (columns.Count() == 6 && tempSpecsId == 2)
                    {
                        targetPath = columns[4];
                        continue;
                    }

                    if (columns.Count() == 10 && tempSpecsId == 3)
                    {
                        sourceFileNamePattern = columns[5];
                        continue;
                    }

                    if (columns.Count() == 6 && tempSpecsId == 4)
                    {
                        targetFileNamePattern = columns[4];
                    }
                }

                if (!string.IsNullOrEmpty(filesetName) &&
                    !string.IsNullOrEmpty(sourcePath) &&
                    !string.IsNullOrEmpty(targetPath) &&
                    !string.IsNullOrEmpty(sourceFileNamePattern))
                {
                    NfmFilesetRow nfmFilesetRow = new NfmFilesetRow
                    {
                        FilesetName = filesetName,
                        SourcePath = sourcePath,
                        TargetPath = targetPath,
                        SourceFileNamePattern = sourceFileNamePattern,
                        TargetFileNamePattern = targetFileNamePattern,
                        DeploymentDate = deploymentDate
                    };
                    nfmFilesetRows.Add(nfmFilesetRow);
                }
            }

            return nfmFilesetRows;
        }

        /// <inheritdoc />
        public IEnumerable<PgpLogRow> PopulatePgpLog()
        {
            List<PgpLogRow> pgpLogRows = new List<PgpLogRow>();

            string pgpLogFilePath = "../../../../XXXDeveloperTools.NfmPlansAndFilesets/ProductionTpsprodLogs/";
            DirectoryInfo pgpLogDirectory = new DirectoryInfo(pgpLogFilePath);
            FileInfo[] pgpLogFiles = pgpLogDirectory.GetFiles("PharmacyGrep*_*.txt");

            foreach (FileInfo pgpLogFile in pgpLogFiles)
            {
                string fileContents = File.ReadAllText(Path.Combine(pgpLogFilePath + pgpLogFile.Name));
                string[] lines = fileContents.Split("\n");

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    string[] columns = line.Split(":");

                    //Get date from log file name.
                    string pgpLogFileName = columns[0].Trim();
                    string[] pgpLogFileDateString = pgpLogFileName.Split(".");
                    DateTime pgpLogDate = DateTime.ParseExact(pgpLogFileDateString[2].Trim(), "yyMMdd", System.Globalization.CultureInfo.InvariantCulture);

                    //Get pgp action from the grep of log files.
                    string pgpLogActionString = columns[1].Trim();

                    //Get the pgp file name from the grep of log files.
                    string pgpFileNameAndPath = columns[2].Trim();
                    string pgpFileName = Path.GetFileName(pgpFileNameAndPath);
                    string[] pgpFilePathParts = pgpFileNameAndPath.Split("/");
                    string pgpFilePath = string.Join("/", pgpFilePathParts.SkipLast(1)) + "/";

                    PgpLogRow pgpLogRow = new PgpLogRow
                    {
                        IsDecryption = pgpLogActionString.Contains("decrypt"),
                        PgpFileName = pgpFileName,
                        PgpFilePath = pgpFilePath,
                        PgpDate = pgpLogDate
                    };

                    pgpLogRows.Add(pgpLogRow);
                }
            }

            return pgpLogRows;
        }
    }
}
