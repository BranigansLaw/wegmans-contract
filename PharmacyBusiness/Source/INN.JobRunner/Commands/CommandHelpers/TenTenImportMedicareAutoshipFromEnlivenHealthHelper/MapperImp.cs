using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportMedicareAutoshipFromEnlivenHealthHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<MedicareAutoship> MapToTenTenMedicareAutoship(IEnumerable<MedicareAutoshipRow> medicareAutoshipRows)
        {
            return medicareAutoshipRows.Select(r => new MedicareAutoship
            {
                PhoneNumber = r.PhoneNumber,
                DerivedFileNameDate = r.DerivedFileNameDate,
                DerivedProcessDate = r.DerivedProcessDate
            });
        }
    }
}
