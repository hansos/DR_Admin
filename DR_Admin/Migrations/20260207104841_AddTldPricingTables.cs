using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddTldPricingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrarSelectionPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    OffersHosting = table.Column<bool>(type: "INTEGER", nullable: false),
                    OffersEmail = table.Column<bool>(type: "INTEGER", nullable: false),
                    OffersSsl = table.Column<bool>(type: "INTEGER", nullable: false),
                    MaxCostDifferenceThreshold = table.Column<decimal>(type: "TEXT", nullable: true),
                    PreferForHostingCustomers = table.Column<bool>(type: "INTEGER", nullable: false),
                    PreferForEmailCustomers = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarSelectionPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarSelectionPreferences_Registrars_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarTldCostPricing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarTldId = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegistrationCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    RenewalCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    TransferCost = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrivacyCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    FirstYearRegistrationCost = table.Column<decimal>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTldCostPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTldCostPricing_RegistrarTlds_RegistrarTldId",
                        column: x => x.RegistrarTldId,
                        principalTable: "RegistrarTlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResellerTldDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResellerCompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "TEXT", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "TEXT", nullable: true),
                    DiscountCurrency = table.Column<string>(type: "TEXT", nullable: true),
                    ApplyToRegistration = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApplyToRenewal = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApplyToTransfer = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerTldDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResellerTldDiscounts_ResellerCompanies_ResellerCompanyId",
                        column: x => x.ResellerCompanyId,
                        principalTable: "ResellerCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResellerTldDiscounts_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TldSalesPricing",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RegistrationPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    RenewalPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    TransferPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrivacyPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    FirstYearRegistrationPrice = table.Column<decimal>(type: "TEXT", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    IsPromotional = table.Column<bool>(type: "INTEGER", nullable: false),
                    PromotionName = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TldSalesPricing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TldSalesPricing_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarSelectionPreferences_RegistrarId",
                table: "RegistrarSelectionPreferences",
                column: "RegistrarId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldCostPricing_RegistrarTldId",
                table: "RegistrarTldCostPricing",
                column: "RegistrarTldId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerTldDiscounts_ResellerCompanyId",
                table: "ResellerTldDiscounts",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerTldDiscounts_TldId",
                table: "ResellerTldDiscounts",
                column: "TldId");

            migrationBuilder.CreateIndex(
                name: "IX_TldSalesPricing_TldId",
                table: "TldSalesPricing",
                column: "TldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrarSelectionPreferences");

            migrationBuilder.DropTable(
                name: "RegistrarTldCostPricing");

            migrationBuilder.DropTable(
                name: "ResellerTldDiscounts");

            migrationBuilder.DropTable(
                name: "TldSalesPricing");
        }
    }
}
