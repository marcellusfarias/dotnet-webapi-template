using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBuyingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Added_one_more_role : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "name" },
                values: new object[] { 2, "RegularUser" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "roles",
                keyColumn: "id",
                keyValue: 2);
        }
    }
}
