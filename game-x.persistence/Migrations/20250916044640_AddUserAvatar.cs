using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "avatar_id",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_avatar_id",
                table: "users",
                column: "avatar_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_media_files_avatar_id",
                table: "users",
                column: "avatar_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_media_files_avatar_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_avatar_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "avatar_id",
                table: "users");
        }
    }
}
