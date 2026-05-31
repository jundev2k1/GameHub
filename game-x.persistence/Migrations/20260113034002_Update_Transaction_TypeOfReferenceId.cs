using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update_Transaction_TypeOfReferenceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE talent_wallet_transactions
                ALTER COLUMN reference_id
                TYPE uuid
                USING reference_id::uuid;");

            migrationBuilder.Sql(@"
                ALTER TABLE system_wallet_transactions
                ALTER COLUMN reference_id
                TYPE uuid
                USING reference_id::uuid;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "reference_id",
                table: "talent_wallet_transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "reference_id",
                table: "system_wallet_transactions",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
