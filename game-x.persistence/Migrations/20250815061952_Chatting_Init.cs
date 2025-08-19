using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Chatting_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conversations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    type = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    status = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    customer_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    assigned_agent_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    last_message_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversations", x => x.id);
                    table.ForeignKey(
                        name: "fk_conversations_asp_net_users_assigned_agent_id",
                        column: x => x.assigned_agent_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_conversations_asp_net_users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "social_links",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id_min = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    user_id_max = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    requester_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    addressee_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    blocker_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    blocked_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_social_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_social_links_asp_net_users_addressee_user_id",
                        column: x => x.addressee_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_social_links_asp_net_users_blocked_user_id",
                        column: x => x.blocked_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_social_links_asp_net_users_blocker_user_id",
                        column: x => x.blocker_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_social_links_asp_net_users_requester_user_id",
                        column: x => x.requester_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    conversation_id = table.Column<int>(type: "integer", nullable: false),
                    sender_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    sender_role = table.Column<short>(type: "smallint", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    text = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    payload_json = table.Column<string>(type: "jsonb", nullable: true),
                    reply_to_message_id = table.Column<int>(type: "integer", nullable: true),
                    is_tombstone = table.Column<bool>(type: "boolean", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    edit_count = table.Column<int>(type: "integer", nullable: false),
                    current_version = table.Column<int>(type: "integer", nullable: false),
                    reactions = table.Column<string>(type: "jsonb", nullable: false, defaultValueSql: "'{}'::jsonb"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_asp_net_users_sender_user_id",
                        column: x => x.sender_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_messages_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_messages_messages_reply_to_message_id",
                        column: x => x.reply_to_message_id,
                        principalTable: "messages",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "conversation_members",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    conversation_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    role = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    left_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_read_message_id = table.Column<int>(type: "integer", nullable: true),
                    last_delivered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_conversation_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_conversation_members_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_conversation_members_conversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "conversations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_conversation_members_message_last_read_message_id",
                        column: x => x.last_read_message_id,
                        principalTable: "messages",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "message_attachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<int>(type: "integer", nullable: false),
                    media_file_id = table.Column<int>(type: "integer", nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    added_by_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    binding_status = table.Column<int>(type: "integer", nullable: false),
                    error = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_attachments_asp_net_users_added_by_user_id",
                        column: x => x.added_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_message_attachments_media_files_media_file_id",
                        column: x => x.media_file_id,
                        principalTable: "media_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_attachments_message_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "message_edit_snapshots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message_id = table.Column<int>(type: "integer", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    editor_user_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    message_kind = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    payload_json = table.Column<string>(type: "text", nullable: true),
                    reply_to_message_id = table.Column<int>(type: "integer", nullable: true),
                    is_tombstone = table.Column<bool>(type: "boolean", nullable: false),
                    attachment_ids = table.Column<List<int>>(type: "integer[]", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_edit_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "fk_message_edit_snapshots_asp_net_users_editor_user_id",
                        column: x => x.editor_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_message_edit_snapshots_messages_message_id",
                        column: x => x.message_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_message_edit_snapshots_messages_reply_to_message_id",
                        column: x => x.reply_to_message_id,
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_conversation_members_conversation_id_user_id",
                table: "conversation_members",
                columns: new[] { "conversation_id", "user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_conversation_members_last_read_message_id",
                table: "conversation_members",
                column: "last_read_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_members_user_id_conversation_id",
                table: "conversation_members",
                columns: new[] { "user_id", "conversation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_conversations_assigned_agent_id_status",
                table: "conversations",
                columns: new[] { "assigned_agent_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_conversations_customer_id_status",
                table: "conversations",
                columns: new[] { "customer_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_conversations_status_last_message_at",
                table: "conversations",
                columns: new[] { "status", "last_message_at" });

            migrationBuilder.CreateIndex(
                name: "ix_conversations_type_last_message_at",
                table: "conversations",
                columns: new[] { "type", "last_message_at" });

            migrationBuilder.CreateIndex(
                name: "ix_message_attachments_added_by_user_id",
                table: "message_attachments",
                column: "added_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachments_media_file_id",
                table: "message_attachments",
                column: "media_file_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachments_message_id_media_file_id",
                table: "message_attachments",
                columns: new[] { "message_id", "media_file_id" },
                unique: true,
                filter: "\"media_file_id\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_message_attachments_message_id_sort_order",
                table: "message_attachments",
                columns: new[] { "message_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_message_edit_snapshots_editor_user_id",
                table: "message_edit_snapshots",
                column: "editor_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_edit_snapshots_message_id_version_number",
                table: "message_edit_snapshots",
                columns: new[] { "message_id", "version_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_message_edit_snapshots_reply_to_message_id",
                table: "message_edit_snapshots",
                column: "reply_to_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_conversation_id_sent_at",
                table: "messages",
                columns: new[] { "conversation_id", "sent_at" });

            migrationBuilder.CreateIndex(
                name: "ix_messages_reply_to_message_id",
                table: "messages",
                column: "reply_to_message_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_user_id",
                table: "messages",
                column: "sender_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_social_links_addressee_user_id",
                table: "social_links",
                column: "addressee_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_social_links_blocked_user_id",
                table: "social_links",
                column: "blocked_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_social_links_blocker_user_id",
                table: "social_links",
                column: "blocker_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_social_links_kind_blocker_user_id_blocked_user_id",
                table: "social_links",
                columns: new[] { "kind", "blocker_user_id", "blocked_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_social_links_kind_state_addressee_user_id",
                table: "social_links",
                columns: new[] { "kind", "state", "addressee_user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_social_links_kind_state_user_id_max",
                table: "social_links",
                columns: new[] { "kind", "state", "user_id_max" });

            migrationBuilder.CreateIndex(
                name: "ix_social_links_kind_state_user_id_min",
                table: "social_links",
                columns: new[] { "kind", "state", "user_id_min" });

            migrationBuilder.CreateIndex(
                name: "ix_social_links_requester_user_id",
                table: "social_links",
                column: "requester_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_social_links_user_id_min_user_id_max_kind",
                table: "social_links",
                columns: new[] { "user_id_min", "user_id_max", "kind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conversation_members");

            migrationBuilder.DropTable(
                name: "message_attachments");

            migrationBuilder.DropTable(
                name: "message_edit_snapshots");

            migrationBuilder.DropTable(
                name: "social_links");

            migrationBuilder.DropTable(
                name: "messages");

            migrationBuilder.DropTable(
                name: "conversations");
        }
    }
}
