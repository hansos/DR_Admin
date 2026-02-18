using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class ContactPersonRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultAdministrator",
                table: "ContactPersons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultBilling",
                table: "ContactPersons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultOwner",
                table: "ContactPersons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultTech",
                table: "ContactPersons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultAdministrator",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "IsDefaultBilling",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "IsDefaultOwner",
                table: "ContactPersons");

            migrationBuilder.DropColumn(
                name: "IsDefaultTech",
                table: "ContactPersons");
        }
    }
}
