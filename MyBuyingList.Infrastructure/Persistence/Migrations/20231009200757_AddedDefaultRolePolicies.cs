using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyBuyingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRolePolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "role_policies",
                columns: new[] { "id", "policy_id", "role_id" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 5, 1 },
                    { 6, 6, 1 },
                    { 7, 7, 1 },
                    { 8, 8, 1 },
                    { 9, 9, 1 },
                    { 10, 10, 1 },
                    { 11, 11, 1 },
                    { 12, 12, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "role_policies",
                keyColumn: "id",
                keyValue: 12);
        }
    }
}
