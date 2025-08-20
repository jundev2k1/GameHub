using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChainTransactionForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_usdt_ledgers_chain_transactions_chain_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_chain_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_chain_transaction_id",
                table: "user_usdt_ledgers",
                column: "chain_transaction_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_user_usdt_ledgers_chain_transactions_chain_transaction_id",
                table: "user_usdt_ledgers",
                column: "chain_transaction_id",
                principalTable: "chain_transactions",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_usdt_ledgers_chain_transactions_chain_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.DropIndex(
                name: "ix_user_usdt_ledgers_chain_transaction_id",
                table: "user_usdt_ledgers");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_chain_transaction_id",
                table: "user_usdt_ledgers",
                column: "chain_transaction_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_usdt_ledgers_chain_transactions_chain_transaction_id",
                table: "user_usdt_ledgers",
                column: "chain_transaction_id",
                principalTable: "chain_transactions",
                principalColumn: "id");
        }
    }
}
