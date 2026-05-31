using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveStreamGiftAndDonationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "livestream_gifts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    image_id = table.Column<int>(type: "integer", nullable: true),
                    coin_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_livestream_gifts", x => x.id);
                    table.ForeignKey(
                        name: "fk_livestream_gifts_media_files_image_id",
                        column: x => x.image_id,
                        principalTable: "media_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "livestream_donations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    livestream_schedule_id = table.Column<int>(type: "integer", nullable: false),
                    donor_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    gift_id = table.Column<int>(type: "integer", nullable: true),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    donated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_livestream_donations", x => x.id);
                    table.ForeignKey(
                        name: "fk_livestream_donations_asp_net_users_donor_id",
                        column: x => x.donor_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_livestream_donations_live_stream_gifts_gift_id",
                        column: x => x.gift_id,
                        principalTable: "livestream_gifts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_livestream_donations_live_stream_schedules_livestream_sched",
                        column: x => x.livestream_schedule_id,
                        principalTable: "livestream_schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_livestream_donations_code",
                table: "livestream_donations",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_livestream_donations_donor_id",
                table: "livestream_donations",
                column: "donor_id");

            migrationBuilder.CreateIndex(
                name: "ix_livestream_donations_gift_id",
                table: "livestream_donations",
                column: "gift_id");

            migrationBuilder.CreateIndex(
                name: "ix_livestream_donations_livestream_schedule_id",
                table: "livestream_donations",
                column: "livestream_schedule_id");

            migrationBuilder.CreateIndex(
                name: "ix_livestream_gifts_image_id",
                table: "livestream_gifts",
                column: "image_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "livestream_donations");

            migrationBuilder.DropTable(
                name: "livestream_gifts");
        }
    }
}
