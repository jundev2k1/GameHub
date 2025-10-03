using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.gamex.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMetadataColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "metadata",
                table: "media_files",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "metadata",
                table: "livestream_chat_messages");

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                table: "livestream_chat_messages",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
                table: "livestream_chat_messages");

            migrationBuilder.AlterColumn<string>(
                name: "metadata",
                table: "media_files",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
