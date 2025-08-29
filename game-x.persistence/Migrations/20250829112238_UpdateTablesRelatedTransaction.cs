using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTablesRelatedTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_external_game_platforms_game_platform_id",
                table: "transactions_external");

            migrationBuilder.DropTable(
                name: "balance_transfer_logs");

            migrationBuilder.DropTable(
                name: "user_usdt_ledgers");

            migrationBuilder.DropTable(
                name: "chain_transactions");

            migrationBuilder.DropTable(
                name: "game_transactions");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_external_game_platforms_game_platform_id",
                table: "transactions_external",
                column: "game_platform_id",
                principalTable: "game_platforms",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_external_game_platforms_game_platform_id",
                table: "transactions_external");

            migrationBuilder.CreateTable(
                name: "balance_transfer_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    crypto_token_id = table.Column<int>(type: "integer", nullable: false),
                    from_user_id = table.Column<string>(type: "text", nullable: false),
                    to_user_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fee = table.Column<decimal>(type: "numeric", nullable: false),
                    note = table.Column<string>(type: "text", nullable: true),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_balance_transfer_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_balance_transfer_logs_asp_net_users_from_user_id",
                        column: x => x.from_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_balance_transfer_logs_asp_net_users_to_user_id",
                        column: x => x.to_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_balance_transfer_logs_crypto_tokens_crypto_token_id",
                        column: x => x.crypto_token_id,
                        principalTable: "crypto_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "chain_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    crypto_token_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    fee = table.Column<decimal>(type: "numeric", nullable: false),
                    from_address = table.Column<string>(type: "text", nullable: true),
                    hash = table.Column<string>(type: "text", nullable: true),
                    meta = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    note = table.Column<string>(type: "text", nullable: true),
                    order_number = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    order_uid = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    to_address = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
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
                        name: "fk_chain_transactions_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "game_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    crypto_token_id = table.Column<int>(type: "integer", nullable: true),
                    game_platform_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    g598_sno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    meta = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true, defaultValue: ""),
                    code = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_game_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_game_transactions_crypto_tokens_crypto_token_id",
                        column: x => x.crypto_token_id,
                        principalTable: "crypto_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_transactions_game_platforms_game_platform_id",
                        column: x => x.game_platform_id,
                        principalTable: "game_platforms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_game_transactions_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "user_usdt_ledgers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chain_transaction_id = table.Column<int>(type: "integer", nullable: true),
                    game_transaction_id = table.Column<int>(type: "integer", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    balance_after = table.Column<decimal>(type: "numeric", nullable: false),
                    change_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    flow_type = table.Column<int>(type: "integer", nullable: false),
                    meta = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    source_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    status_at_event = table.Column<string>(type: "text", nullable: true, defaultValue: ""),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_usdt_ledgers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_usdt_ledgers_chain_transactions_chain_transaction_id",
                        column: x => x.chain_transaction_id,
                        principalTable: "chain_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_usdt_ledgers_game_transactions_game_transaction_id",
                        column: x => x.game_transaction_id,
                        principalTable: "game_transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_usdt_ledgers_user_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_balance_transfer_logs_crypto_token_id",
                table: "balance_transfer_logs",
                column: "crypto_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_balance_transfer_logs_from_user_id",
                table: "balance_transfer_logs",
                column: "from_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_balance_transfer_logs_public_id",
                table: "balance_transfer_logs",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_balance_transfer_logs_to_user_id",
                table: "balance_transfer_logs",
                column: "to_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_crypto_token_id",
                table: "chain_transactions",
                column: "crypto_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_hash",
                table: "chain_transactions",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_order_number",
                table: "chain_transactions",
                column: "order_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_public_id",
                table: "chain_transactions",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_chain_transactions_user_id",
                table: "chain_transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_transactions_crypto_token_id",
                table: "game_transactions",
                column: "crypto_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_transactions_g598sno",
                table: "game_transactions",
                column: "g598_sno",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_transactions_game_platform_id",
                table: "game_transactions",
                column: "game_platform_id");

            migrationBuilder.CreateIndex(
                name: "ix_game_transactions_public_id",
                table: "game_transactions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_game_transactions_user_id",
                table: "game_transactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_chain_transaction_id",
                table: "user_usdt_ledgers",
                column: "chain_transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_game_transaction_id",
                table: "user_usdt_ledgers",
                column: "game_transaction_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_public_id",
                table: "user_usdt_ledgers",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_user_id_type_timestamp",
                table: "user_usdt_ledgers",
                columns: new[] { "user_id", "type", "timestamp" });

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_external_game_platforms_game_platform_id",
                table: "transactions_external",
                column: "game_platform_id",
                principalTable: "game_platforms",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
