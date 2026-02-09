using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddHostingConfigurationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BandwidthLimitMB",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BandwidthUsageMB",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationJson",
                table: "HostingAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiskQuotaMB",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiskUsageMB",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalAccountId",
                table: "HostingAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "HostingAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxDatabases",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxEmailAccounts",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxFtpAccounts",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxSubdomains",
                table: "HostingAccounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanName",
                table: "HostingAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncStatus",
                table: "HostingAccounts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HostingDatabases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    DatabaseName = table.Column<string>(type: "TEXT", nullable: false),
                    DatabaseType = table.Column<string>(type: "TEXT", nullable: false),
                    SizeMB = table.Column<int>(type: "INTEGER", nullable: true),
                    ServerHost = table.Column<string>(type: "TEXT", nullable: true),
                    ServerPort = table.Column<int>(type: "INTEGER", nullable: true),
                    CharacterSet = table.Column<string>(type: "TEXT", nullable: true),
                    Collation = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalDatabaseId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingDatabases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingDatabases_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    DomainName = table.Column<string>(type: "TEXT", nullable: false),
                    DomainType = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentRoot = table.Column<string>(type: "TEXT", nullable: true),
                    SslEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SslExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SslIssuer = table.Column<string>(type: "TEXT", nullable: true),
                    AutoRenewSsl = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExternalDomainId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    PhpEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    PhpVersion = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingDomains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingDomains_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingEmailAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    QuotaMB = table.Column<int>(type: "INTEGER", nullable: true),
                    UsageMB = table.Column<int>(type: "INTEGER", nullable: true),
                    IsForwarderOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    ForwardTo = table.Column<string>(type: "TEXT", nullable: true),
                    AutoResponderEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoResponderMessage = table.Column<string>(type: "TEXT", nullable: true),
                    SpamFilterEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    SpamScoreThreshold = table.Column<int>(type: "INTEGER", nullable: true),
                    ExternalEmailId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingEmailAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingEmailAccounts_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingFtpAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingAccountId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    HomeDirectory = table.Column<string>(type: "TEXT", nullable: false),
                    QuotaMB = table.Column<int>(type: "INTEGER", nullable: true),
                    ReadOnly = table.Column<bool>(type: "INTEGER", nullable: false),
                    SftpEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    FtpsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExternalFtpId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingFtpAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingFtpAccounts_HostingAccounts_HostingAccountId",
                        column: x => x.HostingAccountId,
                        principalTable: "HostingAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostingDatabaseUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HostingDatabaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Privileges = table.Column<string>(type: "TEXT", nullable: true),
                    AllowedHosts = table.Column<string>(type: "TEXT", nullable: true),
                    ExternalUserId = table.Column<string>(type: "TEXT", nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SyncStatus = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostingDatabaseUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostingDatabaseUsers_HostingDatabases_HostingDatabaseId",
                        column: x => x.HostingDatabaseId,
                        principalTable: "HostingDatabases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HostingDatabases_HostingAccountId",
                table: "HostingDatabases",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingDatabaseUsers_HostingDatabaseId",
                table: "HostingDatabaseUsers",
                column: "HostingDatabaseId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingDomains_HostingAccountId",
                table: "HostingDomains",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingEmailAccounts_HostingAccountId",
                table: "HostingEmailAccounts",
                column: "HostingAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_HostingFtpAccounts_HostingAccountId",
                table: "HostingFtpAccounts",
                column: "HostingAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HostingDatabaseUsers");

            migrationBuilder.DropTable(
                name: "HostingDomains");

            migrationBuilder.DropTable(
                name: "HostingEmailAccounts");

            migrationBuilder.DropTable(
                name: "HostingFtpAccounts");

            migrationBuilder.DropTable(
                name: "HostingDatabases");

            migrationBuilder.DropColumn(
                name: "BandwidthLimitMB",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "BandwidthUsageMB",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "ConfigurationJson",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "DiskQuotaMB",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "DiskUsageMB",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "ExternalAccountId",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "MaxDatabases",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "MaxEmailAccounts",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "MaxFtpAccounts",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "MaxSubdomains",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "PlanName",
                table: "HostingAccounts");

            migrationBuilder.DropColumn(
                name: "SyncStatus",
                table: "HostingAccounts");
        }
    }
}
