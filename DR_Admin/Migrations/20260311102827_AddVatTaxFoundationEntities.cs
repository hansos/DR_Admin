using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddVatTaxFoundationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TaxJurisdictionId",
                table: "TaxRules",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxJurisdictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    StateCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    TaxAuthority = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TaxCurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxJurisdictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderTaxSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaxJurisdictionId = table.Column<int>(type: "INTEGER", nullable: true),
                    BuyerCountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    BuyerStateCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    BuyerType = table.Column<int>(type: "INTEGER", nullable: false),
                    BuyerTaxId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BuyerTaxIdValidated = table.Column<bool>(type: "INTEGER", nullable: false),
                    TaxCurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    DisplayCurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 8, nullable: true),
                    ExchangeRateDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NetAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    AppliedTaxRate = table.Column<decimal>(type: "TEXT", precision: 18, scale: 6, nullable: false),
                    AppliedTaxName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ReverseChargeApplied = table.Column<bool>(type: "INTEGER", nullable: false),
                    RuleVersion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CalculationInputsJson = table.Column<string>(type: "TEXT", maxLength: 8000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTaxSnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTaxSnapshots_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTaxSnapshots_TaxJurisdictions_TaxJurisdictionId",
                        column: x => x.TaxJurisdictionId,
                        principalTable: "TaxJurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaxRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaxJurisdictionId = table.Column<int>(type: "INTEGER", nullable: false),
                    LegalEntityName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EffectiveUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaxRegistrations_TaxJurisdictions_TaxJurisdictionId",
                        column: x => x.TaxJurisdictionId,
                        principalTable: "TaxJurisdictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_CountryCode",
                table: "TaxRules",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_CountryCode_StateCode",
                table: "TaxRules",
                columns: new[] { "CountryCode", "StateCode" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_EffectiveFrom",
                table: "TaxRules",
                column: "EffectiveFrom");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_EffectiveUntil",
                table: "TaxRules",
                column: "EffectiveUntil");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_IsActive",
                table: "TaxRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRules_TaxJurisdictionId",
                table: "TaxRules",
                column: "TaxJurisdictionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxSnapshots_BuyerCountryCode",
                table: "OrderTaxSnapshots",
                column: "BuyerCountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxSnapshots_CreatedAt",
                table: "OrderTaxSnapshots",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxSnapshots_OrderId",
                table: "OrderTaxSnapshots",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTaxSnapshots_TaxJurisdictionId",
                table: "OrderTaxSnapshots",
                column: "TaxJurisdictionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxJurisdictions_Code",
                table: "TaxJurisdictions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxJurisdictions_CountryCode",
                table: "TaxJurisdictions",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_TaxJurisdictions_CountryCode_StateCode",
                table: "TaxJurisdictions",
                columns: new[] { "CountryCode", "StateCode" });

            migrationBuilder.CreateIndex(
                name: "IX_TaxJurisdictions_IsActive",
                table: "TaxJurisdictions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRegistrations_IsActive",
                table: "TaxRegistrations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRegistrations_RegistrationNumber",
                table: "TaxRegistrations",
                column: "RegistrationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRegistrations_TaxJurisdictionId",
                table: "TaxRegistrations",
                column: "TaxJurisdictionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxRegistrations_TaxJurisdictionId_RegistrationNumber",
                table: "TaxRegistrations",
                columns: new[] { "TaxJurisdictionId", "RegistrationNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxRules_TaxJurisdictions_TaxJurisdictionId",
                table: "TaxRules",
                column: "TaxJurisdictionId",
                principalTable: "TaxJurisdictions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaxRules_TaxJurisdictions_TaxJurisdictionId",
                table: "TaxRules");

            migrationBuilder.DropTable(
                name: "OrderTaxSnapshots");

            migrationBuilder.DropTable(
                name: "TaxRegistrations");

            migrationBuilder.DropTable(
                name: "TaxJurisdictions");

            migrationBuilder.DropIndex(
                name: "IX_TaxRules_CountryCode",
                table: "TaxRules");

            migrationBuilder.DropIndex(
                name: "IX_TaxRules_CountryCode_StateCode",
                table: "TaxRules");

            migrationBuilder.DropIndex(
                name: "IX_TaxRules_EffectiveFrom",
                table: "TaxRules");

            migrationBuilder.DropIndex(
                name: "IX_TaxRules_EffectiveUntil",
                table: "TaxRules");

            migrationBuilder.DropIndex(
                name: "IX_TaxRules_IsActive",
                table: "TaxRules");

            migrationBuilder.DropIndex(
                name: "IX_TaxRules_TaxJurisdictionId",
                table: "TaxRules");

            migrationBuilder.DropColumn(
                name: "TaxJurisdictionId",
                table: "TaxRules");
        }
    }
}
