using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBuyingList.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixingBugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_buying_list_groups_group_id",
                table: "buying_list");

            migrationBuilder.DropForeignKey(
                name: "fk_buying_list_users_user_created_id",
                table: "buying_list");

            migrationBuilder.DropForeignKey(
                name: "fk_buying_list_items_buying_list_buying_list_id",
                table: "buying_list_items");

            migrationBuilder.DropForeignKey(
                name: "fk_groups_users_user_id",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "pk_buying_list",
                table: "buying_list");

            migrationBuilder.RenameTable(
                name: "buying_list",
                newName: "buying_lists");

            migrationBuilder.RenameIndex(
                name: "ix_buying_list_group_id",
                table: "buying_lists",
                newName: "ix_buying_lists_group_id");

            migrationBuilder.RenameIndex(
                name: "ix_buying_list_created_by",
                table: "buying_lists",
                newName: "ix_buying_lists_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "pk_buying_lists",
                table: "buying_lists",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_groups_created_by",
                table: "groups",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "fk_buying_list_items_buying_lists_buying_list_id",
                table: "buying_list_items",
                column: "buying_list_id",
                principalTable: "buying_lists",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_buying_list_items_buying_lists_buying_list_id",
                table: "buying_list_items");

            migrationBuilder.DropForeignKey(
                name: "fk_buying_lists_groups_group_id",
                table: "buying_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_buying_lists_users_user_created_id",
                table: "buying_lists");

            migrationBuilder.DropForeignKey(
                name: "fk_groups_users_user_id",
                table: "groups");

            migrationBuilder.DropIndex(
                name: "ix_groups_created_by",
                table: "groups");

            migrationBuilder.DropPrimaryKey(
                name: "pk_buying_lists",
                table: "buying_lists");

            migrationBuilder.RenameTable(
                name: "buying_lists",
                newName: "buying_list");

            migrationBuilder.RenameIndex(
                name: "ix_buying_lists_group_id",
                table: "buying_list",
                newName: "ix_buying_list_group_id");

            migrationBuilder.RenameIndex(
                name: "ix_buying_lists_created_by",
                table: "buying_list",
                newName: "ix_buying_list_created_by");

            migrationBuilder.AddPrimaryKey(
                name: "pk_buying_list",
                table: "buying_list",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_buying_list_groups_group_id",
                table: "buying_list",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_buying_list_users_user_created_id",
                table: "buying_list",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_buying_list_items_buying_list_buying_list_id",
                table: "buying_list_items",
                column: "buying_list_id",
                principalTable: "buying_list",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_groups_users_user_id",
                table: "groups",
                column: "id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
