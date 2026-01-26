using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateType = table.Column<int>(type: "INTEGER", nullable: false),
                    FileContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlaceholderVariables = table.Column<string>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_TemplateType",
                table: "DocumentTemplates",
                column: "TemplateType");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_IsActive",
                table: "DocumentTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_IsDefault_TemplateType",
                table: "DocumentTemplates",
                columns: new[] { "IsDefault", "TemplateType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DocumentTemplates_TemplateType",
                table: "DocumentTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTemplates_IsActive",
                table: "DocumentTemplates");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTemplates_IsDefault_TemplateType",
                table: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "DocumentTemplates");
        }
    }
}
