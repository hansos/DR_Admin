using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddResellerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResellerCompanyId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesAgentId",
                table: "Services",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResellerCompanyId",
                table: "DnsZonePackages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesAgentId",
                table: "DnsZonePackages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResellerCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 2, nullable: true),
                    CompanyRegistrationNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TaxId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    VatNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResellerCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ResellerCompanyId = table.Column<int>(type: "INTEGER", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    MobilePhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesAgents_ResellerCompanies_ResellerCompanyId",
                        column: x => x.ResellerCompanyId,
                        principalTable: "ResellerCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Services_ResellerCompanyId",
                table: "Services",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_SalesAgentId",
                table: "Services",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_ResellerCompanyId",
                table: "DnsZonePackages",
                column: "ResellerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_DnsZonePackages_SalesAgentId",
                table: "DnsZonePackages",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCompanies_Email",
                table: "ResellerCompanies",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCompanies_IsActive",
                table: "ResellerCompanies",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ResellerCompanies_Name",
                table: "ResellerCompanies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_Email",
                table: "SalesAgents",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_IsActive",
                table: "SalesAgents",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SalesAgents_ResellerCompanyId",
                table: "SalesAgents",
                column: "ResellerCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_DnsZonePackages_ResellerCompanies_ResellerCompanyId",
                table: "DnsZonePackages",
                column: "ResellerCompanyId",
                principalTable: "ResellerCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DnsZonePackages_SalesAgents_SalesAgentId",
                table: "DnsZonePackages",
                column: "SalesAgentId",
                principalTable: "SalesAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ResellerCompanies_ResellerCompanyId",
                table: "Services",
                column: "ResellerCompanyId",
                principalTable: "ResellerCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_SalesAgents_SalesAgentId",
                table: "Services",
                column: "SalesAgentId",
                principalTable: "SalesAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DnsZonePackages_ResellerCompanies_ResellerCompanyId",
                table: "DnsZonePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_DnsZonePackages_SalesAgents_SalesAgentId",
                table: "DnsZonePackages");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ResellerCompanies_ResellerCompanyId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_SalesAgents_SalesAgentId",
                table: "Services");

            migrationBuilder.DropTable(
                name: "SalesAgents");

            migrationBuilder.DropTable(
                name: "ResellerCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Services_ResellerCompanyId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_SalesAgentId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_DnsZonePackages_ResellerCompanyId",
                table: "DnsZonePackages");

            migrationBuilder.DropIndex(
                name: "IX_DnsZonePackages_SalesAgentId",
                table: "DnsZonePackages");

            migrationBuilder.DropColumn(
                name: "ResellerCompanyId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "SalesAgentId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ResellerCompanyId",
                table: "DnsZonePackages");

            migrationBuilder.DropColumn(
                name: "SalesAgentId",
                table: "DnsZonePackages");
        }
    }
}
