using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyBuyingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20231002_AddedDefaultUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "policies",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 9, "GroupGet" },
                    { 10, "GroupCreate" },
                    { 11, "GroupUpdate" },
                    { 12, "GroupDelete" }
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "id", "role_id", "user_id" },
                values: new object[] { 1, 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
