using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddMulticurrencySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create CurrencyExchangeRates table
            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TargetCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Markup = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EffectiveRate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                });

            // Create index on currency pair and effective date for fast lookups
            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_BaseCurrency_TargetCurrency_EffectiveDate",
                table: "CurrencyExchangeRates",
                columns: new[] { "BaseCurrency", "TargetCurrency", "EffectiveDate" });

            // Create index on active rates
            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_IsActive",
                table: "CurrencyExchangeRates",
                column: "IsActive");

            // Add currency fields to Customers table
            migrationBuilder.AddColumn<string>(
                name: "PreferredCurrency",
                table: "Customers",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<bool>(
                name: "AllowCurrencyOverride",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: true);

            // Add currency fields to Invoices table
            migrationBuilder.AddColumn<string>(
                name: "BaseCurrencyCode",
                table: "Invoices",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<string>(
                name: "DisplayCurrencyCode",
                table: "Invoices",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Invoices",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseTotalAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true);

            // Add currency fields to Orders table
            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "Orders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<string>(
                name: "BaseCurrencyCode",
                table: "Orders",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "Orders",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExchangeRateDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            // Add currency fields to PaymentTransactions table
            migrationBuilder.AddColumn<string>(
                name: "BaseCurrencyCode",
                table: "PaymentTransactions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "PaymentTransactions",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseAmount",
                table: "PaymentTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            // Add currency fields to ResellerCompanies table
            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "ResellerCompanies",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "EUR");

            migrationBuilder.AddColumn<string>(
                name: "SupportedCurrencies",
                table: "ResellerCompanies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ApplyCurrencyMarkup",
                table: "ResellerCompanies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultCurrencyMarkup",
                table: "ResellerCompanies",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop CurrencyExchangeRates table
            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            // Remove currency fields from Customers table
            migrationBuilder.DropColumn(
                name: "PreferredCurrency",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AllowCurrencyOverride",
                table: "Customers");

            // Remove currency fields from Invoices table
            migrationBuilder.DropColumn(
                name: "BaseCurrencyCode",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DisplayCurrencyCode",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BaseTotalAmount",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "Invoices");

            // Remove currency fields from Orders table
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "BaseCurrencyCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExchangeRateDate",
                table: "Orders");

            // Remove currency fields from PaymentTransactions table
            migrationBuilder.DropColumn(
                name: "BaseCurrencyCode",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "BaseAmount",
                table: "PaymentTransactions");

            // Remove currency fields from ResellerCompanies table
            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "ResellerCompanies");

            migrationBuilder.DropColumn(
                name: "SupportedCurrencies",
                table: "ResellerCompanies");

            migrationBuilder.DropColumn(
                name: "ApplyCurrencyMarkup",
                table: "ResellerCompanies");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyMarkup",
                table: "ResellerCompanies");
        }
    }
}
