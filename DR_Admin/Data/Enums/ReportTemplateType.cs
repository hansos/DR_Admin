namespace ISPAdmin.Data.Enums;

/// <summary>
/// Represents the type/category of report template
/// </summary>
public enum ReportTemplateType
{
    /// <summary>
    /// Template for invoice reports
    /// </summary>
    Invoice = 0,
    
    /// <summary>
    /// Template for customer list reports
    /// </summary>
    CustomerList = 1,
    
    /// <summary>
    /// Template for sales reports
    /// </summary>
    SalesReport = 2,
    
    /// <summary>
    /// Template for domain reports
    /// </summary>
    DomainReport = 3,
    
    /// <summary>
    /// Template for hosting reports
    /// </summary>
    HostingReport = 4,
    
    /// <summary>
    /// Template for financial reports
    /// </summary>
    FinancialReport = 5,
    
    /// <summary>
    /// Template for subscription reports
    /// </summary>
    SubscriptionReport = 6,
    
    /// <summary>
    /// Template for order reports
    /// </summary>
    OrderReport = 7,
    
    /// <summary>
    /// Template for custom reports
    /// </summary>
    Custom = 99
}
