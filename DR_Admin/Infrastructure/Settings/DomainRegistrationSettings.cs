namespace ISPAdmin.Infrastructure.Settings;

/// <summary>
/// Configuration settings for domain registration
/// </summary>
public class DomainRegistrationSettings
{
    /// <summary>
    /// Whether customer self-service registrations require admin/sales approval
    /// </summary>
    public bool RequireApprovalForCustomers { get; set; }
    
    /// <summary>
    /// Whether sales/admin registrations require additional approval
    /// </summary>
    public bool RequireApprovalForSales { get; set; }
    
    /// <summary>
    /// Whether customers are allowed to register domains themselves
    /// </summary>
    public bool AllowCustomerRegistration { get; set; } = true;
    
    /// <summary>
    /// Default registration period in years (used as initial value in UI)
    /// </summary>
    public int DefaultRegistrationYears { get; set; } = 1;
    
    /// <summary>
    /// Whether to check domain availability with registrar before registration
    /// </summary>
    public bool EnableAvailabilityCheck { get; set; }
    
    /// <summary>
    /// Whether to fetch real-time pricing from registrar
    /// </summary>
    public bool EnablePricingCheck { get; set; } = true;
}
