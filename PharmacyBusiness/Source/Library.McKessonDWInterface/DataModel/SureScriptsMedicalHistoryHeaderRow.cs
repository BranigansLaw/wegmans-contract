namespace Library.McKessonDWInterface.DataModel;

public class SureScriptsMedicalHistoryHeaderRow
{
    /// <summary>
    /// Record Type, an, 3/3
    /// </summary>
    public required string RecordType { get; set; }

    /// <summary>
    /// Version/Release Number, an, 3/3
    /// </summary>
    public required string VersionReleaseNumber { get; set; }

    /// <summary>
    /// SenderID [not used], an, 3/30
    /// </summary>
    public required string? SenderId { get; set; }

    /// <summary>
    /// Sender Participant Password [not used], an, 10/10
    /// </summary>
    public required string? SenderParticipantPassword { get; set; }

    /// <summary>
    /// ReceiverID, an, 1/30
    /// </summary>
    public required string ReceiverId { get; set; }

    /// <summary>
    /// Source Name [future use], an, 1/35
    /// </summary>
    public required string? SourceName { get; set; }

    /// <summary>
    /// Transmission Control Number [run date], an, 1/10
    /// </summary>
    public required string TransmissionControlNumber { get; set; }

    /// <summary>
    /// Transmission Date, dt, 8/8
    /// </summary>
    public required string TransmissionDate { get; set; }

    /// <summary>
    /// Transmission Time, tm, 8/8
    /// </summary>
    public required string TransmissionTime { get; set; }

    /// <summary>
    /// Transmission File Type, an, 1/3
    /// </summary>
    public required string TransmissionFileType { get; set; }

    /// <summary>
    /// Transmission Action, an, 1/1
    /// </summary>
    public required string TransmissionAction { get; set; }

    /// <summary>
    /// Extract Date, dt, 8/8
    /// </summary>
    public required string ExtractDate { get; set; }

    /// <summary>
    /// File Type [T=Test, P=Production], an, 1/1
    /// </summary>
    public required string FileType { get; set; }
}
