using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_UserExtend_GameBaccaratSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "urex_gp_rebateset",
                table: "user_extends",
                newName: "usrex_gp_rebateset");

            migrationBuilder.RenameColumn(
                name: "urex_gp_password",
                table: "user_extends",
                newName: "usrex_gp_password");

            migrationBuilder.RenameColumn(
                name: "urex_gp_nickname",
                table: "user_extends",
                newName: "usrex_gp_nickname");

            migrationBuilder.RenameColumn(
                name: "urex_gp_account",
                table: "user_extends",
                newName: "usrex_gp_account");

            migrationBuilder.AddColumn<string>(
                name: "usrex_gb_account",
                table: "user_extends",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "usrex_gb_nickname",
                table: "user_extends",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "usrex_gb_password",
                table: "user_extends",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "usrex_gb_account",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "usrex_gb_nickname",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "usrex_gb_password",
                table: "user_extends");

            migrationBuilder.RenameColumn(
                name: "usrex_gp_rebateset",
                table: "user_extends",
                newName: "urex_gp_rebateset");

            migrationBuilder.RenameColumn(
                name: "usrex_gp_password",
                table: "user_extends",
                newName: "urex_gp_password");

            migrationBuilder.RenameColumn(
                name: "usrex_gp_nickname",
                table: "user_extends",
                newName: "urex_gp_nickname");

            migrationBuilder.RenameColumn(
                name: "usrex_gp_account",
                table: "user_extends",
                newName: "urex_gp_account");
        }
    }
}
