using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNameSeverSructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NameServers_RegisteredDomains_DomainId",
                table: "NameServers");

            migrationBuilder.DropIndex(
                name: "IX_NameServers_DomainId",
                table: "NameServers");

            migrationBuilder.DropIndex(
                name: "IX_NameServers_DomainId_SortOrder",
                table: "NameServers");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "NameServers");

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "NameServers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NameServerDomains",
                columns: table => new
                {
                    NameServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    DomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NameServerDomains", x => new { x.NameServerId, x.DomainId });
                    table.ForeignKey(
                        name: "FK_NameServerDomains_NameServers_NameServerId",
                        column: x => x.NameServerId,
                        principalTable: "NameServers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NameServerDomains_RegisteredDomains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "RegisteredDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_Hostname",
                table: "NameServers",
                column: "Hostname");

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_ServerId",
                table: "NameServers",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_SortOrder",
                table: "NameServers",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_NameServerDomains_DomainId",
                table: "NameServerDomains",
                column: "DomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_NameServers_Servers_ServerId",
                table: "NameServers",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NameServers_Servers_ServerId",
                table: "NameServers");

            migrationBuilder.DropTable(
                name: "NameServerDomains");

            migrationBuilder.DropIndex(
                name: "IX_NameServers_Hostname",
                table: "NameServers");

            migrationBuilder.DropIndex(
                name: "IX_NameServers_ServerId",
                table: "NameServers");

            migrationBuilder.DropIndex(
                name: "IX_NameServers_SortOrder",
                table: "NameServers");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "NameServers");

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "NameServers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_DomainId",
                table: "NameServers",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_NameServers_DomainId_SortOrder",
                table: "NameServers",
                columns: new[] { "DomainId", "SortOrder" });

            migrationBuilder.AddForeignKey(
                name: "FK_NameServers_RegisteredDomains_DomainId",
                table: "NameServers",
                column: "DomainId",
                principalTable: "RegisteredDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
