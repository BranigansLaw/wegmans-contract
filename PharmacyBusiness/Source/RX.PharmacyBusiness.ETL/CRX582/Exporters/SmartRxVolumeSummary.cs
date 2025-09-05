namespace RX.PharmacyBusiness.ETL.CRX582.Exporters
{
    using RX.PharmacyBusiness.ETL.CRX582.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;

    public class SmartRxVolumeSummary
    {
        private const string fileDelimiter = "|";
        private string outputFileLocation;
        private IFileManager fileManager { get; set; }
        private FileHelper fileHelper;

        public SmartRxVolumeSummary(string outputFileLocation)
        {
            this.outputFileLocation = outputFileLocation;
            this.fileManager = this.fileManager ?? new FileManager();
            this.fileHelper = new FileHelper(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public void Export(ICollection<ThirdPartyClaimRecord> records, DateTime runDate)
        {
            var output = new StringBuilder();
            var fields = new Collection<string>();

            output.AppendLine("SmartRx Volume Summary");
            output.AppendLine(string.Format("Claims Date: {0:MM/dd/yyyy}", runDate.AddDays(-1)));
            output.AppendLine("");
            output.AppendLine(CreateHeaderRecord());

            var summaryRecords =
                records.GroupBy(r => new { r.OriginatingStoreId })
                     .Select(
                         g =>
                         new
                         {
                             Store = g.First().OriginatingStoreId,
                             TxCount = g.Count(),
                             ClaimCount = (g.Count() + g.Count(r => string.IsNullOrEmpty(r.CardholderId_2) == false) + g.Count(r => string.IsNullOrEmpty(r.CardholderId_3) == false)),
                             CoPay = g.Sum(r => ((r.CopayAmount_1 ?? 0) + (r.CopayAmount_2 ?? 0) + (r.CopayAmount_3 ?? 0))),
                             InsPays = g.Sum(r => ((r.AdjudicatedAmount_1 ?? 0) + (r.AdjudicatedAmount_2 ?? 0) + (r.AdjudicatedAmount_3 ?? 0))),
                             Alert = g.Count() == 0 ? "*Missing transactions*" : ""
                         })
                     .ToList();
            int totalTxCount = 0;
            int totalClaimCount = 0;
            decimal totalCoPay = 0;
            decimal totalInsPays = 0;

            foreach (var summaryRecord in summaryRecords.OrderBy(x => x.Store))
            {
                totalTxCount += summaryRecord.TxCount;
                totalClaimCount += summaryRecord.ClaimCount;
                totalCoPay += summaryRecord.CoPay;
                totalInsPays += summaryRecord.InsPays;

                fields.Clear();

                fields.Add(summaryRecord.Store);
                fields.Add(summaryRecord.TxCount.ToString());
                fields.Add(summaryRecord.ClaimCount.ToString());
                fields.Add(summaryRecord.CoPay.ToString("0.00"));
                fields.Add(summaryRecord.InsPays.ToString("0.00"));
                fields.Add(summaryRecord.Alert);

                output.AppendLine(string.Join(fileDelimiter, fields.ToArray()));
            }

            fields.Clear();

            fields.Add("Totals");
            fields.Add(totalTxCount.ToString());
            fields.Add(totalClaimCount.ToString());
            fields.Add(totalCoPay.ToString("0.00"));
            fields.Add(totalInsPays.ToString("0.00"));
            fields.Add("");

            output.AppendLine(string.Join(fileDelimiter, fields.ToArray()));

            this.fileManager.WriteAllText(this.GetFileName(runDate), output.ToString());
            this.fileHelper.CopyFileToArchiveForQA(this.GetFileName(runDate));
        }

        private static string CreateHeaderRecord()
        {
            return
                string.Format("Store{0}TxCount{0}ClaimCount{0}CoPay{0}InsPays{0}Alert", fileDelimiter);
        }

        private string GetFileName(DateTime runDate)
        {
            return Path.Combine(this.outputFileLocation, string.Format("SmartRx_VolSummary_{0:yyyyMMdd}.txt", runDate));
        }
    }
}
