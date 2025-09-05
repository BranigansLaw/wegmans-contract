namespace Library.EmplifiInterface.DataModel;

public class JpapStatusRow
{
    [ExportHeaderColumnLabel("PHARM_CODE")]
    public required string PharmacyCode { get; set; }

    [ExportHeaderColumnLabel("PHARM_NPI")]
    public required int PharmacyNpi { get; set; }

    [ExportHeaderColumnLabel("SP_TRANSACTION_ID")]
    public required string SpTransactionId { get; set; }

    [ExportHeaderColumnLabel("PROGRAM_ID")]
    public required string ProgramId { get; set; }

    [ExportHeaderColumnLabel("PATIENT_ID")]
    public required string PatientId { get; set; }

    [ExportHeaderColumnLabel("PAT_LAST_NAME")]
    public required string PatientLastName { get; set; }

    [ExportHeaderColumnLabel("PAT_FIRST_NAME")]
    public required string PatientFirstName { get; set; }

    [ExportHeaderColumnLabel("PAT_DOB")]
    public required int PatientDob { get; set; } //YYYYMMDD

    [ExportHeaderColumnLabel("BRAND")]
    public required string Brand { get; set; }

    [ExportHeaderColumnLabel("NDC_NUMBER")]
    public required long? NdcNumber { get; set; }

    [ExportHeaderColumnLabel("SHIP_DATE")]
    public int? ShipDate { get; set; } //YYYYMMDD

    [ExportHeaderColumnLabel("CARRIER")]
    public string? Carrier { get; set; }

    [ExportHeaderColumnLabel("TRACKING_NUM")]
    public string? TrackingNumber { get; set; }

    [ExportHeaderColumnLabel("QUANTITY")]
    public decimal? Quantity { get; set; }

    [ExportHeaderColumnLabel("DAY_SUPPLY")]
    public int? DaySupply { get; set; }

    [ExportHeaderColumnLabel("REFILLS_REMAIN")]
    public string? RefillsRemaining { get; set; }

    [ExportHeaderColumnLabel("PRES_LAST_NAME")]
    public required string PresLastName { get; set; }

    [ExportHeaderColumnLabel("PRES_FIRST_NAME")]
    public required string PresFirstName { get; set; }

    [ExportHeaderColumnLabel("PRES_NPI")]
    public required long PresNpi { get; set; }

    [ExportHeaderColumnLabel("PRES_DEA")]
    public string? PresDea { get; set; }

    [ExportHeaderColumnLabel("PRES_ADDR_1")]
    public required string PresAddr1 { get; set; }

    [ExportHeaderColumnLabel("PRES_ADDR_2")]
    public string? PresAddr2 { get; set; }

    [ExportHeaderColumnLabel("PRES_CITY")]
    public required string PresCity { get; set; }

    [ExportHeaderColumnLabel("PRES_STATE")]
    public required string PresState { get; set; }

    [ExportHeaderColumnLabel("PRES_ZIP")]
    public required int PresZip { get; set; }

    [ExportHeaderColumnLabel("PRES_PHONE")]
    public long? PresPhone { get; set; } //0123456789, no dashes

    [ExportHeaderColumnLabel("PRES_FAX")]
    public long? PresFax { get; set; }  // ^

    [ExportHeaderColumnLabel("DEMOGRAPHICID")]
    public required string DemographicId { get; set; }

    [ExportHeaderColumnLabel("CAREPATH_TRANSACTION_ID")]
    public required string CarePathTransactionId { get; set; }

    [ExportHeaderColumnLabel("SHIP_TO_LOCATION")]
    public string? ShipToLocation { get; set; }

    [ExportHeaderColumnLabel("SHIP_TO_ADDR1")]
    public string? ShipToAddress1 { get; set; }

    [ExportHeaderColumnLabel("SHIP_TO_ADDR2")]
    public string? ShipToAddress2 { get; set; }

    [ExportHeaderColumnLabel("SHIP_TO_CITY")]
    public string? ShipToCity { get; set; }

    [ExportHeaderColumnLabel("SHIP_TO_STATE")]
    public string? ShipToState { get; set; }

    [ExportHeaderColumnLabel("SHIP_TO_ZIP")]
    public int? ShipToZip { get; set; }

    [ExportHeaderColumnLabel("RESHIPMENT_FLAG")]
    public string? ReshipmentFlag { get; set; }

    [ExportHeaderColumnLabel("STATUS_DATE")]
    public required long StatusDate { get; set; } //YYYYMMddHHmmss

    [ExportHeaderColumnLabel("STATUS")]
    public required string Status { get; set; }

    [ExportHeaderColumnLabel("SUB_STATUS")]
    public required string SubStatus { get; set; }
}
