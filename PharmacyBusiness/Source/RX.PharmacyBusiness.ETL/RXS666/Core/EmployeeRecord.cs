namespace RX.PharmacyBusiness.ETL.RXS666.Core
{
    using System.DirectoryServices;
    using System.DirectoryServices.AccountManagement;

    public class EmployeeRecord
    {
        //NOTE: These property names will become header row in file to be TenUp'd to 1010data, and then also be column names in destination table.
        public string AD_Group_Name { get; set; }
        public string SAM_Account_Name { get; set; }
        public string Email_Or_User_Principal_Name { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Employee_ID { get; set; }
        public string Employee_Title { get; set; }
        public string Employee_Division { get; set; }
        public string Employee_Group_Memberships { get; set; }
        
        public EmployeeRecord(string adGroupName, Principal p, UserPrincipal user)
        {
            // NOTE: The data needs to be cleaned for TenUp from a file.
            //       No double quotes can be anywhere in the output file, not even if escaped.
            this.AD_Group_Name = TenTenHelper.CleanStringForTenUp(adGroupName);
            this.SAM_Account_Name = TenTenHelper.CleanStringForTenUp(p.SamAccountName);
            this.Email_Or_User_Principal_Name = TenTenHelper.CleanStringForTenUp((string.IsNullOrEmpty(user.EmailAddress) ? p.UserPrincipalName : user.EmailAddress));
            this.First_Name = TenTenHelper.CleanStringForTenUp(user.GivenName);
            this.Last_Name = TenTenHelper.CleanStringForTenUp(user.Surname);
            this.Employee_ID = TenTenHelper.CleanStringForTenUp(user.EmployeeId);
            this.Employee_Title = TenTenHelper.CleanStringForTenUp(GetProperty(user, "title"));
            this.Employee_Division = TenTenHelper.CleanStringForTenUp(GetProperty(user, "division"));
            this.Employee_Group_Memberships = TenTenHelper.CleanStringForTenUp(string.Join(",", user.GetGroups()));
        }

        /// <summary>
        /// Get a specific property based on the UserPrincipal object
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetProperty(UserPrincipal userPrincipal, string property)
        {
            DirectoryEntry d = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
            return d.Properties[property]?.Value?.ToString();
        }
    }
}
