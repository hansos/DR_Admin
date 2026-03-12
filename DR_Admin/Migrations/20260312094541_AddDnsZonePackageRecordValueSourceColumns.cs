using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddDnsZonePackageRecordValueSourceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ValueSourceReference",
                table: "DnsZonePackageRecords",
                type: "TEXT",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValueSourceType",
                table: "DnsZonePackageRecords",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "Manual");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValueSourceReference",
                table: "DnsZonePackageRecords");

            migrationBuilder.DropColumn(
                name: "ValueSourceType",
                table: "DnsZonePackageRecords");
        }
    }
}
