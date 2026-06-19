using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "TransactionTypes",
                type: "varchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "💳")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "TransactionTypeId",
                keyValue: 1,
                column: "Icon",
                value: "💳");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "TransactionTypeId",
                keyValue: 2,
                column: "Icon",
                value: "💳");

            migrationBuilder.UpdateData(
                table: "TransactionTypes",
                keyColumn: "TransactionTypeId",
                keyValue: 3,
                column: "Icon",
                value: "💵");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "TransactionTypes");
        }
    }
}
