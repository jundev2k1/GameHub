using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropForeignKeyConfig_CounterToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_counter_counter_token_counter_id",
                table: "counter");

            migrationBuilder.CreateIndex(
                name: "IX_counter_token_counter_id",
                table: "counter_token",
                column: "counter_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_counter_token_counter_counter_id",
                table: "counter_token");

            migrationBuilder.DropIndex(
                name: "IX_counter_token_counter_id",
                table: "counter_token");
        }
    }
}
