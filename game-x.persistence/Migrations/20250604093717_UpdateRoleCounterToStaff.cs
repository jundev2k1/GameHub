using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleCounterToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"AspNetRoles\" SET \"Name\" = 'Staff' WHERE \"Name\" = 'counter';"
            );
            migrationBuilder.Sql(
                "UPDATE \"AspNetRoles\" SET \"NormalizedName\" = 'STAFF' WHERE \"NormalizedName\" = 'COUNTER';"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"AspNetRoles\" SET \"Name\" = 'counter' WHERE \"Name\" = 'Staff';"
            );
            migrationBuilder.Sql(
                "UPDATE \"AspNetRoles\" SET \"NormalizedName\" = 'COUNTER' WHERE \"NormalizedName\" = 'STAFF';"
            );
        }
    }
}
