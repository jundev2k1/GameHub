using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMessageAttachmentActorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "added_by_actor_id",
                table: "message_attachments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);
            
            migrationBuilder.AlterColumn<string>(
                name: "added_by_user_id",
                table: "message_attachments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "added_by_user_id",
                table: "message_attachments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldNullable: true);
            
            migrationBuilder.DropColumn(
                name: "added_by_actor_id",
                table: "message_attachments");
        }
    }
}
