using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class CustomerStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "CustomerStatuses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "CustomerStatuses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_IsSystem",
                table: "CustomerStatuses",
                column: "IsSystem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerStatuses_IsSystem",
                table: "CustomerStatuses");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "CustomerStatuses");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "CustomerStatuses");
        }
    }
}
