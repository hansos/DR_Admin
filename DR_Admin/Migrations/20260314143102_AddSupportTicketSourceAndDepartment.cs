using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportTicketSourceAndDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedDepartment",
                table: "SupportTickets",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "SupportTickets",
                type: "TEXT",
                maxLength: 30,
                nullable: false,
                defaultValue: "CustomerWeb");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_AssignedDepartment",
                table: "SupportTickets",
                column: "AssignedDepartment");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_Source",
                table: "SupportTickets",
                column: "Source");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_AssignedDepartment",
                table: "SupportTickets");

            migrationBuilder.DropIndex(
                name: "IX_SupportTickets_Source",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "AssignedDepartment",
                table: "SupportTickets");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "SupportTickets");
        }
    }
}
