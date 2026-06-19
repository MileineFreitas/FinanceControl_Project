using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkTransactionToPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PaymentMethodId",
                table: "Transactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.Sql("""
                UPDATE Transactions t
                SET PaymentMethodId = (
                    SELECT pm.PaymentMethodId
                    FROM PaymentMethods pm
                    WHERE pm.Name = CASE t.PaymentKind
                        WHEN 1 THEN 'Débito'
                        WHEN 2 THEN 'Crédito'
                        WHEN 3 THEN 'Dinheiro'
                        ELSE 'Débito'
                    END
                    LIMIT 1
                )
                WHERE t.PaymentMethodId IS NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE Transactions
                SET PaymentMethodId = (SELECT PaymentMethodId FROM PaymentMethods ORDER BY PaymentMethodId LIMIT 1)
                WHERE PaymentMethodId IS NULL;
                """);

            migrationBuilder.DropColumn(
                name: "PaymentKind",
                table: "Transactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PaymentMethodId",
                table: "Transactions",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldCollation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentMethodId",
                table: "Transactions",
                column: "PaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_PaymentMethods_PaymentMethodId",
                table: "Transactions",
                column: "PaymentMethodId",
                principalTable: "PaymentMethods",
                principalColumn: "PaymentMethodId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_PaymentMethods_PaymentMethodId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PaymentMethodId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "PaymentKind",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE Transactions t
                SET PaymentKind = CASE (
                    SELECT pm.Name FROM PaymentMethods pm WHERE pm.PaymentMethodId = t.PaymentMethodId LIMIT 1
                )
                    WHEN 'Crédito' THEN 2
                    WHEN 'Dinheiro' THEN 3
                    ELSE 1
                END;
                """);

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "Transactions");
        }
    }
}
