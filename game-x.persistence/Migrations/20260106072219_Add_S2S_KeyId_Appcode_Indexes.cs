using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_S2S_KeyId_Appcode_Indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_s2s_credentials_key_id",
                table: "s2s_credentials",
                column: "key_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_s2s_client_settings_app_code",
                table: "s2s_client_settings",
                column: "app_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_s2s_credentials_key_id",
                table: "s2s_credentials");

            migrationBuilder.DropIndex(
                name: "ix_s2s_client_settings_app_code",
                table: "s2s_client_settings");
        }
    }
}
