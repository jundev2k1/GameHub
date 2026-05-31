using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddConversationMemberHiden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_conversation_members_user_id_conversation_id",
                table: "conversation_members");

            migrationBuilder.AddColumn<bool>(
                name: "is_hidden",
                table: "conversation_members",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_seen_at",
                table: "conversation_members",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_messages_conversation_id_id",
                table: "messages",
                columns: new[] { "conversation_id", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_conversation_members_user_id_conversation_id",
                table: "conversation_members",
                columns: new[] { "user_id", "conversation_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_messages_conversation_id_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_conversation_members_user_id_conversation_id",
                table: "conversation_members");

            migrationBuilder.DropColumn(
                name: "is_hidden",
                table: "conversation_members");

            migrationBuilder.DropColumn(
                name: "last_seen_at",
                table: "conversation_members");

            migrationBuilder.CreateIndex(
                name: "ix_conversation_members_user_id_conversation_id",
                table: "conversation_members",
                columns: new[] { "user_id", "conversation_id" });
        }
    }
}
