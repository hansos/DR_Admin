using System;
using System.Collections.Generic;

namespace ReportGeneratorLib.Models
{
    public class OfferDocumentDto
    {
        public int? QuoteId { get; set; }

        public SellerInfoDto? Seller { get; set; }

        public SaleContextDto? SaleContext { get; set; }

        public OfferSettingsDto? OfferSettings { get; set; }

        public List<OfferLineItemDto> LineItems { get; set; } = [];

        public OfferTotalsDto? Totals { get; set; }
    }

    public class SellerInfoDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? ContactPerson { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? PostalCode { get; set; }

        public string? CountryCode { get; set; }

        public string? CompanyRegistrationNumber { get; set; }

        public string? VatNumber { get; set; }
    }

    public class SaleContextDto
    {
        public string? DomainName { get; set; }

        public string? FlowType { get; set; }

        public CustomerSummaryDto? Customer { get; set; }

        public string? Currency { get; set; }
    }

    public class CustomerSummaryDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? CustomerName { get; set; }

        public string? Email { get; set; }
    }

    public class OfferSettingsDto
    {
        public DateOnly? ValidUntil { get; set; }

        public string? CouponCode { get; set; }

        public decimal? DiscountPercent { get; set; }

        public string? Notes { get; set; }

        public DateTime? SentAt { get; set; }

        public DateTime? AcceptedAt { get; set; }
    }

    public class OfferLineItemDto
    {
        public int LineNumber { get; set; }

        public int? ServiceId { get; set; }

        public string? Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }

        public string? Type { get; set; }
    }

    public class OfferTotalsDto
    {
        public int LineCount { get; set; }

        public decimal OneTimeSubtotal { get; set; }

        public decimal RecurringSubtotal { get; set; }

        public decimal GrandTotal { get; set; }
    }
}
