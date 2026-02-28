using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ReportGeneratorLib.Models;
using System;
using System.Linq;

namespace ReportGeneratorLib.Helpers
{
    public static class GenerateOfferPdfBytes
    {
        public static byte[] Create(OfferDocumentDto offer)
        {
            var saleContext = offer.SaleContext;
            var seller = offer.Seller;
            var offerSettings = offer.OfferSettings;
            var totals = offer.Totals;
            var currency = saleContext?.Currency ?? "USD";
            var lines = offer.LineItems
                .OrderBy(item => item.LineNumber)
                .ThenBy(item => item.Description)
                .ToList();

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Offer Report").SemiBold().FontSize(18);

                        if (!string.IsNullOrWhiteSpace(seller?.Name))
                        {
                            column.Item().Text(seller.Name).FontSize(12).SemiBold();
                        }

                        var sellerContact = string.Join(" · ", new[] { seller?.ContactPerson, seller?.Email, seller?.Phone }
                            .Where(value => !string.IsNullOrWhiteSpace(value))
                            .Select(value => value!));
                        if (!string.IsNullOrWhiteSpace(sellerContact))
                        {
                            column.Item().Text(sellerContact).FontSize(9);
                        }
                    });

                    page.Content().Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item().Text($"Domain: {saleContext?.DomainName ?? "-"}");
                                left.Item().Text($"Flow: {saleContext?.FlowType ?? "-"}");
                                left.Item().Text($"Customer: {saleContext?.Customer?.Name ?? saleContext?.Customer?.CustomerName ?? "-"}");
                            });

                            row.RelativeItem().Column(right =>
                            {
                                right.Item().Text($"Valid until: {offerSettings?.ValidUntil?.ToString("yyyy-MM-dd") ?? "-"}");
                                right.Item().Text($"Coupon: {offerSettings?.CouponCode ?? "-"}");
                                right.Item().Text($"Discount: {(offerSettings?.DiscountPercent ?? 0):0.##}%");
                            });
                        });

                        if (!string.IsNullOrWhiteSpace(offerSettings?.Notes))
                        {
                            column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                                .Text($"Notes: {offerSettings.Notes}").FontSize(10);
                        }

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35);
                                columns.RelativeColumn(4);
                                columns.ConstantColumn(45);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(70);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("#").SemiBold();
                                header.Cell().Text("Description").SemiBold();
                                header.Cell().AlignRight().Text("Qty").SemiBold();
                                header.Cell().AlignRight().Text("Unit").SemiBold();
                                header.Cell().AlignRight().Text("Subtotal").SemiBold();
                                header.Cell().Text("Type").SemiBold();
                            });

                            foreach (var line in lines)
                            {
                                table.Cell().Text(line.LineNumber > 0 ? line.LineNumber.ToString() : "-");
                                table.Cell().Text(line.Description ?? "-");
                                table.Cell().AlignRight().Text(line.Quantity.ToString("0.##"));
                                table.Cell().AlignRight().Text(FormatCurrency(line.UnitPrice, currency));
                                table.Cell().AlignRight().Text(FormatCurrency(line.Subtotal, currency));
                                table.Cell().Text(line.Type ?? "-");
                            }
                        });

                        column.Item().AlignRight().Column(totalColumn =>
                        {
                            totalColumn.Item().Text($"One-time: {FormatCurrency(totals?.OneTimeSubtotal ?? 0, currency)}");
                            totalColumn.Item().Text($"Recurring: {FormatCurrency(totals?.RecurringSubtotal ?? 0, currency)}");
                            totalColumn.Item().Text($"Grand total: {FormatCurrency(totals?.GrandTotal ?? 0, currency)}").SemiBold();
                            totalColumn.Item().Text($"Line count: {totals?.LineCount ?? lines.Count}");
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated at ");
                        text.Span(DateTime.UtcNow.ToString("u")).SemiBold();
                    });
                });
            }).GeneratePdf();
        }

        private static string FormatCurrency(decimal amount, string currency)
        {
            return $"{amount:0.00} {currency}";
        }
    }
}
