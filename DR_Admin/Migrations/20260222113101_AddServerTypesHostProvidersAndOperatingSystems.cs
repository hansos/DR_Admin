using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddServerTypesHostProvidersAndOperatingSystems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Servers_ServerType",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "HostProvider",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "OperatingSystem",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "ServerType",
                table: "Servers");

            migrationBuilder.AddColumn<int>(
                name: "HostProviderId",
                table: "Servers",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OperatingSystemId",
                table: "Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ServerTypeId",
                table: "Servers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HostProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatingSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servers_HostProviderId",
                table: "Servers",
                column: "HostProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_OperatingSystemId",
                table: "Servers",
                column: "OperatingSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_ServerTypeId",
                table: "Servers",
                column: "ServerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HostProviders_IsActive",
                table: "HostProviders",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HostProviders_Name",
                table: "HostProviders",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatingSystems_IsActive",
                table: "OperatingSystems",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingSystems_Name",
                table: "OperatingSystems",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServerTypes_IsActive",
                table: "ServerTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ServerTypes_Name",
                table: "ServerTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_HostProviders_HostProviderId",
                table: "Servers",
                column: "HostProviderId",
                principalTable: "HostProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_OperatingSystems_OperatingSystemId",
                table: "Servers",
                column: "OperatingSystemId",
                principalTable: "OperatingSystems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Servers_ServerTypes_ServerTypeId",
                table: "Servers",
                column: "ServerTypeId",
                principalTable: "ServerTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servers_HostProviders_HostProviderId",
                table: "Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_OperatingSystems_OperatingSystemId",
                table: "Servers");

            migrationBuilder.DropForeignKey(
                name: "FK_Servers_ServerTypes_ServerTypeId",
                table: "Servers");

            migrationBuilder.DropTable(
                name: "HostProviders");

            migrationBuilder.DropTable(
                name: "OperatingSystems");

            migrationBuilder.DropTable(
                name: "ServerTypes");

            migrationBuilder.DropIndex(
                name: "IX_Servers_HostProviderId",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_OperatingSystemId",
                table: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Servers_ServerTypeId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "HostProviderId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "OperatingSystemId",
                table: "Servers");

            migrationBuilder.DropColumn(
                name: "ServerTypeId",
                table: "Servers");

            migrationBuilder.AddColumn<string>(
                name: "HostProvider",
                table: "Servers",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperatingSystem",
                table: "Servers",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServerType",
                table: "Servers",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_ServerType",
                table: "Servers",
                column: "ServerType");
        }
    }
}
