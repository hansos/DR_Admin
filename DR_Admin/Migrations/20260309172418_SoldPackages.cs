using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISPAdmin.Migrations
{
    /// <inheritdoc />
    public partial class SoldPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoldHostingPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    HostingPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderLineId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BillingCycle = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SetupFee = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RecurringPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConfigurationSnapshotJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldHostingPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldHostingPackages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldHostingPackages_HostingPackages_HostingPackageId",
                        column: x => x.HostingPackageId,
                        principalTable: "HostingPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldHostingPackages_OrderLines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "OrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SoldHostingPackages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoldOptionalServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderLineId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BillingCycle = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CurrencyCode = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    ConfigurationSnapshotJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldOptionalServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldOptionalServices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldOptionalServices_OrderLines_OrderLineId",
                        column: x => x.OrderLineId,
                        principalTable: "OrderLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SoldOptionalServices_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldOptionalServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_CustomerId",
                table: "SoldHostingPackages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_HostingPackageId",
                table: "SoldHostingPackages",
                column: "HostingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_NextBillingDate",
                table: "SoldHostingPackages",
                column: "NextBillingDate");

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_OrderId",
                table: "SoldHostingPackages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_OrderLineId",
                table: "SoldHostingPackages",
                column: "OrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldHostingPackages_Status",
                table: "SoldHostingPackages",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_CustomerId",
                table: "SoldOptionalServices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_NextBillingDate",
                table: "SoldOptionalServices",
                column: "NextBillingDate");

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_OrderId",
                table: "SoldOptionalServices",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_OrderLineId",
                table: "SoldOptionalServices",
                column: "OrderLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_ServiceId",
                table: "SoldOptionalServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldOptionalServices_Status",
                table: "SoldOptionalServices",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoldHostingPackages");

            migrationBuilder.DropTable(
                name: "SoldOptionalServices");
        }
    }
}
