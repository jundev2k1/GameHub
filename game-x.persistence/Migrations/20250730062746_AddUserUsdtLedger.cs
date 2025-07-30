using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserUsdtLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_usdt_ledgers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    flow_type = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    change_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    chain_transaction_id = table.Column<int>(type: "integer", nullable: true),
                    meta = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_usdt_ledgers", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_usdt_ledgers_chain_transactions_chain_transaction_id",
                        column: x => x.chain_transaction_id,
                        principalTable: "chain_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_usdt_ledgers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_chain_transaction_id",
                table: "user_usdt_ledgers",
                column: "chain_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_public_id",
                table: "user_usdt_ledgers",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_usdt_ledgers_user_id",
                table: "user_usdt_ledgers",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_usdt_ledgers");
        }
    }
}
