using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalletTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_wallet_user_user_id",
                table: "wallet");

            migrationBuilder.DropPrimaryKey(
                name: "pk_wallet",
                table: "wallet");

            migrationBuilder.DropIndex(
                name: "ix_wallet_user_id",
                table: "wallet");

            migrationBuilder.RenameTable(
                name: "wallet",
                newName: "wallets");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "wallets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "public_id",
                table: "wallets",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "pk_wallets",
                table: "wallets",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_wallets_public_id",
                table: "wallets",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_wallets_user_id_network",
                table: "wallets",
                columns: new[] { "user_id", "network" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_wallets_wallet_address",
                table: "wallets",
                column: "wallet_address",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_wallets_users_user_id",
                table: "wallets",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_wallets_users_user_id",
                table: "wallets");

            migrationBuilder.DropPrimaryKey(
                name: "pk_wallets",
                table: "wallets");

            migrationBuilder.DropIndex(
                name: "ix_wallets_public_id",
                table: "wallets");

            migrationBuilder.DropIndex(
                name: "ix_wallets_user_id_network",
                table: "wallets");

            migrationBuilder.DropIndex(
                name: "ix_wallets_wallet_address",
                table: "wallets");

            migrationBuilder.RenameTable(
                name: "wallets",
                newName: "wallet");

            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "wallet",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "public_id",
                table: "wallet",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "pk_wallet",
                table: "wallet",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_wallet_user_id",
                table: "wallet",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_wallet_user_user_id",
                table: "wallet",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
