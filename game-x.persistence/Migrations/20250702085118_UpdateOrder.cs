using game_x.domain.Enum;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CryptoType",
                table: "order",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FiatType",
                table: "order",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PricingMode",
                table: "order",
                type: "integer",
                nullable: false,
                defaultValue: PricingMode.FiatAmountFixed);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "order");

            migrationBuilder.DropColumn(
                name: "CryptoType",
                table: "order");

            migrationBuilder.DropColumn(
                name: "FiatType",
                table: "order");

            migrationBuilder.DropColumn(
                name: "PricingMode",
                table: "order");
        }
    }
}
