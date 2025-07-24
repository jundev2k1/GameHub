using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEntity_AuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entity_name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    entity_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    action = table.Column<short>(type: "smallint", nullable: false),
                    changed_by_user_id = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "text", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: true),
                    snapshot_before = table.Column<string>(type: "jsonb", nullable: true),
                    snapshot_after = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_log_AspNetUsers_changed_by_user_id",
                        column: x => x.changed_by_user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_changed_by_user_id",
                table: "audit_log",
                column: "changed_by_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log");
        }
    }
}
