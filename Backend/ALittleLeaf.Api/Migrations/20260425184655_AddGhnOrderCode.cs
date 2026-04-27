using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALittleLeaf.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGhnOrderCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ghn_order_code",
                table: "Bill",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ghn_order_code",
                table: "Bill");
        }
    }
}
