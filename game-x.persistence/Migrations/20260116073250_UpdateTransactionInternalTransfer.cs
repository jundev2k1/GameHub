using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionInternalTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transactions_source_type",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "source_type",
                table: "transactions");

            migrationBuilder.AlterColumn<int>(
                name: "provider_id",
                table: "transactions_internal",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "order_number",
                table: "transactions_internal",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "receiver_id",
                table: "transactions_internal",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "source_type",
                table: "transactions_internal",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "source_type",
                table: "transactions_internal");

            migrationBuilder.DropColumn(
                name: "transferor_id",
                table: "transactions_internal");

            migrationBuilder.AlterColumn<int>(
                name: "provider_id",
                table: "transactions_internal",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "order_number",
                table: "transactions_internal",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "source_type",
                table: "transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_source_type",
                table: "transactions",
                column: "source_type");
        }
    }
}
