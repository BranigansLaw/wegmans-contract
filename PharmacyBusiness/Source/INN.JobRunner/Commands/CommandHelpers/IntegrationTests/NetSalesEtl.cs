using Library.NetezzaInterface.DataModel;
using Library.TenTenInterface.UploadsToTenTen;
using System.Reflection;

namespace INN.JobRunner.Commands.CommandHelpers.IntegrationTests
{
    public class NetSalesEtl : TenTenEtl<NetSaleRow>
    {
        private const string _tenTenTableName = "{0}.netsales";
        private const string _tenTenColumnContainingDataDate = "date_sold";
        private const int _daysBetweenDataDateAndRunForDate = -1;
        private const bool _doesUploadReplaceAllData = false;
        private const string _tenTenTableDefinition = @"<title>Net Sales</title>
  <sdesc>Pharmacy Net Sales</sdesc>
  <cols>
    <th name=""store_num"" type=""i"" format=""type:nocommas;width:3"">Store Number</th>
    <th name=""date_sold"" type=""i"" format=""type:date;width:10"">Date Sold</th>
    <th name=""display_time"" type=""a"" format=""type:char;width:10"">Display Time</th>
    <th name=""tx_num"" type=""a"" format=""type:char;width:7"">Transaction Number</th>
    <th name=""reg_num"" type=""a"" format=""type:char;width:3"">Register Number</th>
    <th name=""netsales_amt"" type=""f"" format=""type:num;dec:2;width:11"">Net Sales Amount</th>
    <th name=""gp_amt"" type=""f"" format=""type:num;dec:2;width:11"">GP Amount</th>
    <th name=""qty"" type=""f"" format=""type:num;dec:2;width:5"">Qty</th>
    <th name=""net_cnt"" type=""f"" format=""type:num;dec:2;width:5"">Net Item Count</th>
    <th name=""item_num"" type=""a"" format=""type:char;width:7"">Item Number</th>
    <th name=""item_desc"" type=""a"" format=""type:char;width:40"">Item Description</th>
    <th name=""dept_num"" type=""a"" format=""type:char;width:3"">Department Number</th>
    <th name=""dept_name"" type=""a"" format=""type:char;width:15"">Department Name</th>
    <th name=""pl_name"" type=""a"" format=""type:char;width:15"">PL Department Name</th>
    <th name=""store_name"" type=""a"" format=""type:char;width:30"">Store Name</th>
    <th name=""reg_desc"" type=""a"" format=""type:char;width:20"">Register Description</th>
    <th name=""cash_num"" type=""a"" format=""type:char;width:7"">Cashier Number</th>
    <th name=""pl_code"" type=""a"" format=""type:char;width:2"">PL Department Code</th>
    <th name=""coupon_desc_wp"" type=""a"" format=""type:char;width:7"">Coupon Description WP</th>
    <th name=""coupon_desc_mfg"" type=""a"" format=""type:char;width:13"">Coupon Description MFG</th>
    <th name=""refund_amt"" type=""f"" format=""type:num;dec:2;width:12"">Refund Amount</th>
    <th name=""tender_amt"" type=""f"" format=""type:num;dec:2;width:12"">Tender Amount</th>
    <th name=""tender_type"" type=""a"" format=""type:char;width:3"">Tender Type</th>
    <th name=""tender_type_desc"" type=""a"" format=""type:char;width:20"">Tender Type Description</th>
    <th name=""tx_type"" type=""a"" format=""type:char;width:2"">Tx Type Code</th>
    <th name=""tx_type_desc"" type=""a"" format=""type:char;width:20"">Tx Type Description</th>
    <th name=""tender_desc"" type=""a"" format=""type:char;width:20"">Tender Status Description</th>
  </cols>";

        private DateOnly _runFor;
        private string _tenTenTablePath;

        public override string EtlName
        {
            get
            {
                MethodBase? currentMethod = MethodBase.GetCurrentMethod();
                if (currentMethod == null)
                {
                    throw new Exception("MethodBase.GetCurrentMethod returned null");
                }

                Type? declaringType = currentMethod.DeclaringType;
                if (declaringType == null)
                {
                    throw new Exception("MethodBase.GetCurrentMethod.DeclaringType returned null");
                }

                return declaringType.Name;
            }
        }

        public override TenTenTableSpecifications TenTenTableSpecs
        {
            get
            {
                return new TenTenTableSpecifications(
                    uploadTableNameAndPath: string.Format(_tenTenTableName, _tenTenTablePath), //Some tables have Run For dates as part of their table names, which makes reruns easy sincewe can replace that entire table with each rerun.
                    doesUploadReplaceAllData: _doesUploadReplaceAllData,
                    queryableTableNameAndPath: string.Format(_tenTenTableName, _tenTenTablePath), //For netsales the upload table is the queryable table. Tables with Run For Dates in their table names are typically merged into a table by another name.
                    tenTenTableDefinition: _tenTenTableDefinition.Replace("\n", "").Replace("\r", "")
                )
                {
                    ColumnNameContainingDataDate = _tenTenColumnContainingDataDate,
                    DataDateAssociatedWithRunForDate = _runFor.AddDays(_daysBetweenDataDateAndRunForDate)
                };
            }
        }

        public NetSalesEtl(DateOnly runFor, string environment) : base(runFor)
        {
            _runFor = runFor;

            //TODO: This is a placeholder. The actual table path will be determined by the environment.
            switch (environment)
            {
                case "PROD":
                    _tenTenTablePath = "wegmans.wegmansdata.dwfeeds";
                    break;
                case "UAT":
                    _tenTenTablePath = "wegmans.devpharm.uat";
                    break;
                case "DEV":
                    _tenTenTablePath = "wegmans.devpharm.chrism.demos";
                    break;
                default:
                    throw new ArgumentException("Invalid environment");
            }
        }
    }
}
