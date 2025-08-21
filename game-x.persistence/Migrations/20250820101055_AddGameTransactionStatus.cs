using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGameTransactionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.AddColumn<short>(
                name: "status",
                table: "game_transactions",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers",
                column: "game_transaction_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.DropColumn(
                name: "status",
                table: "game_transactions");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers",
                column: "game_transaction_id");
        }
    }
}
