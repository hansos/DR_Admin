using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPaymentGatewayReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentGatewayId",
                table: "Subscriptions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PaymentGatewayId",
                table: "Subscriptions",
                column: "PaymentGatewayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_PaymentGateways_PaymentGatewayId",
                table: "Subscriptions",
                column: "PaymentGatewayId",
                principalTable: "PaymentGateways",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_PaymentGateways_PaymentGatewayId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_PaymentGatewayId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "PaymentGatewayId",
                table: "Subscriptions");
        }
    }
}
