using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportMedicareAutoshipFromEnlivenHealthHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps the collection of <see cref="MedicareAutoshipRow"/> to a collection of <see cref="MedicareAutoship"/>
        /// </summary>
        IEnumerable<MedicareAutoship> MapToTenTenMedicareAutoship(IEnumerable<MedicareAutoshipRow> medicareAutoshipRows);
    }
}
