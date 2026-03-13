using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddTldRegistryRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TldRegistryRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequireRegistrantContact = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireAdministrativeContact = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireTechnicalContact = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequireBillingContact = table.Column<bool>(type: "INTEGER", nullable: false),
                    RequiresAuthCodeForTransfer = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransferLockDays = table.Column<int>(type: "INTEGER", nullable: true),
                    RenewalGracePeriodDays = table.Column<int>(type: "INTEGER", nullable: true),
                    RedemptionGracePeriodDays = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TldRegistryRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TldRegistryRules_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TldRegistryRules_IsActive",
                table: "TldRegistryRules",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TldRegistryRules_TldId",
                table: "TldRegistryRules",
                column: "TldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TldRegistryRules");
        }
    }
}
