using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_LiveStreamReminder_TableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_livestream_remainders_asp_net_users_user_id",
                table: "livestream_remainders");

            migrationBuilder.DropForeignKey(
                name: "fk_livestream_remainders_live_stream_schedules_schedule_id",
                table: "livestream_remainders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_livestream_remainders",
                table: "livestream_remainders");

            migrationBuilder.RenameTable(
                name: "livestream_remainders",
                newName: "livestream_reminders");

            migrationBuilder.RenameIndex(
                name: "ix_livestream_remainders_user_id",
                table: "livestream_reminders",
                newName: "ix_livestream_reminders_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_livestream_remainders_schedule_id_user_id_channel",
                table: "livestream_reminders",
                newName: "ix_livestream_reminders_schedule_id_user_id_channel");

            migrationBuilder.AddPrimaryKey(
                name: "pk_livestream_reminders",
                table: "livestream_reminders",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_reminders_asp_net_users_user_id",
                table: "livestream_reminders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_reminders_live_stream_schedules_schedule_id",
                table: "livestream_reminders",
                column: "schedule_id",
                principalTable: "livestream_schedules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_livestream_reminders_asp_net_users_user_id",
                table: "livestream_reminders");

            migrationBuilder.DropForeignKey(
                name: "fk_livestream_reminders_live_stream_schedules_schedule_id",
                table: "livestream_reminders");

            migrationBuilder.DropPrimaryKey(
                name: "pk_livestream_reminders",
                table: "livestream_reminders");

            migrationBuilder.RenameTable(
                name: "livestream_reminders",
                newName: "livestream_remainders");

            migrationBuilder.RenameIndex(
                name: "ix_livestream_reminders_user_id",
                table: "livestream_remainders",
                newName: "ix_livestream_remainders_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_livestream_reminders_schedule_id_user_id_channel",
                table: "livestream_remainders",
                newName: "ix_livestream_remainders_schedule_id_user_id_channel");

            migrationBuilder.AddPrimaryKey(
                name: "pk_livestream_remainders",
                table: "livestream_remainders",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_remainders_asp_net_users_user_id",
                table: "livestream_remainders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_livestream_remainders_live_stream_schedules_schedule_id",
                table: "livestream_remainders",
                column: "schedule_id",
                principalTable: "livestream_schedules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
