using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddServerAndPanelTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HostingPackageId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServerControlPanelId",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServerId",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ControlPanelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Version = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    WebsiteUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlPanelTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostingPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    DiskSpaceMB = table.Column<int>(type: "INTEGER", nullable: false),
                    BandwidthMB = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAccounts = table.Column<int>(type: "INTEGER", nullable: false),
                    Databases = table.Column<int>(type: "INTEGER", nullable: false),
                    Domains = table.Column<int>(type: "INTEGER", nullable: false),
                    Subdomains = table.Column<int>(type: "INTEGER", nullable: false),
                    FtpAccounts = table.Column<int>(type: "INTEGER", nullable: false),
                    SslSupport = table.Column<bool>(type: "INTEGER", nullable: false),
                    BackupSupport = table.Column<bool>(type: "INTEGER", nullable: false),
                    DedicatedIp = table.Column<bool>(type: "INTEGER", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    YearlyPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ServerType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    HostProvider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OperatingSystem = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CpuCores = table.Column<int>(type: "INTEGER", nullable: true),
                    RamMB = table.Column<int>(type: "INTEGER", nullable: true),
                    DiskSpaceGB = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerControlPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ControlPanelTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ApiUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    UseHttps = table.Column<bool>(type: "INTEGER", nullable: false),
                    ApiToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    AdditionalSettings = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastConnectionTest = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsConnectionHealthy = table.Column<bool>(type: "INTEGER", nullable: true),
                    LastError = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerControlPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerControlPanels_ControlPanelTypes_ControlPanelTypeId",
                        column: x => x.ControlPanelTypeId,
                        principalTable: "ControlPanelTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServerControlPanels_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerIpAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerId = table.Column<int>(type: "INTEGER", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false),
                    IpVersion = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AssignedTo = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerIpAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServerIpAddresses_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_HostingPackageId",
                table: "Services",
                column: "HostingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingAccounts_ServerControlPanelId",
                table: "HostingAccounts",
                column: "ServerControlPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingAccounts_ServerId",
                table: "HostingAccounts",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPanelTypes_IsActive",
                table: "ControlPanelTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ControlPanelTypes_Name",
                table: "ControlPanelTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostingPackages_IsActive",
                table: "HostingPackages",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_HostingPackages_Name",
                table: "HostingPackages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_ControlPanelTypeId",
                table: "ServerControlPanels",
                column: "ControlPanelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_ServerId",
                table: "ServerControlPanels",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerControlPanels_Status",
                table: "ServerControlPanels",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServerIpAddresses_IpAddress",
                table: "ServerIpAddresses",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_ServerIpAddresses_ServerId",
                table: "ServerIpAddresses",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerIpAddresses_Status",
                table: "ServerIpAddresses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_Name",
                table: "Servers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_ServerType",
                table: "Servers",
                column: "ServerType");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_Status",
                table: "Servers",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_HostingAccounts_ServerControlPanels_ServerControlPanelId",
                table: "HostingAccounts",
                column: "ServerControlPanelId",
                principalTable: "ServerControlPanels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HostingAccounts_Servers_ServerId",
                table: "HostingAccounts",
                column: "ServerId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_HostingPackages_HostingPackageId",
                table: "Services",
                column: "HostingPackageId",
                principalTable: "HostingPackages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HostingAccounts_ServerControlPanels_ServerControlPanelId",
                table: "HostingAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_HostingAccounts_Servers_ServerId",
                table: "HostingAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_HostingPackages_HostingPackageId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "HostingPackages");

            migrationBuilder.DropTable(
                name: "ServerControlPanels");

            migrationBuilder.DropTable(
                name: "ServerIpAddresses");

            migrationBuilder.DropTable(
                name: "ControlPanelTypes");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropIndex(
                name: "IX_Services_HostingPackageId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_HostingAccounts_ServerControlPanelId",
                table: "HostingAccounts");

            migrationBuilder.DropIndex(
                name: "IX_HostingAccounts_ServerId",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "HostingPackageId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServerControlPanelId",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "HostingAccounts");
        }
    }
}
