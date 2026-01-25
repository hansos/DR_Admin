using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailQueueSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Body",
                table: "SentEmails",
                newName: "BodyText");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SentEmails",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentDate",
                table: "SentEmails",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "RetryCount",
                table: "SentEmails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyHtml",
                table: "SentEmails",
                type: "TEXT",
                maxLength: 2147483647,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxRetries",
                table: "SentEmails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextAttemptAt",
                table: "SentEmails",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "SentEmails",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentGatewayId",
                table: "PaymentTransactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentGateways",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderCode = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", nullable: false),
                    ApiSecret = table.Column<string>(type: "TEXT", nullable: false),
                    ConfigurationJson = table.Column<string>(type: "TEXT", nullable: false),
                    UseSandbox = table.Column<bool>(type: "INTEGER", nullable: false),
                    WebhookUrl = table.Column<string>(type: "TEXT", nullable: false),
                    WebhookSecret = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: false),
                    SupportedCurrencies = table.Column<string>(type: "TEXT", nullable: false),
                    FeePercentage = table.Column<decimal>(type: "TEXT", nullable: false),
                    FixedFee = table.Column<decimal>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateways", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_NextAttemptAt",
                table: "SentEmails",
                column: "NextAttemptAt");

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_Status_NextAttemptAt",
                table: "SentEmails",
                columns: new[] { "Status", "NextAttemptAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentGatewayId",
                table: "PaymentTransactions",
                column: "PaymentGatewayId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_PaymentGateways_PaymentGatewayId",
                table: "PaymentTransactions",
                column: "PaymentGatewayId",
                principalTable: "PaymentGateways",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_PaymentGateways_PaymentGatewayId",
                table: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_SentEmails_NextAttemptAt",
                table: "SentEmails");

            migrationBuilder.DropIndex(
                name: "IX_SentEmails_Status_NextAttemptAt",
                table: "SentEmails");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_PaymentGatewayId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "BodyHtml",
                table: "SentEmails");

            migrationBuilder.DropColumn(
                name: "MaxRetries",
                table: "SentEmails");

            migrationBuilder.DropColumn(
                name: "NextAttemptAt",
                table: "SentEmails");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "SentEmails");

            migrationBuilder.DropColumn(
                name: "PaymentGatewayId",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "BodyText",
                table: "SentEmails",
                newName: "Body");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SentEmails",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentDate",
                table: "SentEmails",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RetryCount",
                table: "SentEmails",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);
        }
    }
}
