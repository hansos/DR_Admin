using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredDomainHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredDomainHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegisteredDomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    ActionType = table.Column<int>(type: "INTEGER", nullable: false),
                    Action = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SourceEntityType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SourceEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    PerformedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredDomainHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisteredDomainHistories_RegisteredDomains_RegisteredDomainId",
                        column: x => x.RegisteredDomainId,
                        principalTable: "RegisteredDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegisteredDomainHistories_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomainHistories_ActionType",
                table: "RegisteredDomainHistories",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomainHistories_OccurredAt",
                table: "RegisteredDomainHistories",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomainHistories_PerformedByUserId",
                table: "RegisteredDomainHistories",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomainHistories_RegisteredDomainId",
                table: "RegisteredDomainHistories",
                column: "RegisteredDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredDomainHistories_RegisteredDomainId_OccurredAt",
                table: "RegisteredDomainHistories",
                columns: new[] { "RegisteredDomainId", "OccurredAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisteredDomainHistories");
        }
    }
}
