using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonyERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSchema_Step1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DefaultSellPrice",
                table: "GameAccounts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ps4OfflineLeft",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ps4PrimaryLeft",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ps4SecondaryLeft",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ps5OfflineLeft",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ps5PrimaryLeft",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Ps5SecondaryLeft",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "GameAccounts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StoreRegion",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorKey",
                table: "GameAccounts",
                type: "TEXT",
                maxLength: 160,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Entity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action_Entity_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "Action", "Entity", "CreatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DefaultSellPrice",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "Ps4OfflineLeft",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "Ps4PrimaryLeft",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "Ps4SecondaryLeft",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "Ps5OfflineLeft",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "Ps5PrimaryLeft",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "Ps5SecondaryLeft",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "StoreRegion",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "TwoFactorKey",
                table: "GameAccounts");
        }
    }
}
