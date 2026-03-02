using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class PaymentInstrument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentInstrument",
                table: "PaymentGateways",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentInstrumentId",
                table: "PaymentGateways",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentInstruments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NormalizedCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentInstruments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_PaymentInstrument",
                table: "PaymentGateways",
                column: "PaymentInstrument");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateways_PaymentInstrumentId",
                table: "PaymentGateways",
                column: "PaymentInstrumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstruments_Code",
                table: "PaymentInstruments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstruments_DisplayOrder",
                table: "PaymentInstruments",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstruments_IsActive",
                table: "PaymentInstruments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentInstruments_NormalizedCode",
                table: "PaymentInstruments",
                column: "NormalizedCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentGateways_PaymentInstruments_PaymentInstrumentId",
                table: "PaymentGateways",
                column: "PaymentInstrumentId",
                principalTable: "PaymentInstruments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentGateways_PaymentInstruments_PaymentInstrumentId",
                table: "PaymentGateways");

            migrationBuilder.DropTable(
                name: "PaymentInstruments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentGateways_PaymentInstrument",
                table: "PaymentGateways");

            migrationBuilder.DropIndex(
                name: "IX_PaymentGateways_PaymentInstrumentId",
                table: "PaymentGateways");

            migrationBuilder.DropColumn(
                name: "PaymentInstrument",
                table: "PaymentGateways");

            migrationBuilder.DropColumn(
                name: "PaymentInstrumentId",
                table: "PaymentGateways");
        }
    }
}
