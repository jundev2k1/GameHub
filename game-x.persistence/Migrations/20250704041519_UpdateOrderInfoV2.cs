using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderInfoV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "order",
                newName: "FiatAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "CryptoAmount",
                table: "order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Fee",
                table: "order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "UxmTimestamp",
                table: "order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CryptoAmount",
                table: "order");

            migrationBuilder.DropColumn(
                name: "Fee",
                table: "order");

            migrationBuilder.DropColumn(
                name: "UxmTimestamp",
                table: "order");

            migrationBuilder.RenameColumn(
                name: "FiatAmount",
                table: "order",
                newName: "Amount");
        }
    }
}
