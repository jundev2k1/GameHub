using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUserWithStaffUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffUserCounterId",
                table: "AspNetUsers",
                type: "character varying(4)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffUserId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffUserStaffId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaffUserUserId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StaffUserId_StaffUserCounterId_StaffUserUserId_~",
                table: "AspNetUsers",
                columns: new[] { "StaffUserId", "StaffUserCounterId", "StaffUserUserId", "StaffUserStaffId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_staff_user_StaffUserId_StaffUserCounterId_Staff~",
                table: "AspNetUsers",
                columns: new[] { "StaffUserId", "StaffUserCounterId", "StaffUserUserId", "StaffUserStaffId" },
                principalTable: "staff_user",
                principalColumns: new[] { "id", "counter_id", "user_id", "staff_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_staff_user_StaffUserId_StaffUserCounterId_Staff~",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StaffUserId_StaffUserCounterId_StaffUserUserId_~",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StaffUserCounterId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StaffUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StaffUserStaffId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StaffUserUserId",
                table: "AspNetUsers");
        }
    }
}
