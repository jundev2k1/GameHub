using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReplacedByTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_asp_net_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.AddColumn<string>(
                name: "replaced_by_token",
                table: "refresh_tokens",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_user_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_refresh_tokens_user_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "replaced_by_token",
                table: "refresh_tokens");

            migrationBuilder.AddForeignKey(
                name: "fk_refresh_tokens_asp_net_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
