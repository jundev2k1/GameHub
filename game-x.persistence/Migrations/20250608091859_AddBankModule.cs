using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBankModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "order_code",
                table: "order",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.CreateTable(
                name: "bank_account",
                columns: table => new
                {
                    bank_account_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bank_account_code = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    bank_account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    bank_account_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    bank_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    branch_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    account_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    owner_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_external = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bank_account", x => x.bank_account_id);
                    table.ForeignKey(
                        name: "FK_bank_account_AspNetUsers_owner_id",
                        column: x => x.owner_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_order_code",
                table: "order",
                column: "order_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bank_account_bank_account_code",
                table: "bank_account",
                column: "bank_account_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bank_account_owner_id",
                table: "bank_account",
                column: "owner_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bank_account");

            migrationBuilder.DropIndex(
                name: "IX_order_order_code",
                table: "order");

            migrationBuilder.DropColumn(
                name: "order_code",
                table: "order");
        }
    }
}
