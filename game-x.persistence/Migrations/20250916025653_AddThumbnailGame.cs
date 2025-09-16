using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "thumbnail_id",
                table: "games",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_games_thumbnail_id",
                table: "games",
                column: "thumbnail_id");

            migrationBuilder.AddForeignKey(
                name: "fk_games_media_files_thumbnail_id",
                table: "games",
                column: "thumbnail_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_games_media_files_thumbnail_id",
                table: "games");

            migrationBuilder.DropIndex(
                name: "ix_games_thumbnail_id",
                table: "games");

            migrationBuilder.DropColumn(
                name: "thumbnail_id",
                table: "games");
        }
    }
}
