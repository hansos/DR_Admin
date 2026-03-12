using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerNoteHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangeType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FieldName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    NewValue = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerChanges_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerChanges_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CustomerInternalNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    Note = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerInternalNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerInternalNotes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerInternalNotes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerChanges_ChangedAt",
                table: "CustomerChanges",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerChanges_ChangedByUserId",
                table: "CustomerChanges",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerChanges_ChangeType",
                table: "CustomerChanges",
                column: "ChangeType");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerChanges_CustomerId",
                table: "CustomerChanges",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInternalNotes_CreatedAt",
                table: "CustomerInternalNotes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInternalNotes_CreatedByUserId",
                table: "CustomerInternalNotes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerInternalNotes_CustomerId",
                table: "CustomerInternalNotes",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerChanges");

            migrationBuilder.DropTable(
                name: "CustomerInternalNotes");
        }
    }
}
