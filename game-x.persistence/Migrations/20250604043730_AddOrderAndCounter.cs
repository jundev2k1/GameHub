using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "counter",
                columns: table => new
                {
                    counter_id = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    counter_name = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    location = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false, defaultValue: ""),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_counter", x => x.counter_id);
                });

            migrationBuilder.CreateTable(
                name: "order",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_type = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    counter_id = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    staff_id = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    price_per_unit = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false, defaultValue: ""),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_order_AspNetUsers_staff_id",
                        column: x => x.staff_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_order_counter_counter_id",
                        column: x => x.counter_id,
                        principalTable: "counter",
                        principalColumn: "counter_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "staff_counter",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    counter_id = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    logout_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_counter", x => new { x.id, x.user_id, x.counter_id });
                    table.ForeignKey(
                        name: "FK_staff_counter_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_staff_counter_counter_counter_id",
                        column: x => x.counter_id,
                        principalTable: "counter",
                        principalColumn: "counter_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "staff_user",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    counter_id = table.Column<string>(type: "character varying(4)", nullable: false),
                    staff_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff_user", x => new { x.id, x.counter_id, x.user_id, x.staff_id });
                    table.ForeignKey(
                        name: "FK_staff_user_AspNetUsers_staff_id",
                        column: x => x.staff_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_staff_user_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_staff_user_counter_counter_id",
                        column: x => x.counter_id,
                        principalTable: "counter",
                        principalColumn: "counter_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_order_counter_id",
                table: "order",
                column: "counter_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_staff_id",
                table: "order",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_user_id",
                table: "order",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_counter_counter_id",
                table: "staff_counter",
                column: "counter_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_counter_user_id",
                table: "staff_counter",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_counter_id",
                table: "staff_user",
                column: "counter_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_staff_id",
                table: "staff_user",
                column: "staff_id");

            migrationBuilder.CreateIndex(
                name: "IX_staff_user_user_id",
                table: "staff_user",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order");

            migrationBuilder.DropTable(
                name: "staff_counter");

            migrationBuilder.DropTable(
                name: "staff_user");

            migrationBuilder.DropTable(
                name: "counter");
        }
    }
}
