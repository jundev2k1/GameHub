using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBankAccountEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_bank_accounts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    bank_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    bank_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    account_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    account_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: ""),
                    currency_id = table.Column<int>(type: "integer", nullable: false),
                    image_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    rejection_reason = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false, defaultValue: "True"),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    date_reviewed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewed_by_id = table.Column<string>(type: "text", nullable: true),
                    reject_details = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_bank_accounts", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_bank_accounts_asp_net_users_reviewed_by_id",
                        column: x => x.reviewed_by_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_bank_accounts_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_bank_accounts_fiat_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "fiat_currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_bank_accounts_media_files_image_id",
                        column: x => x.image_id,
                        principalTable: "media_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_currency_id",
                table: "user_bank_accounts",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_image_id",
                table: "user_bank_accounts",
                column: "image_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_public_id",
                table: "user_bank_accounts",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_reviewed_by_id",
                table: "user_bank_accounts",
                column: "reviewed_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_status",
                table: "user_bank_accounts",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_submitted_at",
                table: "user_bank_accounts",
                column: "submitted_at");

            migrationBuilder.CreateIndex(
                name: "ix_user_bank_accounts_user_id",
                table: "user_bank_accounts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_bank_accounts");
        }
    }
}
