using Library.SnowflakeInterface.Data;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<DurConflictTenTenRow> MapFromDurConflictRow(IEnumerable<DurConflictRow> durConflictRows)
        {
            return durConflictRows.Select(d => new DurConflictTenTenRow
            {
                ConflictCode = d.ConflictCode,
                ConflictDesc = d.ConflictDesc,
                ConflictType = d.ConflictType,
                DeaClass = d.DeaClass,
                DrugName = d.DrugName,
                DurDate = d.DurDate,
                Gcn = d.Gcn,
                GcnSequenceNumber = d.GcnSequenceNumber,
                IsCritical = d.IsCritical,
                IsException = d.IsException,
                LevelOfEffort = d.LevelOfEffort,
                NdcWo = d.NdcWo,
                PartSeqNumber = d.PartSeqNumber,
                PatientNumber = d.PatientNumber,
                PrescriberKey = d.PrescriberKey,
                ProfService = d.ProfService,
                ReasonForService = d.ReasonForService,
                RefillNumber = d.RefillNumber,
                ResultOfService = d.ResultOfService,
                RxFillSequence = d.RxFillSequence,
                RxNumber = d.RxNumber,
                RxRecordNumber = d.RxRecordNumber,
                Sdgi = d.Sdgi,
                SeverityDesc = d.SeverityDesc,
                StoreNumber = d.StoreNumber,
                UserKey = d.UserKey,
            });
        }
    }
}
