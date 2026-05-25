using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserClaimMissionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_user_mission_claims_user_reward",
                table: "user_mission_claims");

            migrationBuilder.CreateIndex(
                name: "ux_user_mission_claims_user_reward_cycle_number",
                table: "user_mission_claims",
                columns: new[] { "user_id", "mission_reward_id", "cycle_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_user_mission_claims_user_reward_cycle_number",
                table: "user_mission_claims");

            migrationBuilder.CreateIndex(
                name: "ux_user_mission_claims_user_reward",
                table: "user_mission_claims",
                columns: new[] { "user_id", "mission_reward_id" },
                unique: true);
        }
    }
}
