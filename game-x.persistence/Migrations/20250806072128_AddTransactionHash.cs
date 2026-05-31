using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hash",
                table: "chain_transactions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_hash",
                table: "chain_transactions",
                column: "hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_chain_transactions_hash",
                table: "chain_transactions");

            migrationBuilder.DropColumn(
                name: "hash",
                table: "chain_transactions");
        }
    }
}
