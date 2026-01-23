using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class RemovedRegistryCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_Units_UnitId",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Registrars");

            migrationBuilder.DropColumn(
                name: "ApiPassword",
                table: "Registrars");

            migrationBuilder.DropColumn(
                name: "ApiSecret",
                table: "Registrars");

            migrationBuilder.DropColumn(
                name: "ApiUrl",
                table: "Registrars");

            migrationBuilder.DropColumn(
                name: "ApiUsername",
                table: "Registrars");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Code",
                table: "Units",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_IsActive",
                table: "Units",
                column: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_Units_UnitId",
                table: "InvoiceLines",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLines_Units_UnitId",
                table: "InvoiceLines");

            migrationBuilder.DropIndex(
                name: "IX_Units_Code",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_IsActive",
                table: "Units");

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Registrars",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiPassword",
                table: "Registrars",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiSecret",
                table: "Registrars",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiUrl",
                table: "Registrars",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApiUsername",
                table: "Registrars",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLines_Units_UnitId",
                table: "InvoiceLines",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }
    }
}
