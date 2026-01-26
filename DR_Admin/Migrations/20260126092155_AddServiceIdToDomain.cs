using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceIdToDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Domains",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_ServiceId",
                table: "Domains",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_Services_ServiceId",
                table: "Domains",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Domains_Services_ServiceId",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_ServiceId",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Domains");
        }
    }
}
