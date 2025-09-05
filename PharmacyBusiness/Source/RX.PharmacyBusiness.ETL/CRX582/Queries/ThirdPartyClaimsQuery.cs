namespace RxAccounting.ThirdPartyClaims.Interfaces.ClaimsLoad.Queries
{
    using System;
    using Rx.Plumbing.CommandQuery;

    public class ThirdPartyClaimsQuery : IQuery
    {
        public DateTime RunDate { get; set; }
    }
}
