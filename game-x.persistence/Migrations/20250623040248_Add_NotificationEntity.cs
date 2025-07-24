using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_NotificationEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    notification_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    notification_code = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false, defaultValue: ""),
                    message = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false, defaultValue: ""),
                    user_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    type = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)2),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    severity = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.notification_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_expired_at",
                table: "notification",
                column: "expired_at");

            migrationBuilder.CreateIndex(
                name: "IX_notification_is_read",
                table: "notification",
                column: "is_read");

            migrationBuilder.CreateIndex(
                name: "IX_notification_notification_code",
                table: "notification",
                column: "notification_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_notification_severity",
                table: "notification",
                column: "severity");

            migrationBuilder.CreateIndex(
                name: "IX_notification_type",
                table: "notification",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_notification_user_id",
                table: "notification",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification");
        }
    }
}
