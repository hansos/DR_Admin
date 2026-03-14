using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class EmailCommunication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunicationThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RelatedEntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastMessageAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Open"),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationThreads_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CommunicationThreads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommunicationThreadId = table.Column<int>(type: "INTEGER", nullable: false),
                    Direction = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ExternalMessageId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    InternetMessageId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    FromAddress = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    ToAddresses = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CcAddresses = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    BccAddresses = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Subject = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BodyText = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    BodyHtml = table.Column<string>(type: "TEXT", maxLength: 2147483647, nullable: true),
                    Provider = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    SentEmailId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsRead = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReadAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationMessages_CommunicationThreads_CommunicationThreadId",
                        column: x => x.CommunicationThreadId,
                        principalTable: "CommunicationThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunicationMessages_SentEmails_SentEmailId",
                        column: x => x.SentEmailId,
                        principalTable: "SentEmails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommunicationThreadId = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Role = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationParticipants_CommunicationThreads_CommunicationThreadId",
                        column: x => x.CommunicationThreadId,
                        principalTable: "CommunicationThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommunicationMessageId = table.Column<int>(type: "INTEGER", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 260, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    InlineContentId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationAttachments_CommunicationMessages_CommunicationMessageId",
                        column: x => x.CommunicationMessageId,
                        principalTable: "CommunicationMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunicationStatusEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommunicationMessageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Source = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunicationStatusEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunicationStatusEvents_CommunicationMessages_CommunicationMessageId",
                        column: x => x.CommunicationMessageId,
                        principalTable: "CommunicationMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationAttachments_CommunicationMessageId",
                table: "CommunicationAttachments",
                column: "CommunicationMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_CommunicationThreadId",
                table: "CommunicationMessages",
                column: "CommunicationThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_ExternalMessageId",
                table: "CommunicationMessages",
                column: "ExternalMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_InternetMessageId",
                table: "CommunicationMessages",
                column: "InternetMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_IsRead",
                table: "CommunicationMessages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_ReceivedAtUtc",
                table: "CommunicationMessages",
                column: "ReceivedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_SentAtUtc",
                table: "CommunicationMessages",
                column: "SentAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationMessages_SentEmailId",
                table: "CommunicationMessages",
                column: "SentEmailId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationParticipants_CommunicationThreadId",
                table: "CommunicationParticipants",
                column: "CommunicationThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationParticipants_CommunicationThreadId_EmailAddress_Role",
                table: "CommunicationParticipants",
                columns: new[] { "CommunicationThreadId", "EmailAddress", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationParticipants_EmailAddress",
                table: "CommunicationParticipants",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationParticipants_Role",
                table: "CommunicationParticipants",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationStatusEvents_CommunicationMessageId",
                table: "CommunicationStatusEvents",
                column: "CommunicationMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationStatusEvents_OccurredAtUtc",
                table: "CommunicationStatusEvents",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationStatusEvents_Status",
                table: "CommunicationStatusEvents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationThreads_CustomerId",
                table: "CommunicationThreads",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationThreads_LastMessageAtUtc",
                table: "CommunicationThreads",
                column: "LastMessageAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationThreads_RelatedEntityType_RelatedEntityId",
                table: "CommunicationThreads",
                columns: new[] { "RelatedEntityType", "RelatedEntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationThreads_Status",
                table: "CommunicationThreads",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CommunicationThreads_UserId",
                table: "CommunicationThreads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommunicationAttachments");

            migrationBuilder.DropTable(
                name: "CommunicationParticipants");

            migrationBuilder.DropTable(
                name: "CommunicationStatusEvents");

            migrationBuilder.DropTable(
                name: "CommunicationMessages");

            migrationBuilder.DropTable(
                name: "CommunicationThreads");
        }
    }
}
