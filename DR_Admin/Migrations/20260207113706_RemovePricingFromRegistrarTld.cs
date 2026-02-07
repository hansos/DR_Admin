using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RemovePricingFromRegistrarTld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove pricing columns from RegistrarTlds table
            // Pricing is now managed in separate temporal tables:
            // - RegistrarTldCostPricing (for costs from registrar)
            // - TldSalesPricing (for sales prices to customers)
            
            migrationBuilder.DropColumn(
                name: "RegistrationCost",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "RegistrationPrice",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "RenewalCost",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "RenewalPrice",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "TransferCost",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "TransferPrice",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "PrivacyCost",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "PrivacyPrice",
                table: "RegistrarTlds");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "RegistrarTlds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore pricing columns if migration is rolled back
            migrationBuilder.AddColumn<decimal>(
                name: "RegistrationCost",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RegistrationPrice",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RenewalCost",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RenewalPrice",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferCost",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferPrice",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrivacyCost",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrivacyPrice",
                table: "RegistrarTlds",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "RegistrarTlds",
                type: "TEXT",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");
        }
    }
}
