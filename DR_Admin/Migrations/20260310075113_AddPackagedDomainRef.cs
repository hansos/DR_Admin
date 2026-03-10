using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddPackagedDomainRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegisteredDomainId",
                table: "SoldOptionalServices",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegisteredDomainId",
                table: "SoldHostingPackages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_RegisteredDomainId",
                table: "SoldOptionalServices",
                column: "RegisteredDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_RegisteredDomainId",
                table: "SoldHostingPackages",
                column: "RegisteredDomainId");

            migrationBuilder.AddForeignKey(
                name: "FK_SoldHostingPackages_RegisteredDomains_RegisteredDomainId",
                table: "SoldHostingPackages",
                column: "RegisteredDomainId",
                principalTable: "RegisteredDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SoldOptionalServices_RegisteredDomains_RegisteredDomainId",
                table: "SoldOptionalServices",
                column: "RegisteredDomainId",
                principalTable: "RegisteredDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SoldHostingPackages_RegisteredDomains_RegisteredDomainId",
                table: "SoldHostingPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_SoldOptionalServices_RegisteredDomains_RegisteredDomainId",
                table: "SoldOptionalServices");

            migrationBuilder.DropIndex(
                name: "IX_SoldOptionalServices_RegisteredDomainId",
                table: "SoldOptionalServices");

            migrationBuilder.DropIndex(
                name: "IX_SoldHostingPackages_RegisteredDomainId",
                table: "SoldHostingPackages");

            migrationBuilder.DropColumn(
                name: "RegisteredDomainId",
                table: "SoldOptionalServices");

            migrationBuilder.DropColumn(
                name: "RegisteredDomainId",
                table: "SoldHostingPackages");
        }
    }
}
