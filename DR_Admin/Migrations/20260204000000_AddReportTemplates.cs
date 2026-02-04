using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddReportTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TemplateType = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportEngine = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FileContent = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    MimeType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataSourceInfo = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DefaultExportFormat = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_TemplateType",
                table: "ReportTemplates",
                column: "TemplateType");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_ReportEngine",
                table: "ReportTemplates",
                column: "ReportEngine");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_IsActive",
                table: "ReportTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_IsDefault",
                table: "ReportTemplates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_DeletedAt",
                table: "ReportTemplates",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_Name",
                table: "ReportTemplates",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_TemplateType_IsDefault",
                table: "ReportTemplates",
                columns: new[] { "TemplateType", "IsDefault" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportTemplates");
        }
    }
}
