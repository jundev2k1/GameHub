using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInteractionRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "interaction_rules",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    event_type = table.Column<short>(type: "smallint", nullable: false),
                    condition_expression = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    repeat_policy = table.Column<short>(type: "smallint", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interaction_rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "interaction_rule_messages",
                columns: table => new
                {
                    rule_id = table.Column<int>(type: "integer", nullable: false),
                    language_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    text = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    voice_media_id = table.Column<int>(type: "integer", nullable: true),
                    character_id = table.Column<int>(type: "integer", nullable: false),
                    pose_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_interaction_rule_messages", x => new { x.rule_id, x.language_code });
                    table.ForeignKey(
                        name: "fk_interaction_rule_messages_interaction_character_poses_pose_",
                        column: x => x.pose_id,
                        principalTable: "interaction_character_poses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_interaction_rule_messages_interaction_characters_character_",
                        column: x => x.character_id,
                        principalTable: "interaction_characters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_interaction_rule_messages_interaction_rules_rule_id",
                        column: x => x.rule_id,
                        principalTable: "interaction_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_interaction_rule_messages_media_files_voice_media_id",
                        column: x => x.voice_media_id,
                        principalTable: "media_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_interaction_rule_messages_character_id",
                table: "interaction_rule_messages",
                column: "character_id");

            migrationBuilder.CreateIndex(
                name: "ix_interaction_rule_messages_pose_id",
                table: "interaction_rule_messages",
                column: "pose_id");

            migrationBuilder.CreateIndex(
                name: "ix_interaction_rule_messages_voice_media_id",
                table: "interaction_rule_messages",
                column: "voice_media_id");

            migrationBuilder.CreateIndex(
                name: "ix_interaction_rules_code",
                table: "interaction_rules",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "interaction_rule_messages");

            migrationBuilder.DropTable(
                name: "interaction_rules");
        }
    }
}
