using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdempotencyKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_idempotency_keys_key",
                table: "idempotency_keys");

            migrationBuilder.RenameColumn(
                name: "response_metadata",
                table: "idempotency_keys",
                newName: "response_payload");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "idempotency_keys",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ux_idempotency_keys_key_user_id_key_action_type",
                table: "idempotency_keys",
                columns: new[] { "key", "user_id", "action_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_idempotency_keys_key_user_id_key_action_type",
                table: "idempotency_keys");

            migrationBuilder.DropColumn(
                name: "status",
                table: "idempotency_keys");

            migrationBuilder.RenameColumn(
                name: "response_payload",
                table: "idempotency_keys",
                newName: "response_metadata");

            migrationBuilder.CreateIndex(
                name: "ux_idempotency_keys_key",
                table: "idempotency_keys",
                column: "key",
                unique: true);
        }
    }
}
