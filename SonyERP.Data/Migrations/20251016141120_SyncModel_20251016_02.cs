using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SonyERP.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel_20251016_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite: أي تعديل على سكيمـا مع قيود مفاتيح خارجية يحتاج تعطيل/إعادة تفعيل FK خارج الترانزاكشن
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            migrationBuilder.DropForeignKey(
                name: "FK_GameAccounts_Branches_BranchId",
                table: "GameAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_GameAccounts_GameAccountId",
                table: "Sales");

            // استبدال AuditLogs بـ AuditEvents
            migrationBuilder.DropTable(
                name: "AuditLogs");

            // لم نعد نحتاج هذا الإندكس (لو كان موجودًا)
            migrationBuilder.DropIndex(
                name: "IX_GameAccounts_BranchId",
                table: "GameAccounts");

            // تحوّيل أعمدة Decimal إلى TEXT (استراتيجية SQLite)
            migrationBuilder.AlterColumn<decimal>(
                name: "PurchasePrice",
                table: "GameAccounts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultSellPrice",
                table: "GameAccounts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            // إزالة StoreRegion (لو حابين نرجّعه في Down)
            migrationBuilder.DropColumn(
                name: "StoreRegion",
                table: "GameAccounts");

            // إضافة سعر قياسي للبيع (اختياري)
            migrationBuilder.AddColumn<decimal>(
                name: "StandardSellPrice",
                table: "GameAccounts",
                type: "TEXT",
                nullable: true);

            // ✅ إضافة TwoFactorKey
            migrationBuilder.AddColumn<string>(
                name: "TwoFactorKey",
                table: "GameAccounts",
                type: "TEXT",
                nullable: true);

            // إنشاء جدول AuditEvents الجديد
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

            // فهارس
            migrationBuilder.CreateIndex(
                name: "IX_GameAccounts_Email",
                table: "GameAccounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_Action",
                table: "AuditEvents",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_TimestampUtc",
                table: "AuditEvents",
                column: "TimestampUtc");

            // علاقات بالمفعول المناسب
            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_GameAccounts_GameAccountId",
                table: "Sales",
                column: "GameAccountId",
                principalTable: "GameAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("PRAGMA foreign_keys = OFF;", suppressTransaction: true);

            // فك العلاقات التي أضفناها
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_GameAccounts_GameAccountId",
                table: "Sales");

            // حذف جدول AuditEvents
            migrationBuilder.DropTable(
                name: "AuditEvents");

            // حذف فهرس Email الفريد (سيُحذف ضمنيًا عند حذف العمود/الجدول؛ لكن نبقيه صريحًا)
            migrationBuilder.DropIndex(
                name: "IX_GameAccounts_Email",
                table: "GameAccounts");

            // إزالة الأعمدة الجديدة
            migrationBuilder.DropColumn(
                name: "StandardSellPrice",
                table: "GameAccounts");

            migrationBuilder.DropColumn(
                name: "TwoFactorKey",
                table: "GameAccounts");

            // إعادة أعمدة الأسعار إلى الدقة/النوع السابقين
            migrationBuilder.AlterColumn<decimal>(
                name: "PurchasePrice",
                table: "GameAccounts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DefaultSellPrice",
                table: "GameAccounts",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            // استرجاع StoreRegion
            migrationBuilder.AddColumn<int>(
                name: "StoreRegion",
                table: "GameAccounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // إعادة جدول AuditLogs القديم كما كان
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Action = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BranchId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Details = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: true),
                    Entity = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            // إعادة فهرس BranchId (لو كانت هناك حاجة له)
            migrationBuilder.CreateIndex(
                name: "IX_GameAccounts_BranchId",
                table: "GameAccounts",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action_Entity_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "Action", "Entity", "CreatedAt" });

            // استعادة العلاقات كما كانت
            migrationBuilder.AddForeignKey(
                name: "FK_GameAccounts_Branches_BranchId",
                table: "GameAccounts",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_GameAccounts_GameAccountId",
                table: "Sales",
                column: "GameAccountId",
                principalTable: "GameAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("PRAGMA foreign_keys = ON;", suppressTransaction: true);
        }
    }
}
