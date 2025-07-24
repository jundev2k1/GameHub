using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "counter_token",
                columns: table => new
                {
                    counter_token_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    counter_id = table.Column<string>(type: "character varying(4)", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    is_valid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counter_token", x => x.counter_token_id);
                    table.ForeignKey(
                        name: "FK_counter_token_counter_counter_id",
                        column: x => x.counter_id,
                        principalTable: "counter",
                        principalColumn: "counter_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_counter_token_counter_id",
                table: "counter_token",
                column: "counter_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_counter_token_token",
                table: "counter_token",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "counter_token");
        }
    }
}
