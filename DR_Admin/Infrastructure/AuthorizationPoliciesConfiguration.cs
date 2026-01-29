using Microsoft.AspNetCore.Authorization;

namespace ISPAdmin.Infrastructure;

public static class AuthorizationPoliciesConfiguration
{
    public static void ConfigureAuthorizationPolicies(this AuthorizationOptions options)
    {
        // Admin policies
        options.AddPolicy("Admin.Only", policy =>
            policy.RequireRole("Admin"));

        // BillingCycle policies
        options.AddPolicy("BillingCycle.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("BillingCycle.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("BillingCycle.Write", policy =>
            policy.RequireRole("Admin"));

        // ContactPerson policies
        options.AddPolicy("ContactPerson.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ContactPerson.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("ContactPerson.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // ControlPanelType policies
        options.AddPolicy("ControlPanelType.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ControlPanelType.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("ControlPanelType.Write", policy =>
            policy.RequireRole("Admin"));

        // Country policies
        options.AddPolicy("Country.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Country.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Country.Write", policy =>
            policy.RequireRole("Admin"));

        // Coupon policies
        options.AddPolicy("Coupon.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Coupon.Read", policy =>
            policy.RequireRole("Admin", "Sales"));

        options.AddPolicy("Coupon.Write", policy =>
            policy.RequireRole("Admin"));

        // Currency policies
        options.AddPolicy("Currency.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Currency.Read", policy =>
            policy.RequireRole("Admin", "Finance"));

        options.AddPolicy("Currency.Write", policy =>
            policy.RequireRole("Admin", "Finance"));

        // Customer policies
        options.AddPolicy("Customer.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Customer.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Customer.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // CustomerCredit policies
        options.AddPolicy("CustomerCredit.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("CustomerCredit.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("CustomerCredit.Write", policy =>
            policy.RequireRole("Admin"));

        // CustomerPaymentMethod policies
        options.AddPolicy("CustomerPaymentMethod.Delete", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("CustomerPaymentMethod.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("CustomerPaymentMethod.Write", policy =>
            policy.RequireRole("Admin", "Support"));

        // CustomerStatus policies
        options.AddPolicy("CustomerStatus.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("CustomerStatus.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("CustomerStatus.Write", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        // DnsRecord policies
        options.AddPolicy("DnsRecord.Delete", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("DnsRecord.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("DnsRecord.ReadOwn", policy =>
            policy.RequireRole("Admin", "Support", "Customer"));

        options.AddPolicy("DnsRecord.Write", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("DnsRecord.WriteOwn", policy =>
            policy.RequireRole("Admin", "Support", "Customer"));

        // DnsRecordType policies
        options.AddPolicy("DnsRecordType.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("DnsRecordType.Read", policy =>
            policy.RequireRole("Admin", "Support", "Customer"));

        options.AddPolicy("DnsRecordType.Write", policy =>
            policy.RequireRole("Admin"));

        // DnsZonePackage policies
        options.AddPolicy("DnsZonePackage.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("DnsZonePackage.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("DnsZonePackage.Write", policy =>
            policy.RequireRole("Admin"));

        // DnsZonePackageRecord policies
        options.AddPolicy("DnsZonePackageRecord.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("DnsZonePackageRecord.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("DnsZonePackageRecord.Write", policy =>
            policy.RequireRole("Admin"));

        // DocumentTemplate policies
        options.AddPolicy("DocumentTemplate.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("DocumentTemplate.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("DocumentTemplate.Write", policy =>
            policy.RequireRole("Admin"));

        // EmailQueue policies
        options.AddPolicy("EmailQueue.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("EmailQueue.Write", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        // ExchangeRateDownloadLog policies
        options.AddPolicy("ExchangeRateDownloadLog.Read", policy =>
            policy.RequireRole("Admin", "Finance"));

        // HostingPackage policies
        options.AddPolicy("HostingPackage.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("HostingPackage.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("HostingPackage.Write", policy =>
            policy.RequireRole("Admin"));

        // Invoice policies
        options.AddPolicy("Invoice.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Invoice.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Invoice.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // InvoiceLine policies
        options.AddPolicy("InvoiceLine.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("InvoiceLine.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("InvoiceLine.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // Order policies
        options.AddPolicy("Order.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Order.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Order.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // PaymentGateway policies
        options.AddPolicy("PaymentGateway.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("PaymentGateway.Read", policy =>
            policy.RequireRole("Admin", "Support", "Customer"));

        options.AddPolicy("PaymentGateway.Write", policy =>
            policy.RequireRole("Admin"));

        // PaymentIntent policies
        options.AddPolicy("PaymentIntent.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("PaymentIntent.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("PaymentIntent.Write", policy =>
            policy.RequireRole("Admin", "Support", "Customer"));

        // PostalCode policies
        options.AddPolicy("PostalCode.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("PostalCode.Read", policy =>
            policy.RequireRole("Admin", "Support", "Customer"));

        options.AddPolicy("PostalCode.Write", policy =>
            policy.RequireRole("Admin"));

        // Quote policies
        options.AddPolicy("Quote.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Quote.Read", policy =>
            policy.RequireRole("Admin", "Sales", "Support"));

        options.AddPolicy("Quote.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // Refund policies
        options.AddPolicy("Refund.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Refund.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("Refund.Write", policy =>
            policy.RequireRole("Admin"));

        // Registrar policies
        options.AddPolicy("Registrar.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Registrar.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales", "Customer"));

        options.AddPolicy("Registrar.Write", policy =>
            policy.RequireRole("Admin"));

        // RegistrarTld policies
        options.AddPolicy("RegistrarTld.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("RegistrarTld.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales", "Customer"));

        options.AddPolicy("RegistrarTld.Write", policy =>
            policy.RequireRole("Admin"));

        // ResellerCompany policies
        options.AddPolicy("ResellerCompany.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ResellerCompany.Read", policy =>
            policy.RequireRole("Admin", "Sales"));

        options.AddPolicy("ResellerCompany.Write", policy =>
            policy.RequireRole("Admin"));

        // Role policies
        options.AddPolicy("Role.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Role.Read", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Role.Write", policy =>
            policy.RequireRole("Admin"));

        // SalesAgent policies
        options.AddPolicy("SalesAgent.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("SalesAgent.Read", policy =>
            policy.RequireRole("Admin", "Sales"));

        options.AddPolicy("SalesAgent.Write", policy =>
            policy.RequireRole("Admin"));

        // SentEmail policies
        options.AddPolicy("SentEmail.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        // Server policies
        options.AddPolicy("Server.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Server.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("Server.Write", policy =>
            policy.RequireRole("Admin"));

        // ServerControlPanel policies
        options.AddPolicy("ServerControlPanel.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ServerControlPanel.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("ServerControlPanel.Write", policy =>
            policy.RequireRole("Admin"));

        // ServerIpAddress policies
        options.AddPolicy("ServerIpAddress.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ServerIpAddress.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("ServerIpAddress.Write", policy =>
            policy.RequireRole("Admin"));

        // Service policies
        options.AddPolicy("Service.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Service.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Service.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // ServiceType policies
        options.AddPolicy("ServiceType.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("ServiceType.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("ServiceType.Write", policy =>
            policy.RequireRole("Admin"));

        // Subscription policies
        options.AddPolicy("Subscription.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Subscription.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Subscription.Write", policy =>
            policy.RequireRole("Admin", "Sales"));

        // SubscriptionBillingHistory policies
        options.AddPolicy("SubscriptionBillingHistory.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        // TaxRule policies
        options.AddPolicy("TaxRule.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("TaxRule.Read", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("TaxRule.Write", policy =>
            policy.RequireRole("Admin"));

        // Tld policies
        options.AddPolicy("Tld.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Tld.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales", "Customer"));

        options.AddPolicy("Tld.Write", policy =>
            policy.RequireRole("Admin"));

        // Token policies
        options.AddPolicy("Token.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Token.Read", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Token.Write", policy =>
            policy.RequireRole("Admin"));

        // Unit policies
        options.AddPolicy("Unit.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("Unit.Read", policy =>
            policy.RequireRole("Admin", "Support", "Sales"));

        options.AddPolicy("Unit.Write", policy =>
            policy.RequireRole("Admin"));

        // User policies
        options.AddPolicy("User.Delete", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("User.Read", policy =>
            policy.RequireRole("Admin", "Support"));

        options.AddPolicy("User.Write", policy =>
            policy.RequireRole("Admin", "Support"));
    }
}
