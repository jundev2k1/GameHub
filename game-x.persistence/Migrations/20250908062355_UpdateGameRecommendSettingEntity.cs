using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameRecommendSettingEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_game_recommend_items",
                table: "game_recommend_items");

            migrationBuilder.DropIndex(
                name: "ix_game_recommend_items_game_recommend_id",
                table: "game_recommend_items");

            migrationBuilder.DropColumn(
                name: "id",
                table: "game_recommend_items");

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_recommend_items",
                table: "game_recommend_items",
                columns: new[] { "game_recommend_id", "game_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_game_recommend_items",
                table: "game_recommend_items");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "game_recommend_items",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_game_recommend_items",
                table: "game_recommend_items",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_game_recommend_items_game_recommend_id",
                table: "game_recommend_items",
                column: "game_recommend_id");
        }
    }
}
