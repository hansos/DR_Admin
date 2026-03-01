using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddQuoteRevisionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuoteRevisions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QuoteId = table.Column<int>(type: "INTEGER", nullable: false),
                    RevisionNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    QuoteStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SnapshotJson = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: false),
                    PdfFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PdfFilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ContentHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteRevisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteRevisions_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuoteRevisions_ActionType",
                table: "QuoteRevisions",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteRevisions_QuoteId",
                table: "QuoteRevisions",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteRevisions_QuoteId_RevisionNumber",
                table: "QuoteRevisions",
                columns: new[] { "QuoteId", "RevisionNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuoteRevisions");
        }
    }
}
