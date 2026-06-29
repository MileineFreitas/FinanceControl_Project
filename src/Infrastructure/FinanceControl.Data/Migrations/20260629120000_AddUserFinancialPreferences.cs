using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFinancialPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Users",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "BRL");

            migrationBuilder.AddColumn<string>(
                name: "DateFormat",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "dd/MM/yyyy");

            migrationBuilder.AddColumn<int>(
                name: "FinancialMonthStartDay",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "pt-BR");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DateFormat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FinancialMonthStartDay",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Users");
        }
    }
}
