using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonyERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_EmailPurchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_GameAccounts_Email",
                table: "GameAccounts");

            migrationBuilder.DropIndex(
                name: "IX_GameAccounts_GameName_Platform_BranchId",
                table: "GameAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AuditEvents_Action",
                table: "AuditEvents");

            migrationBuilder.DropIndex(
                name: "IX_AuditEvents_TimestampUtc",
                table: "AuditEvents");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    Contact = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    PurchasedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: true),
                    GameName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Platform = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAtPurchase = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TwoFactorKeyAtPurchase = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fees = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Currency = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    GameAccountId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailPurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailPurchases_GameAccounts_GameAccountId",
                        column: x => x.GameAccountId,
                        principalTable: "GameAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailPurchases_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailPurchases_GameAccountId",
                table: "EmailPurchases",
                column: "GameAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailPurchases_SupplierId",
                table: "EmailPurchases",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailPurchases_TimestampUtc",
                table: "EmailPurchases",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Name",
                table: "Suppliers",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailPurchases");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameAccounts_Email",
                table: "GameAccounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameAccounts_GameName_Platform_BranchId",
                table: "GameAccounts",
                columns: new[] { "GameName", "Platform", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_Action",
                table: "AuditEvents",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TimestampUtc",
                table: "AuditEvents",
                column: "TimestampUtc");
        }
    }
}
