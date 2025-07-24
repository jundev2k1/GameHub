using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEntity_MediaFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "UserPassport");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "UserPassport");

            migrationBuilder.AddColumn<int>(
                name: "passport_image_id",
                table: "UserPassport",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "media_file",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bucket_name = table.Column<string>(type: "text", nullable: false),
                    object_name = table.Column<string>(type: "text", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    mime_type = table.Column<string>(type: "text", nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    metadata = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media_file", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPassport_passport_image_id",
                table: "UserPassport",
                column: "passport_image_id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPassport_media_file_passport_image_id",
                table: "UserPassport",
                column: "passport_image_id",
                principalTable: "media_file",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPassport_media_file_passport_image_id",
                table: "UserPassport");

            migrationBuilder.DropTable(
                name: "media_file");

            migrationBuilder.DropIndex(
                name: "IX_UserPassport_passport_image_id",
                table: "UserPassport");

            migrationBuilder.DropColumn(
                name: "passport_image_id",
                table: "UserPassport");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "UserPassport",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "UserPassport",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
