using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "crypto_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    symbol = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    network = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    contract_address = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_crypto_tokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chain_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    order_number = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    transaction_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    from_address = table.Column<string>(type: "text", nullable: true),
                    to_address = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    fee = table.Column<decimal>(type: "numeric", nullable: false),
                    crypto_token_id = table.Column<int>(type: "integer", nullable: false),
                    confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    meta = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    note = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chain_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_chain_transactions_crypto_tokens_crypto_token_id",
                        column: x => x.crypto_token_id,
                        principalTable: "crypto_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_chain_transactions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_crypto_token_id",
                table: "chain_transactions",
                column: "crypto_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_public_id",
                table: "chain_transactions",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_transaction_hash",
                table: "chain_transactions",
                column: "transaction_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_user_id",
                table: "chain_transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_crypto_tokens_symbol_network",
                table: "crypto_tokens",
                columns: new[] { "symbol", "network" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chain_transactions");

            migrationBuilder.DropTable(
                name: "crypto_tokens");
        }
    }
}
