using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderTaxSnapshotIdempotency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "OrderTaxSnapshots",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxSnapshots_OrderId_IdempotencyKey",
                table: "OrderTaxSnapshots",
                columns: new[] { "OrderId", "IdempotencyKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderTaxSnapshots_OrderId_IdempotencyKey",
                table: "OrderTaxSnapshots");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "OrderTaxSnapshots");
        }
    }
}
