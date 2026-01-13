using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_GamePlatformBalance_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_platform_balances",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    platform_id = table.Column<int>(type: "integer", nullable: false),
                    available_balance = table.Column<decimal>(type: "numeric", nullable: false),
                    locked_balance = table.Column<decimal>(type: "numeric", nullable: false),
                    last_synced_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_platform_balances", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_platform_balances_game_platforms_platform_id",
                        column: x => x.platform_id,
                        principalTable: "game_platforms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_game_platform_balances_platform_id",
                table: "game_platform_balances",
                column: "platform_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_platform_balances_user_id_platform_id",
                table: "game_platform_balances",
                columns: new[] { "user_id", "platform_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_platform_balances");
        }
    }
}
