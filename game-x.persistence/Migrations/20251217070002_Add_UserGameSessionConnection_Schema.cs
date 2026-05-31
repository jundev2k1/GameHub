using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserGameSessionConnection_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_game_sessions_users_user_id",
                table: "User_game_sessions");

            migrationBuilder.DropIndex(
                name: "ix_user_game_sessions_login_at",
                table: "User_game_sessions");

            migrationBuilder.DropColumn(
                name: "login_at",
                table: "User_game_sessions");

            migrationBuilder.RenameTable(
                name: "User_game_sessions",
                newName: "user_game_sessions");

            migrationBuilder.AddColumn<bool>(
                name: "is_end",
                table: "user_game_sessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "user_game_session_connections",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_game_session_id = table.Column<int>(type: "integer", nullable: false),
                    connection_id = table.Column<string>(type: "text", nullable: false),
                    connected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    disconnected_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    left_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_game_session_connections", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_game_session_connections_user_game_sessions_user_game_",
                        column: x => x.user_game_session_id,
                        principalTable: "user_game_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_connected_at_disconnected_at",
                table: "user_game_session_connections",
                columns: new[] { "connected_at", "disconnected_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_connection_id",
                table: "user_game_session_connections",
                column: "connection_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_left_at",
                table: "user_game_session_connections",
                column: "left_at");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_user_game_session_id",
                table: "user_game_session_connections",
                column: "user_game_session_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_game_sessions_user_user_id",
                table: "user_game_sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_game_sessions_user_user_id",
                table: "user_game_sessions");

            migrationBuilder.DropTable(
                name: "user_game_session_connections");

            migrationBuilder.DropColumn(
                name: "is_end",
                table: "user_game_sessions");

            migrationBuilder.RenameTable(
                name: "user_game_sessions",
                newName: "User_game_sessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "login_at",
                table: "User_game_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_login_at",
                table: "User_game_sessions",
                column: "login_at");

            migrationBuilder.AddForeignKey(
                name: "fk_user_game_sessions_users_user_id",
                table: "User_game_sessions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
