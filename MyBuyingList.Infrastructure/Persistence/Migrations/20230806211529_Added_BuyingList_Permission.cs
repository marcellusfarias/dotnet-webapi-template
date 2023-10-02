using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyBuyingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_BuyingList_Permission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "policies",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 4, "GetAllUsers" },
                    { 5, "BuyingListGet" },
                    { 6, "BuyingListCreate" },
                    { 7, "BuyingListUpdate" },
                    { 8, "BuyingListDelete" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "policies",
                keyColumn: "id",
                keyValue: 8);
        }
    }
}
