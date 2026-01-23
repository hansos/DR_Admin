namespace ISPAdmin.DTOs;

/// <summary>
/// Data transfer object representing a sent email
/// </summary>
public class SentEmailDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the sent email record
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the email was sent
    /// </summary>
    public DateTime SentDate { get; set; }
    
    /// <summary>
    /// Gets or sets the sender email address
    /// </summary>
    public string From { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the recipient email address(es)
    /// </summary>
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CC (carbon copy) email address(es)
    /// </summary>
    public string? Cc { get; set; }
    
    /// <summary>
    /// Gets or sets the BCC (blind carbon copy) email address(es)
    /// </summary>
    public string? Bcc { get; set; }
    
    /// <summary>
    /// Gets or sets the email subject line
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email body content
    /// </summary>
    public string? Body { get; set; }
    
    /// <summary>
    /// Gets or sets the unique message ID assigned by the mail server
    /// </summary>
    public string MessageId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email delivery status (e.g., Sent, Failed, Pending)
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Gets or sets any error message if the email failed to send
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Gets or sets the number of retry attempts
    /// </summary>
    public int? RetryCount { get; set; }
    
    /// <summary>
    /// Gets or sets the customer ID this email relates to
    /// </summary>
    public int? CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the user ID who initiated this email
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the type of related entity (e.g., Invoice, Order)
    /// </summary>
    public string? RelatedEntityType { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the related entity
    /// </summary>
    public int? RelatedEntityId { get; set; }
    
    /// <summary>
    /// Gets or sets information about email attachments
    /// </summary>
    public string? Attachments { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the date and time when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Data transfer object for creating a new sent email record
/// </summary>
public class CreateSentEmailDto
{
    /// <summary>
    /// Gets or sets the date and time when the email was sent
    /// </summary>
    public DateTime SentDate { get; set; }
    
    /// <summary>
    /// Gets or sets the sender email address
    /// </summary>
    public string From { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the recipient email address(es)
    /// </summary>
    public string To { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the CC (carbon copy) email address(es)
    /// </summary>
    public string? Cc { get; set; }
    
    /// <summary>
    /// Gets or sets the BCC (blind carbon copy) email address(es)
    /// </summary>
    public string? Bcc { get; set; }
    
    /// <summary>
    /// Gets or sets the email subject line
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email body content
    /// </summary>
    public string? Body { get; set; }
    
    /// <summary>
    /// Gets or sets the unique message ID assigned by the mail server
    /// </summary>
    public string MessageId { get; set; } = string.Empty;
    
    /// <summary>
    /// Gets or sets the email delivery status (e.g., Sent, Failed, Pending)
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Gets or sets any error message if the email failed to send
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Gets or sets the number of retry attempts
    /// </summary>
    public int? RetryCount { get; set; }
    
    /// <summary>
    /// Gets or sets the customer ID this email relates to
    /// </summary>
    public int? CustomerId { get; set; }
    
    /// <summary>
    /// Gets or sets the user ID who initiated this email
    /// </summary>
    public int? UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the type of related entity (e.g., Invoice, Order)
    /// </summary>
    public string? RelatedEntityType { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the related entity
    /// </summary>
    public int? RelatedEntityId { get; set; }
    
    /// <summary>
    /// Gets or sets information about email attachments
    /// </summary>
    public string? Attachments { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing sent email record
/// </summary>
public class UpdateSentEmailDto
{
    /// <summary>
    /// Gets or sets the email delivery status (e.g., Sent, Failed, Pending)
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// Gets or sets any error message if the email failed to send
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Gets or sets the number of retry attempts
    /// </summary>
    public int? RetryCount { get; set; }
}
