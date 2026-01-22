using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistrar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Domains_DomainProviders_ProviderId",
                table: "Domains");

            migrationBuilder.DropTable(
                name: "DomainProviders");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Domains",
                newName: "RegistrarId");

            migrationBuilder.RenameIndex(
                name: "IX_Domains_ProviderId",
                table: "Domains",
                newName: "IX_Domains_RegistrarId");

            migrationBuilder.AddColumn<bool>(
                name: "AutoRenew",
                table: "Domains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Domains",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PrivacyProtection",
                table: "Domains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RegistrarTldId",
                table: "Domains",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RegistrationPrice",
                table: "Domains",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RenewalPrice",
                table: "Domains",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Registrars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApiUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiSecret = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ApiUsername = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ApiPassword = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Website = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DefaultRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiresPrivacy = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tlds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrarTlds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistrarId = table.Column<int>(type: "INTEGER", nullable: false),
                    TldId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrationCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RegistrationPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RenewalCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RenewalPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TransferCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TransferPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PrivacyCost = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    PrivacyPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    MinRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxRegistrationYears = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrarTlds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrarTlds_Registrars_RegistrarId",
                        column: x => x.RegistrarId,
                        principalTable: "Registrars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrarTlds_Tlds_TldId",
                        column: x => x.TldId,
                        principalTable: "Tlds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Domains_ExpirationDate",
                table: "Domains",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Name",
                table: "Domains",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_RegistrarTldId",
                table: "Domains",
                column: "RegistrarTldId");

            migrationBuilder.CreateIndex(
                name: "IX_Domains_Status",
                table: "Domains",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_Code",
                table: "Registrars",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_IsActive",
                table: "Registrars",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Registrars_Name",
                table: "Registrars",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTlds_IsAvailable",
                table: "RegistrarTlds",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTlds_RegistrarId_TldId",
                table: "RegistrarTlds",
                columns: new[] { "RegistrarId", "TldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrarTlds_TldId",
                table: "RegistrarTlds",
                column: "TldId");

            migrationBuilder.CreateIndex(
                name: "IX_Tlds_Extension",
                table: "Tlds",
                column: "Extension",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tlds_IsActive",
                table: "Tlds",
                column: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_RegistrarTlds_RegistrarTldId",
                table: "Domains",
                column: "RegistrarTldId",
                principalTable: "RegistrarTlds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_Registrars_RegistrarId",
                table: "Domains",
                column: "RegistrarId",
                principalTable: "Registrars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Domains_RegistrarTlds_RegistrarTldId",
                table: "Domains");

            migrationBuilder.DropForeignKey(
                name: "FK_Domains_Registrars_RegistrarId",
                table: "Domains");

            migrationBuilder.DropTable(
                name: "RegistrarTlds");

            migrationBuilder.DropTable(
                name: "Registrars");

            migrationBuilder.DropTable(
                name: "Tlds");

            migrationBuilder.DropIndex(
                name: "IX_Domains_ExpirationDate",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_Name",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_RegistrarTldId",
                table: "Domains");

            migrationBuilder.DropIndex(
                name: "IX_Domains_Status",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "AutoRenew",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "PrivacyProtection",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "RegistrarTldId",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "RegistrationPrice",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "RenewalPrice",
                table: "Domains");

            migrationBuilder.RenameColumn(
                name: "RegistrarId",
                table: "Domains",
                newName: "ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Domains_RegistrarId",
                table: "Domains",
                newName: "IX_Domains_ProviderId");

            migrationBuilder.CreateTable(
                name: "DomainProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiEndpoint = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ApiSecret = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainProviders", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Domains_DomainProviders_ProviderId",
                table: "Domains",
                column: "ProviderId",
                principalTable: "DomainProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
