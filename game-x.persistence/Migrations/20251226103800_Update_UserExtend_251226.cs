using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserExtend_251226 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_game_session_connections_connection_id",
                table: "user_game_session_connections");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "user_game_session_connections");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "user_game_session_connections",
                newName: "last_seen_at");

            migrationBuilder.AddColumn<string>(
                name: "usrex_slot_account",
                table: "user_extends",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "usrex_slot_nickname",
                table: "user_extends",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_connection_id",
                table: "user_game_session_connections",
                column: "connection_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_disconnected_at_last_seen_at",
                table: "user_game_session_connections",
                columns: new[] { "disconnected_at", "last_seen_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_game_session_connections_connection_id",
                table: "user_game_session_connections");

            migrationBuilder.DropIndex(
                name: "ix_user_game_session_connections_disconnected_at_last_seen_at",
                table: "user_game_session_connections");

            migrationBuilder.DropColumn(
                name: "usrex_slot_account",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "usrex_slot_nickname",
                table: "user_extends");

            migrationBuilder.RenameColumn(
                name: "last_seen_at",
                table: "user_game_session_connections",
                newName: "updated_at");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "user_game_session_connections",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_user_game_session_connections_connection_id",
                table: "user_game_session_connections",
                column: "connection_id");
        }
    }
}
