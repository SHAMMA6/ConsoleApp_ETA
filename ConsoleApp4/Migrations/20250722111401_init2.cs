using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleApp_ETA_eReceipts.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptId",
                table: "ReceiptItems");

            migrationBuilder.RenameColumn(
                name: "ReceiptId",
                table: "ReceiptItems",
                newName: "ReceiptEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptItems_ReceiptId",
                table: "ReceiptItems",
                newName: "IX_ReceiptItems_ReceiptEntityId");

            migrationBuilder.AlterColumn<int>(
                name: "SellerId",
                table: "Receipts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DateTimeIssuedUtc",
                table: "Receipts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptEntityId",
                table: "ReceiptItems",
                column: "ReceiptEntityId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptEntityId",
                table: "ReceiptItems");

            migrationBuilder.RenameColumn(
                name: "ReceiptEntityId",
                table: "ReceiptItems",
                newName: "ReceiptId");

            migrationBuilder.RenameIndex(
                name: "IX_ReceiptItems_ReceiptEntityId",
                table: "ReceiptItems",
                newName: "IX_ReceiptItems_ReceiptId");

            migrationBuilder.AlterColumn<int>(
                name: "SellerId",
                table: "Receipts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTimeIssuedUtc",
                table: "Receipts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptItems_Receipts_ReceiptId",
                table: "ReceiptItems",
                column: "ReceiptId",
                principalTable: "Receipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
