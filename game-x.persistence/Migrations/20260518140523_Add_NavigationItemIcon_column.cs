using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_NavigationItemIcon_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "icon_id",
                table: "navigation_items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_navigation_items_icon_id",
                table: "navigation_items",
                column: "icon_id");

            migrationBuilder.AddForeignKey(
                name: "fk_navigation_items_media_files_icon_id",
                table: "navigation_items",
                column: "icon_id",
                principalTable: "media_files",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_navigation_items_media_files_icon_id",
                table: "navigation_items");

            migrationBuilder.DropIndex(
                name: "ix_navigation_items_icon_id",
                table: "navigation_items");

            migrationBuilder.DropColumn(
                name: "icon_id",
                table: "navigation_items");
        }
    }
}
