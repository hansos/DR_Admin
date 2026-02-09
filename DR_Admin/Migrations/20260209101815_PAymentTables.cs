using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class PAymentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoicePayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AmountApplied = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    InvoiceBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    InvoiceTotalAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsFullPayment = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoicePayments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoicePayments_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    PaymentTransactionId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomerPaymentMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptedAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    GatewayResponse = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: false),
                    RetryCount = table.Column<int>(type: "INTEGER", nullable: false),
                    NextRetryAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RequiresAuthentication = table.Column<bool>(type: "INTEGER", nullable: false),
                    AuthenticationUrl = table.Column<string>(type: "TEXT", nullable: false),
                    AuthenticationStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                    UserAgent = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_CustomerPaymentMethods_CustomerPaymentMethodId",
                        column: x => x.CustomerPaymentMethodId,
                        principalTable: "CustomerPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethodTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerPaymentMethodId = table.Column<int>(type: "INTEGER", nullable: false),
                    EncryptedToken = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayCustomerId = table.Column<string>(type: "TEXT", nullable: false),
                    GatewayPaymentMethodId = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Last4Digits = table.Column<string>(type: "TEXT", nullable: false),
                    CardBrand = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiryMonth = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpiryYear = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVerified = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethodTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethodTokens_CustomerPaymentMethods_CustomerPaymentMethodId",
                        column: x => x.CustomerPaymentMethodId,
                        principalTable: "CustomerPaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_InvoiceId",
                table: "InvoicePayments",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePayments_PaymentTransactionId",
                table: "InvoicePayments",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_CustomerPaymentMethodId",
                table: "PaymentAttempts",
                column: "CustomerPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_InvoiceId",
                table: "PaymentAttempts",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_PaymentTransactionId",
                table: "PaymentAttempts",
                column: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethodTokens_CustomerPaymentMethodId",
                table: "PaymentMethodTokens",
                column: "CustomerPaymentMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoicePayments");

            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "PaymentMethodTokens");
        }
    }
}
