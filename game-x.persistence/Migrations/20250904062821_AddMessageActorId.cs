using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageActorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_asp_net_users_sender_user_id",
                table: "messages");

            migrationBuilder.AlterColumn<string>(
                name: "sender_user_id",
                table: "messages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<string>(
                name: "sender_actor_id",
                table: "messages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_asp_net_users_sender_user_id",
                table: "messages",
                column: "sender_user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_asp_net_users_sender_user_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "sender_actor_id",
                table: "messages");

            migrationBuilder.AlterColumn<string>(
                name: "sender_user_id",
                table: "messages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_asp_net_users_sender_user_id",
                table: "messages",
                column: "sender_user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
