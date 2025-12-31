using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Transaction_ReviewedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_user_user_id",
                table: "transactions");

            migrationBuilder.AddColumn<string>(
                name: "reviewed_by_id",
                table: "transactions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_reviewed_by_id",
                table: "transactions",
                column: "reviewed_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_asp_net_users_reviewed_by_id",
                table: "transactions",
                column: "reviewed_by_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_asp_net_users_user_id",
                table: "transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_asp_net_users_reviewed_by_id",
                table: "transactions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_asp_net_users_user_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_reviewed_by_id",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "reviewed_by_id",
                table: "transactions");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_user_user_id",
                table: "transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
