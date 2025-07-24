using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleStaffToStaffLowercase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"AspNetRoles\" SET \"Name\" = 'staff' WHERE \"Name\" = 'Staff';"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"AspNetRoles\" SET \"Name\" = 'Staff' WHERE \"Name\" = 'staff';"
            );
        }
    }
}
