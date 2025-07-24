using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderBank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayeeAccountNumber",
                table: "order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayeeBranchCode",
                table: "order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayeeName",
                table: "order",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PayerBankName",
                table: "order",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PayeeAccountNumber",
                table: "order");

            migrationBuilder.DropColumn(
                name: "PayeeBranchCode",
                table: "order");

            migrationBuilder.DropColumn(
                name: "PayeeName",
                table: "order");

            migrationBuilder.DropColumn(
                name: "PayerBankName",
                table: "order");
        }
    }
}
