using Library.LibraryUtilities.DataFileWriter;
using Library.SnowflakeInterface;
using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using Library.TenTenInterface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper
{
    public class SnowflakeToTenTenDataSftpExportHelperImp : ISnowflakeToTenTenDataSftpExportHelper
    {
        private readonly ISnowflakeInterface _snowflakeInterface;
        private readonly ITenTenInterface _tenTenInterface;
        private readonly IDataFileWriter _dataFileWriter;
        private readonly IMapper _mapper;
        private readonly ILogger<SnowflakeToTenTenDataSftpExportHelperImp> _logger;
        private readonly IOptions<SnowflakeDataOutputDirectories> _snowflakeDataOutputDirectoriesOptions;

        public SnowflakeToTenTenDataSftpExportHelperImp(
            ISnowflakeInterface snowflakeInterface,
            ITenTenInterface tenTenInterface,
            IDataFileWriter dataFileWriter,
            IMapper mapper,
            ILogger<SnowflakeToTenTenDataSftpExportHelperImp> logger,
            IOptions<SnowflakeDataOutputDirectories> snowflakeDataOutputDirectoriesOptions)
        {
            _snowflakeInterface = snowflakeInterface ?? throw new ArgumentNullException(nameof(snowflakeInterface));
            _tenTenInterface = tenTenInterface ?? throw new ArgumentNullException(nameof(tenTenInterface));
            _dataFileWriter = dataFileWriter ?? throw new ArgumentNullException(nameof(dataFileWriter));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snowflakeDataOutputDirectoriesOptions = snowflakeDataOutputDirectoriesOptions ?? throw new ArgumentNullException(nameof(snowflakeDataOutputDirectoriesOptions));
        }

        /// <inheritdoc />
        public async Task ExportDurConflictAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake DurConflict data");
            IEnumerable<DurConflictRow> durConflictData = await _snowflakeInterface.QuerySnowflakeAsync(
                new DurConflictQuery
                {
                    RunDate = runFor
                }, c).ConfigureAwait(false);

            _logger.LogInformation($"Returned {durConflictData.Count()} rows");

            await _tenTenInterface.UploadDataAsync(
                _mapper.MapFromDurConflictRow(durConflictData), 
                c, 
                runFor).ConfigureAwait(false);

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_DurConflict.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(durConflictData, new DataFileWriterConfig<DurConflictRow>
            {
                Header = "StoreNumber|RxNumber|RefillNumber|PartSeqNumber|DurDate|PatientNumber|NdcWo|DrugName|Sdgi|Gcn|GcnSequenceNumber|DeaClass|ConflictCode|ConflictDesc|ConflictType|SeverityDesc|ResultOfService|ProfService|LevelOfEffort|ReasonForService|IsCritical|IsException|RxFillSequence|RxRecordNumber|PrescriberKey|UserKey",
                WriteDataLine = (c) =>
                    $"{c.StoreNumber}|{c.RxNumber}|{c.RefillNumber}|{c.PartSeqNumber}|{c.DurDate}|{c.PatientNumber}|{c.NdcWo}|{c.DrugName}|{c.Sdgi}|{c.Gcn}|{c.GcnSequenceNumber}|{c.DeaClass}|{c.ConflictCode}|{c.ConflictDesc}|{c.ConflictType}|{c.SeverityDesc}|{c.ResultOfService}|{c.ProfService}|{c.LevelOfEffort}|{c.ReasonForService}|{c.IsCritical}|{c.IsException}|{c.RxFillSequence}|{c.RxRecordNumber}|{c.PrescriberKey}|{c.UserKey}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc/>
        public async Task ExportInvAdjAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Exporting InvAdjustment");
            IEnumerable<InvAdjustmentRow> InvAdjustment = await _snowflakeInterface.QuerySnowflakeAsync(
                new InvAdjustmentQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {InvAdjustment.Count()} records");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_InvAdj.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(InvAdjustment, new DataFileWriterConfig<InvAdjustmentRow>
            {
                Header = "DATE_KEY|STORE_NUM|DRUG_LABEL_NAME|DRUG_NDC|ADJUSTMENT_QUANTITY|ADJUSTMENT_UNIT_COST|ADJUSTMENT_EXTENDED_COST|ADJUSTMENT_TYPE_CODE|INVENTORY_ADJUSTMENT_NUM|ADJUSTMENT_REASON_CODE|DESCRIPTION|SYS_USER_FNAME|SYS_USER_LNAME|SYS_USER_KEY|NDC|REF_NUM",
                WriteDataLine = (InvAdjustmentRow c) =>
                $"{c.DateKey}|" +
                $"{c.StoreNumber}|" +
                $"{c.DrugLabelName}|" +
                $"{c.DrugNDC}|" +
                $"{c.AdjustmentQuantity}|" +
                $"{c.AdjustmentCost}|" +
                $"{c.AdjustmentExtendedCost}|" +
                $"{c.AdjustmentTypeCode}|" +
                $"{c.InventoryAdjustmentNumber}|" +
                $"{c.AdjustmentReasonCode}|" +
                $"{c.Description}|" +
                $"{c.SystemUserFirstName}|" +
                $"{c.SystemUserLastName}|" +
                $"{c.SystemUserKey}|" +
                $"{c.NDC}|" +
                $"{c.ReferenceNumber}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc/>
        public async Task ExportPetPtNumsAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Exporting PetPtNums");
            IEnumerable<PetPtNumRow> petPtNums = await _snowflakeInterface.QuerySnowflakeAsync(new PetPtNumsQuery { }, c);

            _logger.LogInformation($"Returned {petPtNums.Count()} records");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_PetPtNums.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(petPtNums, new DataFileWriterConfig<PetPtNumRow>
            {
                Header = "PATIENT_NUM|SPECIES|CREATE_DATE|PET",
                WriteDataLine = (PetPtNumRow c) => $"{c.PatientNum}|{c.Species}|{c.CreateDate}|{c.Pet}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportPrescriberAddressAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake perscriber address data");
            IEnumerable<PrescriberAddressRow> prescriberAddressData = await _snowflakeInterface.QuerySnowflakeAsync(
                new PrescriberAddressQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {prescriberAddressData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_PrescriberAddress.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(prescriberAddressData, new DataFileWriterConfig<PrescriberAddressRow>
            {
                Header = "AddrCity|AddressOne|AddressTwo|AddrIdNum|AddrStatus|AddrType|AddSource|AreaCodeFax|AreaCodeOther|AreaCodeOther2|AreaCodePrim|AreaCodeSec|County|DeaBilling|DeaExpireDate|DeaNum|DrugSched|Email|ExtFax|ExtOther|ExtOther2|ExtPrim|ExtSec|IsDefault|LastUpdate|LicenseState|Npi|NpiBilling|NpiExpireDate|PadrKey|PhoneNumFax|PhoneNumOther|PhoneNumOther2|PhoneNumPrim|PhoneNumSec|PhoneTypeOther2|PracticeName|PrefContact|PrescribAddrNum|PrescriberKey|State|StateLicBilling|StateLicExpireDate|StateLicNum|WebAddr|Zipcode|ZipExt",
                WriteDataLine = (c) => $"{c.AddrCity}|{c.AddressOne}|{c.AddressTwo}|{c.AddrIdNum}|{c.AddrStatus}|{c.AddrType}|{c.AddSource}|{c.AreaCodeFax}|{c.AreaCodeOther}|{c.AreaCodeOther2}|{c.AreaCodePrim}|{c.AreaCodeSec}|{c.County}|{c.DeaBilling}|{c.DeaExpireDate}|{c.DeaNum}|{c.DrugSched}|{c.Email}|{c.ExtFax}|{c.ExtOther}|{c.ExtOther2}|{c.ExtPrim}|{c.ExtSec}|{c.IsDefault}|{c.LastUpdate}|{c.LicenseState}|{c.Npi}|{c.NpiBilling}|{c.NpiExpireDate}|{c.PadrKey}|{c.PhoneNumFax}|{c.PhoneNumOther}|{c.PhoneNumOther2}|{c.PhoneNumPrim}|{c.PhoneNumSec}|{c.PhoneTypeOther2}|{c.PracticeName}|{c.PrefContact}|{c.PrescribAddrNum}|{c.PrescriberKey}|{c.State}|{c.StateLicBilling}|{c.StateLicExpireDate}|{c.StateLicNum}|{c.WebAddr}|{c.Zipcode}|{c.ZipExt}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportPrescribersAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake perscriber data");
            IEnumerable<PrescriberRow> prescriberData = await _snowflakeInterface.QuerySnowflakeAsync(
                new PrescriberQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {prescriberData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_Prescriber.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(prescriberData, new DataFileWriterConfig<PrescriberRow>
            {
                Header = "ActivePrescriberNum|AddSource|AmaActivity|BirthDate|CreateDate|CreateUserKey|DeaBilling|DeaExpireDate|DeaNum|DeceasedDate|EsvMatch|FedtaxidNum|FirstName|GenderCode|InactiveCode|InactiveDate|IsEsvValid|LastName|LastUpdate|LicenseState|MedicaidNum|MiddleName|NarcdeaNum|NcpdpNum|Npi|NpiBilling|NpiExpireDate|PpiEnabled|PrefGeneric|PrefTherSub|PrescriberId|PrescriberKey|PrescriberNum|StateissueNum|StateLicBilling|StateLicExpireDate|StateLicNum|Status|SuffixAbbr|SupvPrsNum|TitleAbbr|UniqPrsNum",
                WriteDataLine = (c) => $"{c.ActivePrescriberNum}|{c.AddSource}|{c.AmaActivity}|{c.BirthDate}|{c.CreateDate}|{c.CreateUserKey}|{c.DeaBilling}|{c.DeaExpireDate}|{c.DeaNum}|{c.DeceasedDate}|{c.EsvMatch}|{c.FedtaxidNum}|{c.FirstName}|{c.GenderCode}|{c.InactiveCode}|{c.InactiveDate}|{c.IsEsvValid}|{c.LastName}|{c.LastUpdate}|{c.LicenseState}|{c.MedicaidNum}|{c.MiddleName}|{c.NarcdeaNum}|{c.NcpdpNum}|{c.Npi}|{c.NpiBilling}|{c.NpiExpireDate}|{c.PpiEnabled}|{c.PrefGeneric}|{c.PrefTherSub}|{c.PrescriberId}|{c.PrescriberKey}|{c.PrescriberNum}|{c.StateissueNum}|{c.StateLicBilling}|{c.StateLicExpireDate}|{c.StateLicNum}|{c.Status}|{c.SuffixAbbr}|{c.SupvPrsNum}|{c.TitleAbbr}|{c.UniqPrsNum}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc />
        public async Task ExportRxTransferAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake Rx Transfer data");

            IEnumerable<RxTransferRow> rxTransferData = await _snowflakeInterface.QuerySnowflakeAsync(
                new RxTransferQuery
                {
                    RunDate = runFor
                }, c);

            _logger.LogInformation($"Returned {rxTransferData.Count()} rows");

            _logger.LogInformation("Write records to file");
            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_RxTransfer.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(rxTransferData, new DataFileWriterConfig<RxTransferRow>
            {
                Header = "BASE_STORE_NUM|BASE_STORE_NAME|TO_STORE_NUM|TO_STORE|FROM_STORE_NUM|FROM_STORE|TRANSFER_DEST|PATIENT_NUM|ORIG_RX_NUM|REFILL_NUM|TRANSFER_DATE|SOLD_DATE|READY_DATE|CANCEL_DATE|TRANSFER_TIME_KEY|WRITTEN_NDC_WO|WRITTEN_DRUG_NAME|DISP_NDC_WO|DISP_DRUG_NAME|QTY_DISPENSED|DAW|TRANS_TYPE|TRANS_METHOD|SIG_TEXT|PRESCRIBER_KEY|RX_RECORD_NUM|RX_FILL_SEQ|PATIENT_PAY|TP_PAY|TX_PRICE|ACQ_COST|U_C_PRICE|FIRST_FILL_DATE|LAST_FILL_DATE|SENDING_RPH|RECEIVE_RPH|XFER_ADDR|XFER_ADDRE|XFER_CITY|XFER_ST|XFER_ZIP|XFER_PHONE|TRANSFER_REASON|NEW_RX_RECORD_NUM|COMPETITOR_GROUP|COMPETITOR_STORE_NUM",
                WriteDataLine = (c) =>
                $"{c.BaseStoreNum}|" +
                $"{c.BaseStoreName}|" +
                $"{c.ToStoreNum}|" +
                $"{c.ToStore}|" +
                $"{c.FromStoreNum}|" +
                $"{c.FromStore}|" +
                $"{c.TransferDest}|" +
                $"{c.PatientNum}|" +
                $"{c.OrigRxNum}|" +
                $"{c.RefillNum}|" +
                $"{c.TransferDate}|" +
                $"{c.SoldDate}|" +
                $"{c.ReadyDate}|" +
                $"{c.CancelDate}|" +
                $"{c.TransferTimeKey}|" +
                $"{c.WrittenNdcWo}|" +
                $"{c.WrittenDrugName}|" +
                $"{c.DispNdcWo}|" +
                $"{c.DispDrugName}|" +
                $"{c.QtyDispensed}|" +
                $"{c.Daw}|" +
                $"{c.TransType}|" +
                $"{c.TransMethod}|" +
                $"{c.SigText}|" +
                $"{c.PrescriberKey}|" +
                $"{c.RxRecordNum}|" +
                $"{c.RxFillSeq}|" +
                $"{c.PatientPay}|" +
                $"{c.TpPay}|" +
                $"{c.TxPrice}|" +
                $"{c.AcqCos}|" +
                $"{c.UCPrice}|" +
                $"{c.FirstFillDate}|" +
                $"{c.LastFillDate}|" +
                $"{c.SendingRph}|" +
                $"{c.ReceiveRph}|" +
                $"{c.XferAddr}|" +
                $"{c.XferAddre}|" +
                $"{c.XferCity}|" +
                $"{c.XferSt}|" +
                $"{c.XferZip}|" +
                $"{c.XferPhone}|" +
                $"{c.TransferReason}|" +
                $"{c.NewRxRecordNum}|" +
                $"{c.CompetitorGroup}|" +
                $"{c.CompetitorStoreNum}",
                OutputFilePath = writePath,
            });
        }

        /// <inheritdoc/>
        public async Task ExportSupplierPriceDrugFileExportAsync(DateOnly runFor, CancellationToken c)
        {
            _logger.LogInformation("Gathering Snowflake Supplier Price Drug File data");

            IEnumerable<SupplierPriceDrugFileRow> supplierPriceDrugFileRows = await _snowflakeInterface.QuerySnowflakeAsync(
            new SupplierPriceDrugFileQuery
            {
                RunDate = runFor
            }, c);
            _logger.LogInformation($"Returned {supplierPriceDrugFileRows.Count()} rows");

            string writePath = _snowflakeDataOutputDirectoriesOptions.Value.INN544_SupplierPriceDrugFile.Replace("yyyyMMdd", DateTime.Now.ToString("yyyyMMdd"));
            _logger.LogInformation($"Writing to path: {writePath}");
            await _dataFileWriter.WriteDataToFileAsync(supplierPriceDrugFileRows, new DataFileWriterConfig<SupplierPriceDrugFileRow>
            {
                Header = "DATE_OF_SERVICE|SUPPLIER_NAME|VENDOR_ITEM_NUMBER|NDC|NDC_WO|DRUG_NAME|DRUG_FORM|DEA_CLASS|STRENGTH|GENERIC_NAME|PACK_SIZE|IS_MAINT_DRUG|SDGI|GCN|GCN_SEQ_NUM|DRUG_MANUFACTURER|ORANGE_BOOK_CODE|PACK_PRICE|PRICE_PER_UNIT|UNIT_PRICE_DATE|PURCH_INCR|PKG_SIZE_INCR|STATUS|EFF_START_DATE|",
                WriteDataLine = (SupplierPriceDrugFileRow c) => $"{c.DateOfService}|" +
                $"{c.SupplierName}|" +
                $"{c.VendorItemNumber}|" +
                $"{c.Ndc}|" +
                $"{c.NdcWo}|" +
                $"{c.DrugName}|" +
                $"{c.DrugForm}|" +
                $"{c.DeaClass}|" +
                $"{c.Strength}|" +
                $"{c.StrengthUnit}|" +
                $"{c.GenericName}|" +
                $"{c.PackSize}|" +
                $"{c.IsMaintDrug}|" +
                $"{c.Sdgi}|" +
                $"{c.Gcn}|" +
                $"{c.GcnSeqNumber}|" +
                $"{c.DrugManufacturer}|" +
                $"{c.OrangeBookCode}|" +
                $"{c.PackPrice}|" +
                $"{c.PricePerUnit}|" +
                $"{c.UnitPriceDate}|" +
                $"{c.PurchIncr}|" +
                $"{c.PkgSizeIncr}|" +
                $"{c.Status}|" +
                $"{c.EffStartDate}|",
                OutputFilePath = writePath,
            });
        }
    }
}
