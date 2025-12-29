using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Transaction_GameAmountColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "game_amount",
                table: "transactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "game_balance_after",
                table: "transactions",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_source_type",
                table: "transactions",
                column: "source_type");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_status",
                table: "transactions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_type",
                table: "transactions",
                column: "type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transactions_source_type",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_status",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_type",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "game_amount",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "game_balance_after",
                table: "transactions");
        }
    }
}
