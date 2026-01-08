using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserExtendAtg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "atg_email",
                table: "user_extends",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "atg_fullname",
                table: "user_extends",
                type: "text",
                nullable: true,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "atg_user_name",
                table: "user_extends",
                type: "text",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "atg_email",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "atg_fullname",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "atg_user_name",
                table: "user_extends");
        }
    }
}
