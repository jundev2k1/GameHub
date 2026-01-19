using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionInternalReferenceTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_internal_asp_net_users_receiver_id",
                table: "transactions_internal");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_internal_asp_net_users_transferor_id",
                table: "transactions_internal");

            migrationBuilder.DropIndex(
                name: "ix_transactions_internal_receiver_id",
                table: "transactions_internal");

            migrationBuilder.DropIndex(
                name: "ix_transactions_internal_transferor_id",
                table: "transactions_internal");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "transactions_internal");

            migrationBuilder.DropColumn(
                name: "transferor_id",
                table: "transactions_internal");

            migrationBuilder.AddColumn<int>(
                name: "reference_id",
                table: "transactions_internal",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_internal_reference_id",
                table: "transactions_internal",
                column: "reference_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_internal_transactions_reference_id",
                table: "transactions_internal",
                column: "reference_id",
                principalTable: "transactions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_internal_transactions_reference_id",
                table: "transactions_internal");

            migrationBuilder.DropIndex(
                name: "ix_transactions_internal_reference_id",
                table: "transactions_internal");

            migrationBuilder.DropColumn(
                name: "reference_id",
                table: "transactions_internal");

            migrationBuilder.AddColumn<string>(
                name: "receiver_id",
                table: "transactions_internal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transferor_id",
                table: "transactions_internal",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_internal_receiver_id",
                table: "transactions_internal",
                column: "receiver_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_internal_transferor_id",
                table: "transactions_internal",
                column: "transferor_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_internal_asp_net_users_receiver_id",
                table: "transactions_internal",
                column: "receiver_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_internal_asp_net_users_transferor_id",
                table: "transactions_internal",
                column: "transferor_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
