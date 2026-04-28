using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALittleLeaf.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBillCreatedAtTimeAndProductRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_password",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "auth_provider",
                table: "User",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "google_id",
                table: "User",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Product",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at_time",
                table: "Bill",
                type: "datetime",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "auth_provider",
                table: "User");

            migrationBuilder.DropColumn(
                name: "google_id",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "created_at_time",
                table: "Bill");

            migrationBuilder.AlterColumn<string>(
                name: "user_password",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
