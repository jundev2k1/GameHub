using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserMemberNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create sequence (6 digits)
            migrationBuilder.Sql(@"
                CREATE SEQUENCE IF NOT EXISTS user_member_seq
                START WITH 100001
                INCREMENT BY 1;
            ");

            // 2. Add column (nullable trước để backfill)
            migrationBuilder.AddColumn<string>(
                name: "member_number",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.Sql(@"
                ALTER TABLE users
                ALTER COLUMN member_number
                SET DEFAULT ('User' || nextval('user_member_seq'));
            ");

            migrationBuilder.Sql(@"
                UPDATE users
                SET member_number = 'User' || nextval('user_member_seq')
                WHERE member_number IS NULL;
            ");

            migrationBuilder.AlterColumn<string>(
                name: "member_number",
                table: "users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_member_number",
                table: "users",
                column: "member_number",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_member_number",
                table: "users");

            migrationBuilder.DropColumn(
                name: "member_number",
                table: "users");

            migrationBuilder.Sql(@"
                DROP SEQUENCE IF EXISTS user_member_seq;
            ");
        }
    }
}