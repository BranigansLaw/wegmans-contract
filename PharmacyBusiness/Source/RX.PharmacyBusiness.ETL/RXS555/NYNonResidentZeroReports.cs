using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Wegmans.PharmacyLibrary.ETL;
using Wegmans.PharmacyLibrary.ETL.Core;
using Wegmans.PharmacyLibrary.IO;
using Wegmans.PharmacyLibrary.Logging;

namespace RX.PharmacyBusiness.ETL.RXS555
{
    public class NYNonResidentZeroReports : ETLBase
    {
        private IFileManager FileManager { get; set; }
        private ReturnCode returnCode = new ReturnCode();
        private readonly string filePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\RXS555\Output\");
        private const string fileNamePrefix = "ZeroReport_";
        private const string fileNameSuffix = "_NY.dat";

        private static readonly Dictionary<string, string> StoreMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "008", "PHA*1609823236*3111919*BW9719711*MT LAUREL*2 CENTERTON RD**MOUNT LAUREL*NJ*08054*8564397345*PHARMACIST*008" },
            { "010", "PHA*1407899016*3193644*BW9771987*CHERRY HILL*2100 ROUTE 70 WEST**CHERRY HILL*NJ*08002*8564882745*PHARMACIST*010" },
            { "032", "PHA*1548244643*3145679*BW8568884*WOODBRIDGE*15 WOODBRIDGE CENTER DR**WOODBRIDGE*NJ*07095*7325963245*PHARMACIST*032" },
            { "043", "PHA*1700110467*3992674*FW1649865*COLLEGEVILLE*600 COMMERCE DRIVE**COLLEGEVILLE*PA*19426*4849021545*PHARMACIST*043" },
            { "045", "PHA*1073724050*3988663*FW0359390*HARRISBURG*6416 CARLISLE PIKE STE 2000**MECHANICSBURG*PA*17050*7177914545*PHARMACIST*045" },
            { "046", "PHA*1558689380*3993880*FW1962100*MALVERN*50 FOUNDRY WAY**MALVERN*PA*19355*4849139645*PHARMACIST*046" },
            { "050", "PHA*1336123389*3980833*BW7942510*DOWNINGTOWN*1056 EAST LANCASTER AVE**DOWNINGTOWN*PA*19335*6105187845*PHARMACIST*050" },
            { "054", "PHA*1578854139*2135817*FW2571861*FREDERICK*7830 WORMANS MILL RD.**FREDERICK*MD*21701*2405757345*PHARMACIST*054" },
            { "059", "PHA*1336543834*2244781*FW4898651*BURLINGTON*53 THIRD AVENUE**BURLINGTON*MA*01803*7814180745*PHARMACIST*059" },
            { "069", "PHA*1831167014*3972660*BW5189952*ERIE WEST*5028 WEST RIDGE RD**ERIE*PA*16506*8148351910*PHARMACIST*069" },
            { "075", "PHA*1053380766*3966326*BW3615498*ERIE*6143 PEACH ST**ERIE*PA*16509*8148666580*PHARMACIST*075" },
            { "077", "PHA*1912976820*3968863*BW4197047*WILKES BARRE*220 HIGHLAND PARK BLVD**WILKES BARRE*PA*18702*5708257890*PHARMACIST*077" },
            { "078", "PHA*1780653758*3971303*BW4804349*WILLIAMSPORT*201 WILLIAM ST**WILLIAMSPORT*PA*17701*5703208787*PHARMACIST*078" },
            { "079", "PHA*1548239452*3974575*BW5967053*ALLENTOWN*3900 TILGHMAN ST**ALLENTOWN*PA*18104*6103367940*PHARMACIST*079" },
            { "093", "PHA*1851360473*3141378*BW6419077*PRINCETON*240 NASSAU PARK BLVD**PRINCETON*NJ*08540*6099199345*PHARMACIST*093" },
            { "095", "PHA*1619936721*3143423*BW7252276*MANALAPAN*55 US HWY RT 9 S. SUITE 100**MANALAPAN*NJ*07726*7326254145*PHARMACIST*095" },
            { "096", "PHA*1376512913*3142596*BW6986408*BRIDGEWATER*724 ROUTE 202 SOUTH**BRIDGEWATER*NJ*08807*9082439645*PHARMACIST*096" },
            { "097", "PHA*1265401814*3979513*BW7530505*BETHLEHEM*5000 WEGMANS DR**BETHLEHEM*PA*18017*6103171345*PHARMACIST*097" },
            { "105", "PHA*1255859658*3153208*FW7111735*MONTVALE*100 FARMVIEW**MONTVALE*NJ*07645*5512492145*PHARMACIST*105" },
            { "124", "PHA*1790104263*2244642*FW4468737*CHESTNUT HILL*200 BOYLSTON STREET**CHESTNUT HILL*MA*02467*6177622045*PHARMACIST*124" },
            { "125", "PHA*1518412246*2140464*FW6302082*OWINGS MILLS*10100 REISTERSTOWN RD**OWINGS MILLS*MD*21117*4434712345*PHARMACIST*125" }
        };

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            this.FileManager = this.FileManager ?? new FileManager();

            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());

            Log.LogInfo($"Creating NY non-resident zero reports for {runDate:yyyyMMdd}");

            int reportsWritten = 0;
            DateTime creationDate = DateTime.Now;
            string fillDate = $"{runDate.AddDays(-1):yyyyMMdd}";

            foreach (var store in StoreMap)
            {
                string transactionControlNumber = $"{creationDate:yyyyMMddHHmm}NR{store.Key}";
                string fileLinePrefix = $"TH*4.2*{transactionControlNumber}*01**{creationDate:yyyyMMdd}*{creationDate:HHmm}*P**\\\\IS*5854293883*Wegmans Food Markets*#{fillDate}#-#{fillDate}#\\";
                string fileLineSuffix = $"\\PAT*******Report*Zero***********\\DSP*****{fillDate}*****\\PRE**\\CDI*****\\AIR*\\TP*7\\TT*{transactionControlNumber}*10\\";

                var fileName = $"{fileNamePrefix}{runDate:yyyyMMdd}_{store.Key}{fileNameSuffix}"; // ZeroReport_20221002_199_NY.dat

                using (var streamWriter = new StreamWriter(Path.Combine(filePath, fileName), false))
                {
                    try
                    {
                        streamWriter.WriteLine($"{fileLinePrefix}{store.Value}{fileLineSuffix}");
                    }
                    finally
                    {
                        streamWriter.Flush();
                    }
                }

                reportsWritten++;
            }

            Log.LogInfo($"Completed with [{reportsWritten}] reports written");

            result = this.returnCode.IsSuccess ? 0 : 1;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}