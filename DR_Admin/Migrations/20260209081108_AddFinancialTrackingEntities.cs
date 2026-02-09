using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialTrackingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualExchangeRate",
                table: "PaymentTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GatewayFeeAmount",
                table: "PaymentTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GatewayFeeCurrency",
                table: "PaymentTransactions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelectedPaymentGatewayId",
                table: "Invoices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGatewayFee",
                table: "InvoiceLines",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LineType",
                table: "InvoiceLines",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CustomerTaxProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxIdNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TaxIdType = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxIdValidated = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxIdValidationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TaxIdValidationResponse = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    TaxResidenceCountry = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    CustomerType = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxExempt = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxExemptionReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TaxExemptionCertificateUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTaxProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerTaxProfiles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundLossAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RefundId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginalInvoiceAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RefundedAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    VendorCostUnrecoverable = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    NetLoss = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ApprovalStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ApprovedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DenialReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    InternalNotes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundLossAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundLossAudits_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundLossAudits_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundLossAudits_Users_ApprovedByUserId",
                        column: x => x.ApprovedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorPayouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorType = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PayoutMethod = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    VendorAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailureReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    FailureCount = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionReference = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PaymentGatewayResponse = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    RequiresManualIntervention = table.Column<bool>(type: "INTEGER", nullable: false),
                    InterventionReason = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    InterventionResolvedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    InterventionResolvedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    InternalNotes = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorPayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorPayouts_Users_InterventionResolvedByUserId",
                        column: x => x.InterventionResolvedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorTaxProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorType = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxIdNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TaxResidenceCountry = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Require1099 = table.Column<bool>(type: "INTEGER", nullable: false),
                    W9OnFile = table.Column<bool>(type: "INTEGER", nullable: false),
                    W9FileUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    WithholdingTaxRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 4, nullable: true),
                    TaxTreatyExempt = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxTreatyCountry = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    TaxNotes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorTaxProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InvoiceLineId = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorPayoutId = table.Column<int>(type: "INTEGER", nullable: true),
                    VendorType = table.Column<int>(type: "INTEGER", nullable: false),
                    VendorId = table.Column<int>(type: "INTEGER", nullable: true),
                    VendorName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    VendorCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    VendorAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    BaseCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    BaseAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRefundable = table.Column<bool>(type: "INTEGER", nullable: false),
                    RefundPolicy = table.Column<int>(type: "INTEGER", nullable: false),
                    RefundDeadline = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorCosts_InvoiceLines_InvoiceLineId",
                        column: x => x.InvoiceLineId,
                        principalTable: "InvoiceLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorCosts_VendorPayouts_VendorPayoutId",
                        column: x => x.VendorPayoutId,
                        principalTable: "VendorPayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SelectedPaymentGatewayId",
                table: "Invoices",
                column: "SelectedPaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTaxProfiles_CustomerId",
                table: "CustomerTaxProfiles",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTaxProfiles_TaxIdNumber",
                table: "CustomerTaxProfiles",
                column: "TaxIdNumber");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTaxProfiles_TaxResidenceCountry",
                table: "CustomerTaxProfiles",
                column: "TaxResidenceCountry");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_ApprovalStatus",
                table: "RefundLossAudits",
                column: "ApprovalStatus");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_ApprovedAt",
                table: "RefundLossAudits",
                column: "ApprovedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_ApprovedByUserId",
                table: "RefundLossAudits",
                column: "ApprovedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_InvoiceId",
                table: "RefundLossAudits",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundLossAudits_RefundId",
                table: "RefundLossAudits",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_InvoiceLineId",
                table: "VendorCosts",
                column: "InvoiceLineId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_IsRefundable",
                table: "VendorCosts",
                column: "IsRefundable");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_Status",
                table: "VendorCosts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_VendorPayoutId",
                table: "VendorCosts",
                column: "VendorPayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorCosts_VendorType",
                table: "VendorCosts",
                column: "VendorType");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_InterventionResolvedByUserId",
                table: "VendorPayouts",
                column: "InterventionResolvedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_RequiresManualIntervention",
                table: "VendorPayouts",
                column: "RequiresManualIntervention");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_ScheduledDate",
                table: "VendorPayouts",
                column: "ScheduledDate");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_Status",
                table: "VendorPayouts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_VendorId",
                table: "VendorPayouts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorPayouts_VendorType",
                table: "VendorPayouts",
                column: "VendorType");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxProfiles_Require1099",
                table: "VendorTaxProfiles",
                column: "Require1099");

            migrationBuilder.CreateIndex(
                name: "IX_VendorTaxProfiles_VendorId_VendorType",
                table: "VendorTaxProfiles",
                columns: new[] { "VendorId", "VendorType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_PaymentGateways_SelectedPaymentGatewayId",
                table: "Invoices",
                column: "SelectedPaymentGatewayId",
                principalTable: "PaymentGateways",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_PaymentGateways_SelectedPaymentGatewayId",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "CustomerTaxProfiles");

            migrationBuilder.DropTable(
                name: "RefundLossAudits");

            migrationBuilder.DropTable(
                name: "VendorCosts");

            migrationBuilder.DropTable(
                name: "VendorTaxProfiles");

            migrationBuilder.DropTable(
                name: "VendorPayouts");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_SelectedPaymentGatewayId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ActualExchangeRate",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "GatewayFeeAmount",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "GatewayFeeCurrency",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "SelectedPaymentGatewayId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsGatewayFee",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "LineType",
                table: "InvoiceLines");
        }
    }
}
