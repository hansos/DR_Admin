using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class DomainContactAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "DomainContacts");

            migrationBuilder.AddColumn<bool>(
                name: "IsCurrentVersion",
                table: "DomainContacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivacyProtected",
                table: "DomainContacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedAt",
                table: "DomainContacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NeedsSync",
                table: "DomainContacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RegistrarContactId",
                table: "DomainContacts",
                type: "TEXT",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrarSnapshot",
                table: "DomainContacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrarType",
                table: "DomainContacts",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "DomainContacts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceContactPersonId",
                table: "DomainContacts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DomainContactAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegisteredDomainId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContactPersonId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleType = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DomainContactAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DomainContactAssignments_ContactPersons_ContactPersonId",
                        column: x => x.ContactPersonId,
                        principalTable: "ContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DomainContactAssignments_RegisteredDomains_RegisteredDomainId",
                        column: x => x.RegisteredDomainId,
                        principalTable: "RegisteredDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_DomainId_RoleType_IsCurrentVersion",
                table: "DomainContacts",
                columns: new[] { "DomainId", "RoleType", "IsCurrentVersion" });

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_Email",
                table: "DomainContacts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_IsCurrentVersion",
                table: "DomainContacts",
                column: "IsCurrentVersion");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_NeedsSync",
                table: "DomainContacts",
                column: "NeedsSync");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_NormalizedEmail",
                table: "DomainContacts",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_RoleType",
                table: "DomainContacts",
                column: "RoleType");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContacts_SourceContactPersonId",
                table: "DomainContacts",
                column: "SourceContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContactAssignments_AssignedAt",
                table: "DomainContactAssignments",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContactAssignments_ContactPersonId",
                table: "DomainContactAssignments",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContactAssignments_IsActive",
                table: "DomainContactAssignments",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContactAssignments_RegisteredDomainId",
                table: "DomainContactAssignments",
                column: "RegisteredDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_DomainContactAssignments_RegisteredDomainId_RoleType_IsActive",
                table: "DomainContactAssignments",
                columns: new[] { "RegisteredDomainId", "RoleType", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_DomainContacts_ContactPersons_SourceContactPersonId",
                table: "DomainContacts",
                column: "SourceContactPersonId",
                principalTable: "ContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DomainContacts_ContactPersons_SourceContactPersonId",
                table: "DomainContacts");

            migrationBuilder.DropTable(
                name: "DomainContactAssignments");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_DomainId_RoleType_IsCurrentVersion",
                table: "DomainContacts");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_Email",
                table: "DomainContacts");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_IsCurrentVersion",
                table: "DomainContacts");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_NeedsSync",
                table: "DomainContacts");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_NormalizedEmail",
                table: "DomainContacts");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_RoleType",
                table: "DomainContacts");

            migrationBuilder.DropIndex(
                name: "IX_DomainContacts_SourceContactPersonId",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "IsCurrentVersion",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "IsPrivacyProtected",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "LastSyncedAt",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "NeedsSync",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "RegistrarContactId",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "RegistrarSnapshot",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "RegistrarType",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "DomainContacts");

            migrationBuilder.DropColumn(
                name: "SourceContactPersonId",
                table: "DomainContacts");

            migrationBuilder.AddColumn<string>(
                name: "ContactType",
                table: "DomainContacts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
