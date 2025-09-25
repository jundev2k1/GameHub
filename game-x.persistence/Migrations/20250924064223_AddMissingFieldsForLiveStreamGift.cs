using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingFieldsForLiveStreamGift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "code",
                table: "livestream_gifts",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "livestream_gifts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_livestream_gifts_code",
                table: "livestream_gifts",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_livestream_gifts_code",
                table: "livestream_gifts");

            migrationBuilder.DropColumn(
                name: "code",
                table: "livestream_gifts");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "livestream_gifts");
        }
    }
}
