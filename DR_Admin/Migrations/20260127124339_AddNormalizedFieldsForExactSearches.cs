using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedFieldsForExactSearches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedUsername",
                table: "Users",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedFirstName",
                table: "SalesAgents",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedLastName",
                table: "SalesAgents",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Registrars",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCity",
                table: "PostalCodes",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCode",
                table: "PostalCodes",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCountryCode",
                table: "PostalCodes",
                type: "TEXT",
                maxLength: 2,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedDistrict",
                table: "PostalCodes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedRegion",
                table: "PostalCodes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedState",
                table: "PostalCodes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "PaymentGateways",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "HostingPackages",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Domains",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCompanyName",
                table: "Customers",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedContactPerson",
                table: "Customers",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Customers",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Coupons",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEnglishName",
                table: "Countries",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedLocalName",
                table: "Countries",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUsername",
                table: "Users",
                column: "NormalizedUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_NormalizedFirstName_NormalizedLastName",
                table: "SalesAgents",
                columns: new[] { "NormalizedFirstName", "NormalizedLastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_NormalizedName",
                table: "Registrars",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_NormalizedCity",
                table: "PostalCodes",
                column: "NormalizedCity");

            migrationBuilder.CreateIndex(
                name: "IX_PostalCodes_NormalizedCode_NormalizedCountryCode",
                table: "PostalCodes",
                columns: new[] { "NormalizedCode", "NormalizedCountryCode" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_IsActive",
                table: "PaymentGateways",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_IsDefault",
                table: "PaymentGateways",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_NormalizedName",
                table: "PaymentGateways",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_ProviderCode",
                table: "PaymentGateways",
                column: "ProviderCode");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPackages_NormalizedName",
                table: "HostingPackages",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_NormalizedName",
                table: "Domains",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NormalizedCompanyName",
                table: "Customers",
                column: "NormalizedCompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NormalizedName",
                table: "Customers",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_IsActive",
                table: "Coupons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_NormalizedName",
                table: "Coupons",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ValidFrom",
                table: "Coupons",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_ValidUntil",
                table: "Coupons",
                column: "ValidUntil");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_NormalizedEnglishName",
                table: "Countries",
                column: "NormalizedEnglishName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_NormalizedUsername",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SalesAgents_NormalizedFirstName_NormalizedLastName",
                table: "SalesAgents");

            migrationBuilder.DropIndex(
                name: "IX_Registrars_NormalizedName",
                table: "Registrars");

            migrationBuilder.DropIndex(
                name: "IX_PostalCodes_NormalizedCity",
                table: "PostalCodes");

            migrationBuilder.DropIndex(
                name: "IX_PostalCodes_NormalizedCode_NormalizedCountryCode",
                table: "PostalCodes");

            migrationBuilder.DropIndex(
                name: "IX_PaymentGateways_IsActive",
                table: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_PaymentGateways_IsDefault",
                table: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_PaymentGateways_NormalizedName",
                table: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_PaymentGateways_ProviderCode",
                table: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_HostingPackages_NormalizedName",
                table: "HostingPackages");

            migrationBuilder.DropIndex(
                name: "IX_Domains_NormalizedName",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Customers_NormalizedCompanyName",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_NormalizedName",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_IsActive",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_NormalizedName",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_ValidFrom",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_ValidUntil",
                table: "Coupons");

            migrationBuilder.DropIndex(
                name: "IX_Countries_NormalizedEnglishName",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "NormalizedUsername",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NormalizedFirstName",
                table: "SalesAgents");

            migrationBuilder.DropColumn(
                name: "NormalizedLastName",
                table: "SalesAgents");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Registrars");

            migrationBuilder.DropColumn(
                name: "NormalizedCity",
                table: "PostalCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedCode",
                table: "PostalCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedCountryCode",
                table: "PostalCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedDistrict",
                table: "PostalCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedRegion",
                table: "PostalCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedState",
                table: "PostalCodes");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "PaymentGateways");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "HostingPackages");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "NormalizedCompanyName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NormalizedContactPerson",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "NormalizedEnglishName",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "NormalizedLocalName",
                table: "Countries");
        }
    }
}
