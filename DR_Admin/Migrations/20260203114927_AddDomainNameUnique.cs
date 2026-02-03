using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainNameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Domains_Name",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_NormalizedName",
                table: "Domains");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Name",
                table: "Domains",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Domains_NormalizedName",
                table: "Domains",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Domains_Name",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_NormalizedName",
                table: "Domains");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Name",
                table: "Domains",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_NormalizedName",
                table: "Domains",
                column: "NormalizedName");
        }
    }
}
