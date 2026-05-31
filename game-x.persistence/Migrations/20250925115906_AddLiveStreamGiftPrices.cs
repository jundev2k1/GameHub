using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveStreamGiftPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coin_cost",
                table: "livestream_gifts");

            migrationBuilder.CreateTable(
                name: "livestream_gift_prices",
                columns: table => new
                {
                    live_stream_gift_id = table.Column<int>(type: "integer", nullable: false),
                    crypto_token_id = table.Column<int>(type: "integer", nullable: false),
                    token_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_livestream_gift_prices", x => new { x.live_stream_gift_id, x.crypto_token_id });
                    table.ForeignKey(
                        name: "fk_livestream_gift_prices_crypto_tokens_crypto_token_id",
                        column: x => x.crypto_token_id,
                        principalTable: "crypto_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_livestream_gift_prices_livestream_gifts_live_stream_gift_id",
                        column: x => x.live_stream_gift_id,
                        principalTable: "livestream_gifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_livestream_gift_prices_crypto_token_id",
                table: "livestream_gift_prices",
                column: "crypto_token_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "livestream_gift_prices");

            migrationBuilder.AddColumn<decimal>(
                name: "coin_cost",
                table: "livestream_gifts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
