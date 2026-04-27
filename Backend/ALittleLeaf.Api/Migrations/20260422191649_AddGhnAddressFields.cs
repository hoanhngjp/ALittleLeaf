using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALittleLeaf.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGhnAddressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "district_id",
                table: "Address_List",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "province_id",
                table: "Address_List",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ward_code",
                table: "Address_List",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "district_id",
                table: "Address_List");

            migrationBuilder.DropColumn(
                name: "province_id",
                table: "Address_List");

            migrationBuilder.DropColumn(
                name: "ward_code",
                table: "Address_List");
        }
    }
}
