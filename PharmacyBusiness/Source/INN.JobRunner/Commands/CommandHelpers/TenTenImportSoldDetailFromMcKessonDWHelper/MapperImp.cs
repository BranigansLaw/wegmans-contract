using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportSoldDetailFromMcKessonDWHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<SoldDetail> MapToTenTenSoldDetail(IEnumerable<SoldDetailRow> soldDetailRows)
        {
            return soldDetailRows.Select(r => new SoldDetail 
            { 
                StoreNbr = r.StoreNbr,
                RxNbr = r.RxNbr,
                RefillNbr = r.RefillNbr,
                PartialFillSequenceNbr = r.PartialFillSequenceNbr,
                SoldDate = r.SoldDate,
                OrderNbr = r.OrderNbr,
                QtyDispensed = r.QtyDispensed,
                NdcWithoutDashes = r.NdcWithoutDashes,
                AcquisitionCost = r.AcquisitionCost,
                ThirdPartyPay = r.ThirdPartyPay,
                PatientPay = r.PatientPay,
                TransactionPrice = r.TransactionPrice
            });
        }
    }
}
