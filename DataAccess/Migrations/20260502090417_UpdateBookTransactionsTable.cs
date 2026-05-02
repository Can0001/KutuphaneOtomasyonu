using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookTransactionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorrowDate",
                table: "BookTransactions");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "BookTransactions");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "BookTransactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "DueDate",
                table: "BookTransactions",
                newName: "TransactionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BookTransactions",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "BookTransactions",
                newName: "DueDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "BorrowDate",
                table: "BookTransactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "BookTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
