using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddDnsRecordType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "DnsRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DnsRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DnsRecordTypeId",
                table: "DnsRecords",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "DnsRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "DnsRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DnsRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "DnsRecords",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DnsRecordTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    HasPriority = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasWeight = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasPort = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsEditableByUser = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DefaultTTL = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 3600),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DnsRecordTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecords_DnsRecordTypeId",
                table: "DnsRecords",
                column: "DnsRecordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecords_DomainId_DnsRecordTypeId",
                table: "DnsRecords",
                columns: new[] { "DomainId", "DnsRecordTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecordTypes_IsActive",
                table: "DnsRecordTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DnsRecordTypes_Type",
                table: "DnsRecordTypes",
                column: "Type",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DnsRecords_DnsRecordTypes_DnsRecordTypeId",
                table: "DnsRecords",
                column: "DnsRecordTypeId",
                principalTable: "DnsRecordTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DnsRecords_DnsRecordTypes_DnsRecordTypeId",
                table: "DnsRecords");

            migrationBuilder.DropTable(
                name: "DnsRecordTypes");

            migrationBuilder.DropIndex(
                name: "IX_DnsRecords_DnsRecordTypeId",
                table: "DnsRecords");

            migrationBuilder.DropIndex(
                name: "IX_DnsRecords_DomainId_DnsRecordTypeId",
                table: "DnsRecords");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DnsRecords");

            migrationBuilder.DropColumn(
                name: "DnsRecordTypeId",
                table: "DnsRecords");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "DnsRecords");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "DnsRecords");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DnsRecords");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "DnsRecords");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "DnsRecords",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
