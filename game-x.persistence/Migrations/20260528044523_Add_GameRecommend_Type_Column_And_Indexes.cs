using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_GameRecommend_Type_Column_And_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "type",
                table: "game_recommends",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2);

            migrationBuilder.CreateIndex(
                name: "ix_game_recommends_start_date_end_date",
                table: "game_recommends",
                columns: new[] { "start_date", "end_date" });

            migrationBuilder.CreateIndex(
                name: "ix_game_recommends_type",
                table: "game_recommends",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_game_recommends_start_date_end_date",
                table: "game_recommends");

            migrationBuilder.DropIndex(
                name: "ix_game_recommends_type",
                table: "game_recommends");

            migrationBuilder.DropColumn(
                name: "type",
                table: "game_recommends");
        }
    }
}
