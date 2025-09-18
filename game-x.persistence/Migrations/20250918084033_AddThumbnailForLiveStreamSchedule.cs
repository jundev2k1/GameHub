using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailForLiveStreamSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "thumbnail_id",
                table: "livestream_schedules",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_livestream_schedules_thumbnail_id",
                table: "livestream_schedules",
                column: "thumbnail_id");

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_schedules_media_files_thumbnail_id",
                table: "livestream_schedules",
                column: "thumbnail_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_livestream_schedules_media_files_thumbnail_id",
                table: "livestream_schedules");

            migrationBuilder.DropIndex(
                name: "ix_livestream_schedules_thumbnail_id",
                table: "livestream_schedules");

            migrationBuilder.DropColumn(
                name: "thumbnail_id",
                table: "livestream_schedules");
        }
    }
}
