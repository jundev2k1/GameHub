using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_SystemWallet_UniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_system_wallets_type",
                table: "system_wallets",
                column: "type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_system_wallets_type",
                table: "system_wallets");
        }
    }
}
