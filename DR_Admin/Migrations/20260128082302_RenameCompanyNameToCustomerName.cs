using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RenameCompanyNameToCustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedCompanyName",
                table: "Customers",
                newName: "NormalizedCustomerName");

            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Customers",
                newName: "CustomerName");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_NormalizedCompanyName",
                table: "Customers",
                newName: "IX_Customers_NormalizedCustomerName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NormalizedCustomerName",
                table: "Customers",
                newName: "NormalizedCompanyName");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Customers",
                newName: "CompanyName");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_NormalizedCustomerName",
                table: "Customers",
                newName: "IX_Customers_NormalizedCompanyName");
        }
    }
}
