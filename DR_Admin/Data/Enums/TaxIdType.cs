namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type of tax identification number
/// </summary>
public enum TaxIdType
{
    /// <summary>
    /// No tax ID provided
    /// </summary>
    None = 0,
    
    /// <summary>
    /// EU VAT Identification Number
    /// </summary>
    VAT = 1,
    
    /// <summary>
    /// US Employer Identification Number
    /// </summary>
    EIN = 2,
    
    /// <summary>
    /// Australia Business Number
    /// </summary>
    ABN = 3,
    
    /// <summary>
    /// India/Canada GST Number
    /// </summary>
    GST = 4,
    
    /// <summary>
    /// Other tax identification type
    /// </summary>
    Other = 5
}
