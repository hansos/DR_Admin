using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerStatusId",
                table: "Customers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    NormalizedCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerStatusId",
                table: "Customers",
                column: "CustomerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_Code",
                table: "CustomerStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_IsActive",
                table: "CustomerStatuses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_IsDefault",
                table: "CustomerStatuses",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_NormalizedCode",
                table: "CustomerStatuses",
                column: "NormalizedCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerStatuses_SortOrder",
                table: "CustomerStatuses",
                column: "SortOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_CustomerStatuses_CustomerStatusId",
                table: "Customers",
                column: "CustomerStatusId",
                principalTable: "CustomerStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_CustomerStatuses_CustomerStatusId",
                table: "Customers");

            migrationBuilder.DropTable(
                name: "CustomerStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Customers_CustomerStatusId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerStatusId",
                table: "Customers");
        }
    }
}
