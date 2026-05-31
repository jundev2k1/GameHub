using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTransactionHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_chain_transactions_transaction_hash",
                table: "chain_transactions");

            migrationBuilder.DropColumn(
                name: "transaction_hash",
                table: "chain_transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "transaction_hash",
                table: "chain_transactions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_transaction_hash",
                table: "chain_transactions",
                column: "transaction_hash",
                unique: true);
        }
    }
}
