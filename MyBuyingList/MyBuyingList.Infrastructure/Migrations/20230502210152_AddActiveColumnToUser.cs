using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBuyingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveColumnToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_buying_lists_groups_group_id",
                table: "buying_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_buying_lists_users_user_created_id",
                table: "buying_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_groups_users_user_id",
                table: "groups");

            migrationBuilder.AddColumn<bool>(
                name: "active",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValueSql: "FALSE");

            migrationBuilder.AddForeignKey(
                name: "fk_buying_lists_group_group_id",
                table: "buying_lists",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_buying_lists_user_user_created_id",
                table: "buying_lists",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_groups_user_user_id",
                table: "groups",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_buying_lists_group_group_id",
                table: "buying_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_buying_lists_user_user_created_id",
                table: "buying_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_groups_user_user_id",
                table: "groups");

            migrationBuilder.DropColumn(
                name: "active",
                table: "users");

            migrationBuilder.AddForeignKey(
                name: "fk_buying_lists_groups_group_id",
                table: "buying_lists",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_buying_lists_users_user_created_id",
                table: "buying_lists",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_groups_users_user_id",
                table: "groups",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
