namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type/category of document template
/// </summary>
public enum DocumentTemplateType
{
    /// <summary>
    /// Template for invoices
    /// </summary>
    Invoice = 0,
    
    /// <summary>
    /// Template for orders
    /// </summary>
    Order = 1,
    
    /// <summary>
    /// Template for outgoing emails
    /// </summary>
    Email = 2,
    
    /// <summary>
    /// Template for quotes/proposals
    /// </summary>
    Quote = 3,
    
    /// <summary>
    /// Template for contracts
    /// </summary>
    Contract = 4,
    
    /// <summary>
    /// Template for receipts
    /// </summary>
    Receipt = 5,
    
    /// <summary>
    /// Template for delivery notes
    /// </summary>
    DeliveryNote = 6,
    
    /// <summary>
    /// Other template types
    /// </summary>
    Other = 99
}
