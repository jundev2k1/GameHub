using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserGameSessionConnection_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_game_session_connections_left_at",
                table: "user_game_session_connections");

            migrationBuilder.DropColumn(
                name: "left_at",
                table: "user_game_session_connections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "left_at",
                table: "user_game_session_connections",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_left_at",
                table: "user_game_session_connections",
                column: "left_at");
        }
    }
}
