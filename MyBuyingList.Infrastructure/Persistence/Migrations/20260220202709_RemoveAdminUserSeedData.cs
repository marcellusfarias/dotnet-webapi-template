using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBuyingList.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAdminUserSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "active", "email", "password", "user_name" },
                values: new object[] { 1, true, "marcelluscfarias@gmail.com", "$2a$16$CZ18qbFWtcoAY6SnsqNYnO1H.D3It5TTD6uuhTFyjge5I/n5SRLKe", "admin" });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "id", "role_id", "user_id" },
                values: new object[] { 1, 1, 1 });
        }
    }
}
