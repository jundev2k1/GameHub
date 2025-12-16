using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_SystemWallet_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_talent_wallet_transactions_talent_wallets_talent_id",
                table: "talent_wallet_transactions");

            migrationBuilder.DropIndex(
                name: "ix_talent_wallet_transactions_balance_before_balance_after",
                table: "talent_wallet_transactions");

            migrationBuilder.DropColumn(
                name: "balance_before",
                table: "talent_wallet_transactions");

            migrationBuilder.CreateTable(
                name: "system_wallets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    balance = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_wallets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_wallet_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wallet_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric", nullable: true),
                    reference_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_wallet_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_system_wallet_transactions_system_wallets_wallet_id",
                        column: x => x.wallet_id,
                        principalTable: "system_wallets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_balance_after",
                table: "talent_wallet_transactions",
                column: "balance_after");

            migrationBuilder.CreateIndex(
                name: "ix_system_wallet_transactions_balance_after",
                table: "system_wallet_transactions",
                column: "balance_after");

            migrationBuilder.CreateIndex(
                name: "ix_system_wallet_transactions_created_at",
                table: "system_wallet_transactions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_system_wallet_transactions_wallet_id",
                table: "system_wallet_transactions",
                column: "wallet_id");

            migrationBuilder.CreateIndex(
                name: "ix_system_wallets_balance",
                table: "system_wallets",
                column: "balance");

            migrationBuilder.AddForeignKey(
                name: "fk_talent_wallet_transactions_talent_wallets_talent_id",
                table: "talent_wallet_transactions",
                column: "talent_id",
                principalTable: "talent_wallets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_talent_wallet_transactions_talent_wallets_talent_id",
                table: "talent_wallet_transactions");

            migrationBuilder.DropTable(
                name: "system_wallet_transactions");

            migrationBuilder.DropTable(
                name: "system_wallets");

            migrationBuilder.DropIndex(
                name: "ix_talent_wallet_transactions_balance_after",
                table: "talent_wallet_transactions");

            migrationBuilder.AddColumn<decimal>(
                name: "balance_before",
                table: "talent_wallet_transactions",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_balance_before_balance_after",
                table: "talent_wallet_transactions",
                columns: new[] { "balance_before", "balance_after" });

            migrationBuilder.AddForeignKey(
                name: "fk_talent_wallet_transactions_talent_wallets_talent_id",
                table: "talent_wallet_transactions",
                column: "talent_id",
                principalTable: "talent_wallets",
                principalColumn: "id");
        }
    }
}
