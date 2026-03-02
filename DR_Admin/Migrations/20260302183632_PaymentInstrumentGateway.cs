using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class PaymentInstrumentGateway : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultGatewayId",
                table: "PaymentInstruments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstruments_DefaultGatewayId",
                table: "PaymentInstruments",
                column: "DefaultGatewayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentInstruments_DefaultGatewayId",
                table: "PaymentInstruments");

            migrationBuilder.DropColumn(
                name: "DefaultGatewayId",
                table: "PaymentInstruments");
        }
    }
}
