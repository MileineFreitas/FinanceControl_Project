using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAccountSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("a1000001-0001-4001-8001-000000000001"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "CreatedAt", "CurrentBalance", "InitialBalance", "IsActive", "Name", "UserId" },
                values: new object[] { new Guid("a1000001-0001-4001-8001-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0m, 0m, true, "Principal", null });
        }
    }
}
