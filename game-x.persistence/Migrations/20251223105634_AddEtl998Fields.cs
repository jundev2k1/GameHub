using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEtl998Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "usrex_etl998_account",
                table: "user_extends",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "usrex_etl998_nickname",
                table: "user_extends",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "usrex_etl998_password",
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
                name: "usrex_etl998_account",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "usrex_etl998_nickname",
                table: "user_extends");

            migrationBuilder.DropColumn(
                name: "usrex_etl998_password",
                table: "user_extends");
        }
    }
}
