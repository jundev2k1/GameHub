using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class check_pending_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_wallets_users_user_id",
                table: "wallets");

            migrationBuilder.AddForeignKey(
                name: "fk_wallets_user_user_id",
                table: "wallets",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_wallets_user_user_id",
                table: "wallets");

            migrationBuilder.AddForeignKey(
                name: "fk_wallets_users_user_id",
                table: "wallets",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
