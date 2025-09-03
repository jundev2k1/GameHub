using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UserLedgerAddGameTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_user_id",
                table: "user_usdt_ledgers");

            migrationBuilder.AddColumn<int>(
                name: "game_transaction_id",
                table: "user_usdt_ledgers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "user_usdt_ledgers",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers",
                column: "game_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_user_id_type_timestamp",
                table: "user_usdt_ledgers",
                columns: new[] { "user_id", "type", "timestamp" });

            migrationBuilder.AddForeignKey(
                name: "fk_user_usdt_ledgers_game_transactions_game_transaction_id",
                table: "user_usdt_ledgers",
                column: "game_transaction_id",
                principalTable: "game_transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_usdt_ledgers_game_transactions_game_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_user_id_type_timestamp",
                table: "user_usdt_ledgers");

            migrationBuilder.DropColumn(
                name: "game_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.DropColumn(
                name: "type",
                table: "user_usdt_ledgers");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_user_id",
                table: "user_usdt_ledgers",
                column: "user_id");
        }
    }
}
