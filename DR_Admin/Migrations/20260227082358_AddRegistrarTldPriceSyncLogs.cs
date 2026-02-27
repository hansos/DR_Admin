using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrarTldPriceSyncLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrarTldPriceDownloadSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TriggerSource = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Success = table.Column<bool>(type: "INTEGER", nullable: false),
                    TldsProcessed = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceChangesDetected = table.Column<int>(type: "INTEGER", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTldPriceDownloadSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTldPriceDownloadSessions_Registrars_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarTldPriceChangeLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarTldId = table.Column<int>(type: "INTEGER", nullable: false),
                    DownloadSessionId = table.Column<int>(type: "INTEGER", nullable: true),
                    ChangeSource = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ChangedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ChangedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OldRegistrationCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    NewRegistrationCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    OldRenewalCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    NewRenewalCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    OldTransferCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    NewTransferCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: true),
                    OldCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    NewCurrency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTldPriceChangeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTldPriceChangeLogs_RegistrarTldPriceDownloadSessions_DownloadSessionId",
                        column: x => x.DownloadSessionId,
                        principalTable: "RegistrarTldPriceDownloadSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrarTldPriceChangeLogs_RegistrarTlds_RegistrarTldId",
                        column: x => x.RegistrarTldId,
                        principalTable: "RegistrarTlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldPriceChangeLogs_ChangedAtUtc",
                table: "RegistrarTldPriceChangeLogs",
                column: "ChangedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldPriceChangeLogs_ChangeSource",
                table: "RegistrarTldPriceChangeLogs",
                column: "ChangeSource");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldPriceChangeLogs_DownloadSessionId",
                table: "RegistrarTldPriceChangeLogs",
                column: "DownloadSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldPriceChangeLogs_RegistrarTldId",
                table: "RegistrarTldPriceChangeLogs",
                column: "RegistrarTldId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldPriceDownloadSessions_RegistrarId_StartedAtUtc",
                table: "RegistrarTldPriceDownloadSessions",
                columns: new[] { "RegistrarId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTldPriceDownloadSessions_Success",
                table: "RegistrarTldPriceDownloadSessions",
                column: "Success");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrarTldPriceChangeLogs");

            migrationBuilder.DropTable(
                name: "RegistrarTldPriceDownloadSessions");
        }
    }
}
