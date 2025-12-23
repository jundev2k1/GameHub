using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserGameSessionConnection_Schema_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_balance_snapshot",
                table: "user_game_sessions",
                column: "balance_snapshot");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_is_end",
                table: "user_game_sessions",
                column: "is_end");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_game_sessions_balance_snapshot",
                table: "user_game_sessions");

            migrationBuilder.DropIndex(
                name: "ix_user_game_sessions_is_end",
                table: "user_game_sessions");
        }
    }
}
