using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCountryCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Customers",
                type: "TEXT",
                maxLength: 2,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CountryCode",
                table: "Customers",
                column: "CountryCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_CountryCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Customers");
        }
    }
}
