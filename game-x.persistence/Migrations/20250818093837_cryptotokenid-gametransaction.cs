using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class cryptotokenidgametransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "crypto_token_id",
                table: "game_transactions",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_transactions_crypto_token_id",
                table: "game_transactions",
                column: "crypto_token_id");

            migrationBuilder.AddForeignKey(
                name: "fk_game_transactions_crypto_tokens_crypto_token_id",
                table: "game_transactions",
                column: "crypto_token_id",
                principalTable: "crypto_tokens",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_game_transactions_crypto_tokens_crypto_token_id",
                table: "game_transactions");

            migrationBuilder.DropIndex(
                name: "ix_game_transactions_crypto_token_id",
                table: "game_transactions");

            migrationBuilder.DropColumn(
                name: "crypto_token_id",
                table: "game_transactions");
        }
    }
}
