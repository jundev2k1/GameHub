using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_UserGameSession_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User_game_sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    platform_id = table.Column<int>(type: "integer", nullable: false),
                    game_id = table.Column<int>(type: "integer", nullable: true),
                    login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    balance_snapshot = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_game_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_game_sessions_game_platforms_platform_id",
                        column: x => x.platform_id,
                        principalTable: "game_platforms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_game_sessions_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_game_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_game_id",
                table: "User_game_sessions",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_login_at",
                table: "User_game_sessions",
                column: "login_at");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_platform_id_game_id",
                table: "User_game_sessions",
                columns: new[] { "platform_id", "game_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_game_sessions_user_id",
                table: "User_game_sessions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_game_sessions");
        }
    }
}
