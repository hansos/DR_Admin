using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class ZonePackageManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DnsZonePackageControlPanels",
                columns: table => new
                {
                    DnsZonePackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerControlPanelId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsZonePackageControlPanels", x => new { x.DnsZonePackageId, x.ServerControlPanelId });
                    table.ForeignKey(
                        name: "FK_DnsZonePackageControlPanels_DnsZonePackages_DnsZonePackageId",
                        column: x => x.DnsZonePackageId,
                        principalTable: "DnsZonePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DnsZonePackageControlPanels_ServerControlPanels_ServerControlPanelId",
                        column: x => x.ServerControlPanelId,
                        principalTable: "ServerControlPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DnsZonePackageServers",
                columns: table => new
                {
                    DnsZonePackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsZonePackageServers", x => new { x.DnsZonePackageId, x.ServerId });
                    table.ForeignKey(
                        name: "FK_DnsZonePackageServers_DnsZonePackages_DnsZonePackageId",
                        column: x => x.DnsZonePackageId,
                        principalTable: "DnsZonePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DnsZonePackageServers_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageControlPanels_DnsZonePackageId",
                table: "DnsZonePackageControlPanels",
                column: "DnsZonePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageControlPanels_ServerControlPanelId",
                table: "DnsZonePackageControlPanels",
                column: "ServerControlPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageServers_DnsZonePackageId",
                table: "DnsZonePackageServers",
                column: "DnsZonePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageServers_ServerId",
                table: "DnsZonePackageServers",
                column: "ServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DnsZonePackageControlPanels");

            migrationBuilder.DropTable(
                name: "DnsZonePackageServers");
        }
    }
}
