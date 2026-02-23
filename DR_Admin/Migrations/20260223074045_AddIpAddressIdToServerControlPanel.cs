using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddIpAddressIdToServerControlPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IpAddressId",
                table: "ServerControlPanels",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_IpAddressId",
                table: "ServerControlPanels",
                column: "IpAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServerControlPanels_ServerIpAddresses_IpAddressId",
                table: "ServerControlPanels",
                column: "IpAddressId",
                principalTable: "ServerIpAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServerControlPanels_ServerIpAddresses_IpAddressId",
                table: "ServerControlPanels");

            migrationBuilder.DropIndex(
                name: "IX_ServerControlPanels_IpAddressId",
                table: "ServerControlPanels");

            migrationBuilder.DropColumn(
                name: "IpAddressId",
                table: "ServerControlPanels");
        }
    }
}
