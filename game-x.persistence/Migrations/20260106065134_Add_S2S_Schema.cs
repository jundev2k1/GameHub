using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_S2S_Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "s2s_clients",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    client_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    client_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false, defaultValue: ""),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_s2s_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "s2s_client_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    client_id = table.Column<string>(type: "text", nullable: false),
                    app_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    app_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    host = table.Column<string>(type: "text", nullable: false),
                    allowed_ips = table.Column<string>(type: "text", nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false, defaultValue: ""),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_s2s_client_settings", x => x.id);
                    table.ForeignKey(
                        name: "fk_s2s_client_settings_s2s_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "s2s_clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "s2s_credentials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    setting_id = table.Column<int>(type: "integer", nullable: false),
                    key_id = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    direction = table.Column<short>(type: "smallint", nullable: false),
                    auth_method = table.Column<short>(type: "smallint", nullable: false),
                    usage_scope = table.Column<short>(type: "smallint", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    activated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_s2s_credentials", x => x.id);
                    table.ForeignKey(
                        name: "fk_s2s_credentials_s2s_client_settings_setting_id",
                        column: x => x.setting_id,
                        principalTable: "s2s_client_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "s2s_credential_materials",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    credential_id = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    is_encrypted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_s2s_credential_materials", x => x.id);
                    table.ForeignKey(
                        name: "fk_s2s_credential_materials_s2s_credentials_credential_id",
                        column: x => x.credential_id,
                        principalTable: "s2s_credentials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_s2s_client_settings_client_id",
                table: "s2s_client_settings",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_s2s_credential_materials_credential_id",
                table: "s2s_credential_materials",
                column: "credential_id");

            migrationBuilder.CreateIndex(
                name: "ix_s2s_credentials_setting_id",
                table: "s2s_credentials",
                column: "setting_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "s2s_credential_materials");

            migrationBuilder.DropTable(
                name: "s2s_credentials");

            migrationBuilder.DropTable(
                name: "s2s_client_settings");

            migrationBuilder.DropTable(
                name: "s2s_clients");
        }
    }
}
