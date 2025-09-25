using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnimationForLiveStreamGift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_livestream_gifts_media_files_image_id",
                table: "livestream_gifts");

            migrationBuilder.RenameColumn(
                name: "image_id",
                table: "livestream_gifts",
                newName: "icon_id");

            migrationBuilder.RenameIndex(
                name: "ix_livestream_gifts_image_id",
                table: "livestream_gifts",
                newName: "ix_livestream_gifts_icon_id");

            migrationBuilder.AddColumn<int>(
                name: "animation_id",
                table: "livestream_gifts",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_livestream_gifts_animation_id",
                table: "livestream_gifts",
                column: "animation_id");

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_gifts_media_files_animation_id",
                table: "livestream_gifts",
                column: "animation_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_gifts_media_files_icon_id",
                table: "livestream_gifts",
                column: "icon_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_livestream_gifts_media_files_animation_id",
                table: "livestream_gifts");

            migrationBuilder.DropForeignKey(
                name: "fk_livestream_gifts_media_files_icon_id",
                table: "livestream_gifts");

            migrationBuilder.DropIndex(
                name: "ix_livestream_gifts_animation_id",
                table: "livestream_gifts");

            migrationBuilder.DropColumn(
                name: "animation_id",
                table: "livestream_gifts");

            migrationBuilder.RenameColumn(
                name: "icon_id",
                table: "livestream_gifts",
                newName: "image_id");

            migrationBuilder.RenameIndex(
                name: "ix_livestream_gifts_icon_id",
                table: "livestream_gifts",
                newName: "ix_livestream_gifts_image_id");

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_gifts_media_files_image_id",
                table: "livestream_gifts",
                column: "image_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
