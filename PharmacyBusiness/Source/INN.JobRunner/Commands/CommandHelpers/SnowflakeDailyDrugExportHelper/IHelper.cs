namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper;

public interface IHelper
{
    string? DeriveDecile(string? groupName,
                long? pPrdProductKey,
                long? pgPrdProductKey,
                string? pgMemberStatus,
                long? gsGrpGroupNumber);

    string? DeriveOtcType(string? groupName,
                long? pPrdProductKey,
                long? pgPrdProductKey,
                string? pgMemberStatus,
                long? gsGrpGroupNumber);

    string? DeriveCostBasisEffDate(decimal? rawPcfrCost,
        decimal? rawPcfcCost,
        decimal? rawPcfnCost,
        string? userDeffEffectiveEndDate,
        string? nteEffectiveEndDate,
        string? conEffectiveEndDate,
        string? repackConEffectiveEndDate);
}
