using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;
using Library.McKessonCPSInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportImmunizationsFromMcKessonHelper
{
    public interface IExportImmunizationHelper
    {
        string? GetStoreProductNumberWhereNdcwoEqualsStrProductNdc(IEnumerable<NdcConversionRow> ndcConversionRows, ImmunizationRow immunizationRow);

        string? GetStoreProductNdcWhereNdcwoEqualsStrProductNdc(IEnumerable<NdcConversionRow> ndcConversionRows, ImmunizationRow immunizationRow);

        string? GetRaceRecipOneWherePatientNumberEqualsDecInternalPtNum(IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows, ImmunizationRow immunizationRow);

        string? GetRecipEthnicityWherePatientNumberEqualsDecInternalPtNum(IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows, ImmunizationRow immunizationRow);

        string? GetNyPriorityGroupWherePatientNumberEqualsDecInternalPtNum(IEnumerable<ImmunizationQuestionnaireRow> immunizationQuestionnaireRows, ImmunizationRow immunizationRow);

        string? GetPrimaryLanguage(ImmunizationRow immunizationRow);

        string? GetRecipEthnicity(ImmunizationRow immunizationRow);

        string? GetRecipRaceOne(ImmunizationRow immunizationRow); 
    }
}
