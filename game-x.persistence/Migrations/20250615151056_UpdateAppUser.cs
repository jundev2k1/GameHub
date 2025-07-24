using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_staff_extension_AspNetUsers_AppUserId",
                table: "staff_extension");

            migrationBuilder.DropIndex(
                name: "IX_staff_extension_AppUserId",
                table: "staff_extension");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StaffId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "staff_extension");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "staff_extension",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_staff_extension_AppUserId",
                table: "staff_extension",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StaffId",
                table: "AspNetUsers",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_staff_extension_AspNetUsers_AppUserId",
                table: "staff_extension",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
