using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddDNSZonePackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DnsZonePackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsZonePackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DnsZonePackageRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DnsZonePackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    DnsRecordTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    TTL = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<int>(type: "INTEGER", nullable: true),
                    Port = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsZonePackageRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DnsZonePackageRecords_DnsRecordTypes_DnsRecordTypeId",
                        column: x => x.DnsRecordTypeId,
                        principalTable: "DnsRecordTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DnsZonePackageRecords_DnsZonePackages_DnsZonePackageId",
                        column: x => x.DnsZonePackageId,
                        principalTable: "DnsZonePackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageRecords_DnsRecordTypeId",
                table: "DnsZonePackageRecords",
                column: "DnsRecordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackageRecords_DnsZonePackageId",
                table: "DnsZonePackageRecords",
                column: "DnsZonePackageId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_IsActive",
                table: "DnsZonePackages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_IsDefault",
                table: "DnsZonePackages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_Name",
                table: "DnsZonePackages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_SortOrder",
                table: "DnsZonePackages",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DnsZonePackageRecords");

            migrationBuilder.DropTable(
                name: "DnsZonePackages");
        }
    }
}
