using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserUsdtLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_usdt_ledgers_users_user_id",
                table: "user_usdt_ledgers");

            migrationBuilder.AddForeignKey(
                name: "fk_user_usdt_ledgers_users_user_id",
                table: "user_usdt_ledgers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_usdt_ledgers_users_user_id",
                table: "user_usdt_ledgers");

            migrationBuilder.AddForeignKey(
                name: "fk_user_usdt_ledgers_users_user_id",
                table: "user_usdt_ledgers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
