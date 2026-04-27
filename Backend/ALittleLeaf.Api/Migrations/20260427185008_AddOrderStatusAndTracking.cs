using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALittleLeaf.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderStatusAndTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "order_status",
                table: "Bill",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "PENDING");

            migrationBuilder.AddColumn<string>(
                name: "tracking_message",
                table: "Bill",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order_status",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "tracking_message",
                table: "Bill");
        }
    }
}
