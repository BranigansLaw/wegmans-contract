using System.Collections.ObjectModel;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper;

public class HelperImp : IHelper
{

    private static readonly ReadOnlyCollection<long> _decileInValues = new([
        1000012,
        1000013,
        1000014,
        1000015,
        1000016,
        1000017,
        1000018,
        1000019,
        1000020,
        1000021,
        1000000,
        1000001,
        1000002,
        1000003,
        1000004,
        1000005,
        1000006,
        1000007,
        1000008,
        1000009]);

    private static readonly ReadOnlyCollection<long> _otcTypeInValues = new([1000010,
        1000011
        ]);

    public string? DeriveDecile(string? groupName,
                long? pPrdProductKey,
                long? pgPrdProductKey,
                string? pgMemberStatus,
                long? gsGrpGroupNumber)
    {
        if (!string.IsNullOrEmpty(groupName) &&
            !string.IsNullOrEmpty(pgMemberStatus) &&
            gsGrpGroupNumber.HasValue &&
            pgPrdProductKey == pPrdProductKey &&
            pgMemberStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) &&
            _decileInValues.Contains(gsGrpGroupNumber.Value))
        {
            return groupName.Substring(0, 1);
        }
        return null;
    }

    public string? DeriveOtcType(string? groupName,
                long? pPrdProductKey,
                long? pgPrdProductKey,
                string? pgMemberStatus,
                long? gsGrpGroupNumber)
    {
        if (!string.IsNullOrEmpty(groupName) &&
            !string.IsNullOrEmpty(pgMemberStatus) &&
            gsGrpGroupNumber.HasValue &&
            pgPrdProductKey == pPrdProductKey &&
            pgMemberStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) &&
            _otcTypeInValues.Contains(gsGrpGroupNumber.Value))
        {
            return groupName.Substring(0, 1);
        }
        return null;
    }

    public string? DeriveCostBasisEffDate(decimal? rawPcfrCost,
        decimal? rawPcfcCost,
        decimal? rawPcfnCost,
        string? userDeffEffectiveEndDate,
        string? nteEffectiveEndDate,
        string? conEffectiveEndDate,
        string? repackConEffectiveEndDate)
    {
        rawPcfrCost ??= 0;
        rawPcfcCost ??= 0;
        rawPcfnCost ??= 0;

        string? costBasisEffectiveDate = null;

        if (rawPcfrCost == 0)
        {
            if (rawPcfcCost == 0)
            {
                if (rawPcfnCost == 0)
                {
                    costBasisEffectiveDate = userDeffEffectiveEndDate;
                }
                else
                {
                    costBasisEffectiveDate = nteEffectiveEndDate;
                }
            }
            else
            {
                costBasisEffectiveDate = conEffectiveEndDate;
            }
        }
        else
        {
            costBasisEffectiveDate = repackConEffectiveEndDate;
        }
        
        return costBasisEffectiveDate;
    }
}