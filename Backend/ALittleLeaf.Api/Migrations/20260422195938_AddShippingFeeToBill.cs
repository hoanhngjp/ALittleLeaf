using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALittleLeaf.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingFeeToBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "shipping_fee",
                table: "Bill",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shipping_fee",
                table: "Bill");
        }
    }
}
