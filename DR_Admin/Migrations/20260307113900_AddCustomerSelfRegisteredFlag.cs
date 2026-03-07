using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerSelfRegisteredFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSelfRegistered",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsSelfRegistered",
                table: "Customers",
                column: "IsSelfRegistered");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_IsSelfRegistered",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsSelfRegistered",
                table: "Customers");
        }
    }
}
