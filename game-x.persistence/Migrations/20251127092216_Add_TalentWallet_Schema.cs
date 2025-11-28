using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_TalentWallet_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "talent_wallets",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    balance = table.Column<decimal>(type: "numeric", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_talent_wallets", x => x.id);
                    table.ForeignKey(
                        name: "fk_talent_wallets_asp_net_users_id",
                        column: x => x.id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "talent_wallet_transactions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    talent_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    balance_before = table.Column<decimal>(type: "numeric", nullable: false),
                    balance_after = table.Column<decimal>(type: "numeric", nullable: false),
                    reference_id = table.Column<string>(type: "text", nullable: true),
                    adjusted_by = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_talent_wallet_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_talent_wallet_transactions_talent_wallets_talent_id",
                        column: x => x.talent_id,
                        principalTable: "talent_wallets",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_balance_before_balance_after",
                table: "talent_wallet_transactions",
                columns: new[] { "balance_before", "balance_after" });

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_code",
                table: "talent_wallet_transactions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_created_at",
                table: "talent_wallet_transactions",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_talent_id",
                table: "talent_wallet_transactions",
                column: "talent_id");

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallet_transactions_type",
                table: "talent_wallet_transactions",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_talent_wallets_balance",
                table: "talent_wallets",
                column: "balance");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "talent_wallet_transactions");

            migrationBuilder.DropTable(
                name: "talent_wallets");
        }
    }
}
