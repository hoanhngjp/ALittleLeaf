using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ALittleLeaf.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerEntityWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Banner",
                columns: new[] { "banner_id", "created_at", "display_order", "image_url", "is_active", "public_id", "target_url", "updated_at" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "https://res.cloudinary.com/dd9umsxtf/image/upload/f_auto,q_auto/v1776268220/slider1_kzfyxn.webp", true, "slider1_kzfyxn", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "https://res.cloudinary.com/dd9umsxtf/image/upload/f_auto,q_auto/v1776268221/slider2_oryoac.webp", true, "slider2_oryoac", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, "https://res.cloudinary.com/dd9umsxtf/image/upload/f_auto,q_auto/v1776268221/slider3_gyv5pc.webp", true, "slider3_gyv5pc", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, "https://res.cloudinary.com/dd9umsxtf/image/upload/f_auto,q_auto/v1776268222/slider4_mphagw.webp", true, "slider4_mphagw", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Banner",
                keyColumn: "banner_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Banner",
                keyColumn: "banner_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Banner",
                keyColumn: "banner_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Banner",
                keyColumn: "banner_id",
                keyValue: 4);
        }
    }
}
