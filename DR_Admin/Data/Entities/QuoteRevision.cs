using ISPAdmin.Data.Enums;

namespace ISPAdmin.Data.Entities;

/// <summary>
/// Stores immutable revisions/snapshots for a quote after important actions like send or print
/// </summary>
public class QuoteRevision : EntityBase
{
    /// <summary>
    /// Foreign key to the parent quote
    /// </summary>
    public int QuoteId { get; set; }

    /// <summary>
    /// Incremental revision number per quote
    /// </summary>
    public int RevisionNumber { get; set; }

    /// <summary>
    /// Quote status at the time of snapshot
    /// </summary>
    public QuoteStatus QuoteStatus { get; set; }

    /// <summary>
    /// Action that created this snapshot, for example "Printed", "Sent", or "Updated"
    /// </summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>
    /// Serialized JSON snapshot used for document generation
    /// </summary>
    public string SnapshotJson { get; set; } = string.Empty;

    /// <summary>
    /// Generated PDF file name
    /// </summary>
    public string PdfFileName { get; set; } = string.Empty;

    /// <summary>
    /// Full generated PDF file path
    /// </summary>
    public string PdfFilePath { get; set; } = string.Empty;

    /// <summary>
    /// Optional content hash for integrity checks
    /// </summary>
    public string ContentHash { get; set; } = string.Empty;

    /// <summary>
    /// Optional comment for this revision
    /// </summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>
    /// Soft delete timestamp
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Navigation property to quote
    /// </summary>
    public Quote Quote { get; set; } = null!;
}
