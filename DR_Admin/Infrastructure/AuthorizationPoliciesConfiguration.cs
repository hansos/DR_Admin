using Microsoft.AspNetCore.Authorization;

namespace ISPAdmin.Infrastructure;

public static class AuthorizationPoliciesConfiguration
{
    private const string ADMIN = "Admin";
    private const string SUPPORT = "Support";
    private const string SALES = "Sales";
    private const string FINANCE = "Finance";
    private const string CUSTOMER = "Customer";

    public static void ConfigureAuthorizationPolicies(this AuthorizationOptions options)
    {
        // Admin policies
        options.AddPolicy("Admin.Only", policy =>
            policy.RequireRole(ADMIN));

        // BillingCycle policies
        options.AddPolicy("BillingCycle.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("BillingCycle.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("BillingCycle.Write", policy =>
            policy.RequireRole(ADMIN));

        // ContactPerson policies
        options.AddPolicy("ContactPerson.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("ContactPerson.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("ContactPerson.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // ControlPanelType policies
        options.AddPolicy("ControlPanelType.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("ControlPanelType.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("ControlPanelType.Write", policy =>
            policy.RequireRole(ADMIN));

        // Country policies
        options.AddPolicy("Country.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Country.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Country.Write", policy =>
            policy.RequireRole(ADMIN));

        // Coupon policies
        options.AddPolicy("Coupon.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Coupon.Read", policy =>
            policy.RequireRole(ADMIN, SALES));

        options.AddPolicy("Coupon.Write", policy =>
            policy.RequireRole(ADMIN));

        // Currency policies
        options.AddPolicy("Currency.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Currency.Read", policy =>
            policy.RequireRole(ADMIN, FINANCE));

        options.AddPolicy("Currency.Write", policy =>
            policy.RequireRole(ADMIN, FINANCE));

        // Customer policies
        options.AddPolicy("Customer.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Customer.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Customer.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // CustomerCredit policies
        options.AddPolicy("CustomerCredit.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("CustomerCredit.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("CustomerCredit.Write", policy =>
            policy.RequireRole(ADMIN));

        // CustomerPaymentMethod policies
        options.AddPolicy("CustomerPaymentMethod.Delete", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("CustomerPaymentMethod.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("CustomerPaymentMethod.Write", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        // CustomerStatus policies
        options.AddPolicy("CustomerStatus.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("CustomerStatus.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("CustomerStatus.Write", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        // DnsRecord policies
        options.AddPolicy("DnsRecord.Delete", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("DnsRecord.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("DnsRecord.ReadOwn", policy =>
            policy.RequireRole(ADMIN, SUPPORT, CUSTOMER));

        options.AddPolicy("DnsRecord.Write", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("DnsRecord.WriteOwn", policy =>
            policy.RequireRole(ADMIN, SUPPORT, CUSTOMER));

        // DnsRecordType policies
        options.AddPolicy("DnsRecordType.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("DnsRecordType.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, CUSTOMER));

        options.AddPolicy("DnsRecordType.Write", policy =>
            policy.RequireRole(ADMIN));

        // DnsZonePackage policies
        options.AddPolicy("DnsZonePackage.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("DnsZonePackage.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("DnsZonePackage.Write", policy =>
            policy.RequireRole(ADMIN));

        // DnsZonePackageRecord policies
        options.AddPolicy("DnsZonePackageRecord.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("DnsZonePackageRecord.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("DnsZonePackageRecord.Write", policy =>
            policy.RequireRole(ADMIN));

        // DocumentTemplate policies
        options.AddPolicy("DocumentTemplate.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("DocumentTemplate.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("DocumentTemplate.Write", policy =>
            policy.RequireRole(ADMIN));

        // EmailQueue policies
        options.AddPolicy("EmailQueue.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("EmailQueue.Write", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        // ExchangeRateDownloadLog policies
        options.AddPolicy("ExchangeRateDownloadLog.Read", policy =>
            policy.RequireRole(ADMIN, FINANCE));

        // HostingPackage policies
        options.AddPolicy("HostingPackage.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("HostingPackage.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("HostingPackage.Write", policy =>
            policy.RequireRole(ADMIN));

        // Invoice policies
        options.AddPolicy("Invoice.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Invoice.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Invoice.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // InvoiceLine policies
        options.AddPolicy("InvoiceLine.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("InvoiceLine.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("InvoiceLine.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // Order policies
        options.AddPolicy("Order.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Order.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Order.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // PaymentGateway policies
        options.AddPolicy("PaymentGateway.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("PaymentGateway.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, CUSTOMER));

        options.AddPolicy("PaymentGateway.Write", policy =>
            policy.RequireRole(ADMIN));

        // PaymentIntent policies
        options.AddPolicy("PaymentIntent.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("PaymentIntent.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("PaymentIntent.Write", policy =>
            policy.RequireRole(ADMIN, SUPPORT, CUSTOMER));

        // PostalCode policies
        options.AddPolicy("PostalCode.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("PostalCode.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, CUSTOMER));

        options.AddPolicy("PostalCode.Write", policy =>
            policy.RequireRole(ADMIN));

        // Quote policies
        options.AddPolicy("Quote.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Quote.Read", policy =>
            policy.RequireRole(ADMIN, SALES, SUPPORT));

        options.AddPolicy("Quote.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // Refund policies
        options.AddPolicy("Refund.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Refund.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("Refund.Write", policy =>
            policy.RequireRole(ADMIN));

        // Registrar policies
        options.AddPolicy("Registrar.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Registrar.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES, CUSTOMER));

        options.AddPolicy("Registrar.Write", policy =>
            policy.RequireRole(ADMIN));

        // RegistrarTld policies
        options.AddPolicy("RegistrarTld.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("RegistrarTld.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES, CUSTOMER));

        options.AddPolicy("RegistrarTld.Write", policy =>
            policy.RequireRole(ADMIN));

        // ResellerCompany policies
        options.AddPolicy("ResellerCompany.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("ResellerCompany.Read", policy =>
            policy.RequireRole(ADMIN, SALES));

        options.AddPolicy("ResellerCompany.Write", policy =>
            policy.RequireRole(ADMIN));

        // Role policies
        options.AddPolicy("Role.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Role.Read", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Role.Write", policy =>
            policy.RequireRole(ADMIN));

        // SalesAgent policies
        options.AddPolicy("SalesAgent.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("SalesAgent.Read", policy =>
            policy.RequireRole(ADMIN, SALES));

        options.AddPolicy("SalesAgent.Write", policy =>
            policy.RequireRole(ADMIN));

        // SentEmail policies
        options.AddPolicy("SentEmail.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        // Server policies
        options.AddPolicy("Server.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Server.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("Server.Write", policy =>
            policy.RequireRole(ADMIN));

        // ServerControlPanel policies
        options.AddPolicy("ServerControlPanel.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("ServerControlPanel.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("ServerControlPanel.Write", policy =>
            policy.RequireRole(ADMIN));

        // ServerIpAddress policies
        options.AddPolicy("ServerIpAddress.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("ServerIpAddress.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("ServerIpAddress.Write", policy =>
            policy.RequireRole(ADMIN));

        // Service policies
        options.AddPolicy("Service.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Service.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Service.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // ServiceType policies
        options.AddPolicy("ServiceType.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("ServiceType.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("ServiceType.Write", policy =>
            policy.RequireRole(ADMIN));

        // Subscription policies
        options.AddPolicy("Subscription.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Subscription.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Subscription.Write", policy =>
            policy.RequireRole(ADMIN, SALES));

        // SubscriptionBillingHistory policies
        options.AddPolicy("SubscriptionBillingHistory.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        // TaxRule policies
        options.AddPolicy("TaxRule.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("TaxRule.Read", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("TaxRule.Write", policy =>
            policy.RequireRole(ADMIN));

        // Tld policies
        options.AddPolicy("Tld.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Tld.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES, CUSTOMER));

        options.AddPolicy("Tld.Write", policy =>
            policy.RequireRole(ADMIN));

        // Token policies
        options.AddPolicy("Token.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Token.Read", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Token.Write", policy =>
            policy.RequireRole(ADMIN));

        // Unit policies
        options.AddPolicy("Unit.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("Unit.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT, SALES));

        options.AddPolicy("Unit.Write", policy =>
            policy.RequireRole(ADMIN));

        // User policies
        options.AddPolicy("User.Delete", policy =>
            policy.RequireRole(ADMIN));

        options.AddPolicy("User.Read", policy =>
            policy.RequireRole(ADMIN, SUPPORT));

        options.AddPolicy("User.Write", policy =>
            policy.RequireRole(ADMIN, SUPPORT));
    }
}
