using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitAndInvoiceLinePropertes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountingCode",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "LineNumber",
                table: "InvoiceLines",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServiceNameSnapshot",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalWithTax",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "InvoiceLines",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InvoiceLines",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLines_UnitId",
                table: "InvoiceLines",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_Unit_UnitId",
                table: "InvoiceLines",
                column: "UnitId",
                principalTable: "Unit",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_Unit_UnitId",
                table: "InvoiceLines");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceLines_UnitId",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "AccountingCode",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "LineNumber",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "ServiceNameSnapshot",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "TotalWithTax",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InvoiceLines");
        }
    }
}
