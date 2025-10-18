using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonyERP.Data.Migrations
{
    public partial class AuditEvents_And_TwoFactorKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add TwoFactorKey to GameAccounts
            migrationBuilder.AddColumn<string>(
                name: "TwoFactorKey",
                table: "GameAccounts",
                type: "TEXT",
                nullable: true);

            // Create AuditEvents
            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccountId = table.Column<int>(type: "INTEGER", nullable: true),
                    SaleId = table.Column<int>(type: "INTEGER", nullable: true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TimestampUtc",
                table: "AuditEvents",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_Action",
                table: "AuditEvents",
                column: "Action");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEvents");

            migrationBuilder.DropColumn(
                name: "TwoFactorKey",
                table: "GameAccounts");
        }
    }
}
