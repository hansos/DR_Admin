using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class UniqueRegistrarMailAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrarMailAddresses_MailAddress",
                table: "RegistrarMailAddresses");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarMailAddresses_MailAddress",
                table: "RegistrarMailAddresses",
                column: "MailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrarMailAddresses_MailAddress",
                table: "RegistrarMailAddresses");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarMailAddresses_MailAddress",
                table: "RegistrarMailAddresses",
                column: "MailAddress");
        }
    }
}
