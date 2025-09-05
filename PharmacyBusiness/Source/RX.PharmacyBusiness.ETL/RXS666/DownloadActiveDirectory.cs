namespace RX.PharmacyBusiness.ETL.RXS666
{
    using RX.PharmacyBusiness.ETL.RXS666.Core;
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS666", "Upload Active Directory list of Pharmacy Employees to 1010data.", "KBA00050334", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGBTASSAQ3V4ARD822PRC909K4B88")]
    public class DownloadActiveDirectory : ETLBase
    {
        private string ldapDomain = "wfm.wegmans.com";

        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();
            List<EmployeeRecord> employees = new List<EmployeeRecord>();
            List<string> activeDirectoryGroups = new List<string>();
            activeDirectoryGroups.Add("FIM - 1010Data");
            activeDirectoryGroups.Add("FIM - 1010Data Reporting");
            activeDirectoryGroups.Add("1010Data Exceptions");
            activeDirectoryGroups.Add("1010Data Reporting Exceptions");

            Log.LogInfo("Executing DownloadActiveDirectory.");

            foreach(var activeDirectoryGroup in activeDirectoryGroups)
            {
                employees.AddRange(SearchActiveDirectory(activeDirectoryGroup));
            }

            sqlServerHelper.WriteListToFile<EmployeeRecord>(
                employees,
                @"%BATCH_ROOT%\RXS666\Output\Active_Employees_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt",
                true,
                "|",
                string.Empty,
                false);

            Log.LogInfo("Finished running DownloadActiveDirectory with [{0}] total records in output file.", employees.Count);
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }

        protected List<EmployeeRecord> SearchActiveDirectory(string adGroupName)
        {
            Log.LogInfo("Searching Active Directory for Group Name [{0}].", adGroupName);
            List<EmployeeRecord> results = new List<EmployeeRecord>();

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, this.ldapDomain))
            using (GroupPrincipal group = GroupPrincipal.FindByIdentity(context, adGroupName))
            {
                foreach (Principal p in group.GetMembers())
                {
                    using (UserPrincipal user = UserPrincipal.FindByIdentity(p.Context, p.UserPrincipalName))
                    {
                        results.Add(new EmployeeRecord(adGroupName, p, user));
                    }
                }
            }

            Log.LogInfo("Found [{0}] employees in this AD Group.", results.Count);
            return results;
        }
    }
}
