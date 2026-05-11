using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesRelatedRewardMission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "idempotency_keys",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    action_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    response_metadata = table.Column<string>(type: "jsonb", maxLength: 4096, nullable: true),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_idempotency_keys", x => x.id);
                    table.ForeignKey(
                        name: "fk_idempotency_keys_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventory_item_definitions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    category = table.Column<string>(type: "text", nullable: false, defaultValue: "Ticket"),
                    monetary_value = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    icon_type = table.Column<int>(type: "integer", nullable: false),
                    icon_id = table.Column<int>(type: "integer", nullable: true),
                    icon_value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventory_item_definitions", x => x.id);
                    table.ForeignKey(
                        name: "fk_inventory_item_definitions_media_files_icon_id",
                        column: x => x.icon_id,
                        principalTable: "media_files",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "reward_pools",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "Roulette"),
                    title = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    config = table.Column<string>(type: "jsonb", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reward_pools", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    ref_type = table.Column<int>(type: "integer", nullable: true),
                    ref_id = table.Column<int>(type: "integer", nullable: true),
                    metadata = table.Column<string>(type: "jsonb", maxLength: 4096, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_events_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "inventories",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    inventory_item_definition_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inventories", x => new { x.user_id, x.inventory_item_definition_id });
                    table.ForeignKey(
                        name: "fk_inventories_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_inventories_inventory_item_definition_inventory_item_defini",
                        column: x => x.inventory_item_definition_id,
                        principalTable: "inventory_item_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "missions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    reward_config_id = table.Column<int>(type: "integer", nullable: true),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    reset_type = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    config = table.Column<string>(type: "jsonb", nullable: false),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_missions", x => x.id);
                    table.ForeignKey(
                        name: "fk_missions_reward_pool_reward_config_id",
                        column: x => x.reward_config_id,
                        principalTable: "reward_pools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "reward_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    reward_pool_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    inventory_item_code = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    item_id = table.Column<int>(type: "integer", nullable: true),
                    type = table.Column<string>(type: "text", nullable: false, defaultValue: "InventoryItem"),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, defaultValue: ""),
                    description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    amount = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    weight = table.Column<int>(type: "integer", nullable: false),
                    metadata = table.Column<string>(type: "jsonb", maxLength: 4096, nullable: true),
                    start_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reward_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_reward_items_inventory_item_definitions_item_id",
                        column: x => x.item_id,
                        principalTable: "inventory_item_definitions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reward_items_reward_pools_reward_pool_id",
                        column: x => x.reward_pool_id,
                        principalTable: "reward_pools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "executions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    reward_pool_id = table.Column<int>(type: "integer", nullable: true),
                    mission_id = table.Column<int>(type: "integer", nullable: true),
                    idempotency_key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    result_metadata = table.Column<string>(type: "jsonb", maxLength: 4096, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_executions", x => x.id);
                    table.ForeignKey(
                        name: "fk_executions_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_executions_mission_mission_id",
                        column: x => x.mission_id,
                        principalTable: "missions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_executions_reward_pool_reward_pool_id",
                        column: x => x.reward_pool_id,
                        principalTable: "reward_pools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "share_links",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    mission_id = table.Column<int>(type: "integer", nullable: false),
                    code = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    click_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_share_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_share_links_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_share_links_missions_mission_id",
                        column: x => x.mission_id,
                        principalTable: "missions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_missions",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    mission_id = table.Column<int>(type: "integer", nullable: false),
                    progress = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    streak = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "InProgress"),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    claimed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reset_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_missions", x => new { x.user_id, x.mission_id });
                    table.ForeignKey(
                        name: "fk_user_missions_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_missions_missions_mission_id",
                        column: x => x.mission_id,
                        principalTable: "missions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_rewards",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    public_id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: false),
                    execution_id = table.Column<int>(type: "integer", nullable: false),
                    reward_pool_id = table.Column<int>(type: "integer", nullable: false),
                    reward_item_id = table.Column<int>(type: "integer", nullable: false),
                    transaction_id = table.Column<int>(type: "integer", nullable: true),
                    inventory_item_definition_id = table.Column<int>(type: "integer", nullable: true),
                    reward_type = table.Column<string>(type: "text", nullable: false, defaultValue: "None"),
                    amount = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "Granted"),
                    metadata = table.Column<string>(type: "jsonb", maxLength: 4096, nullable: true),
                    granted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reward_item_id1 = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_rewards", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_rewards_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_rewards_executions_execution_id",
                        column: x => x.execution_id,
                        principalTable: "executions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_rewards_inventory_item_definitions_inventory_item_defi",
                        column: x => x.inventory_item_definition_id,
                        principalTable: "inventory_item_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_user_rewards_reward_items_reward_item_id",
                        column: x => x.reward_item_id,
                        principalTable: "reward_items",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_rewards_reward_items_reward_item_id1",
                        column: x => x.reward_item_id1,
                        principalTable: "reward_items",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_user_rewards_reward_pools_reward_pool_id",
                        column: x => x.reward_pool_id,
                        principalTable: "reward_pools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_rewards_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "transactions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_executions_idempotency",
                table: "executions",
                column: "idempotency_key");

            migrationBuilder.CreateIndex(
                name: "ix_executions_mission",
                table: "executions",
                column: "mission_id");

            migrationBuilder.CreateIndex(
                name: "ix_executions_reward_pool",
                table: "executions",
                column: "reward_pool_id");

            migrationBuilder.CreateIndex(
                name: "ix_executions_status",
                table: "executions",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_executions_user_type",
                table: "executions",
                columns: new[] { "user_id", "type" });

            migrationBuilder.CreateIndex(
                name: "ux_executions_public_id",
                table: "executions",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_idempotency_keys_expired",
                table: "idempotency_keys",
                column: "expired_at");

            migrationBuilder.CreateIndex(
                name: "ix_idempotency_keys_user_action",
                table: "idempotency_keys",
                columns: new[] { "user_id", "action_type" });

            migrationBuilder.CreateIndex(
                name: "ux_idempotency_keys_key",
                table: "idempotency_keys",
                column: "key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventories_item",
                table: "inventories",
                column: "inventory_item_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_inventories_user",
                table: "inventories",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ux_inventories_user_item",
                table: "inventories",
                columns: new[] { "user_id", "inventory_item_definition_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inventory_item_definitions_active",
                table: "inventory_item_definitions",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_item_definitions_category",
                table: "inventory_item_definitions",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_inventory_item_definitions_icon_id",
                table: "inventory_item_definitions",
                column: "icon_id");

            migrationBuilder.CreateIndex(
                name: "ux_inventory_item_definitions_code",
                table: "inventory_item_definitions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_inventory_item_definitions_public_id",
                table: "inventory_item_definitions",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_missions_date_range",
                table: "missions",
                columns: new[] { "start_at", "end_at" });

            migrationBuilder.CreateIndex(
                name: "ix_missions_reward_config_id",
                table: "missions",
                column: "reward_config_id");

            migrationBuilder.CreateIndex(
                name: "ix_missions_type_active",
                table: "missions",
                columns: new[] { "type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ux_missions_code",
                table: "missions",
                column: "code",
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ux_missions_public_id",
                table: "missions",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reward_items_active_range",
                table: "reward_items",
                columns: new[] { "start_at", "end_at" },
                filter: "deleted_at IS NULL AND is_active = true");

            migrationBuilder.CreateIndex(
                name: "ix_reward_items_inventory_type",
                table: "reward_items",
                column: "inventory_item_code",
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_reward_items_item_id",
                table: "reward_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_reward_items_pool_weight",
                table: "reward_items",
                columns: new[] { "reward_pool_id", "weight" },
                filter: "deleted_at IS NULL AND is_active = true");

            migrationBuilder.CreateIndex(
                name: "ix_reward_items_sort_order",
                table: "reward_items",
                columns: new[] { "reward_pool_id", "sort_order" },
                filter: "deleted_at IS NULL AND is_active = true");

            migrationBuilder.CreateIndex(
                name: "ux_reward_items_pool_code",
                table: "reward_items",
                columns: new[] { "reward_pool_id", "code" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_reward_items_date_range",
                table: "reward_pools",
                columns: new[] { "start_at", "end_at" });

            migrationBuilder.CreateIndex(
                name: "ix_reward_pools_code",
                table: "reward_pools",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reward_pools_type_active",
                table: "reward_pools",
                columns: new[] { "type", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_share_links_expired",
                table: "share_links",
                column: "expired_at");

            migrationBuilder.CreateIndex(
                name: "ix_share_links_mission_id",
                table: "share_links",
                column: "mission_id");

            migrationBuilder.CreateIndex(
                name: "ix_share_links_status",
                table: "share_links",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_share_links_user_mission",
                table: "share_links",
                columns: new[] { "user_id", "mission_id" });

            migrationBuilder.CreateIndex(
                name: "ux_share_links_code",
                table: "share_links",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_share_links_public_id",
                table: "share_links",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_events_ref",
                table: "user_events",
                columns: new[] { "ref_type", "ref_id" });

            migrationBuilder.CreateIndex(
                name: "ix_user_events_type_created",
                table: "user_events",
                columns: new[] { "type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_user_events_user_type_created",
                table: "user_events",
                columns: new[] { "user_id", "type", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ux_user_events_public_id",
                table: "user_events",
                column: "public_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_missions_mission_status",
                table: "user_missions",
                columns: new[] { "mission_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_missions_user_status",
                table: "user_missions",
                columns: new[] { "user_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_execution",
                table: "user_rewards",
                column: "execution_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_inventory_item_definition_id",
                table: "user_rewards",
                column: "inventory_item_definition_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_item",
                table: "user_rewards",
                column: "reward_item_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_pool",
                table: "user_rewards",
                column: "reward_pool_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_reward_item_id1",
                table: "user_rewards",
                column: "reward_item_id1");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_status",
                table: "user_rewards",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_transaction_id",
                table: "user_rewards",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_rewards_user_granted",
                table: "user_rewards",
                columns: new[] { "user_id", "granted_at" });

            migrationBuilder.CreateIndex(
                name: "ux_user_rewards_public_id",
                table: "user_rewards",
                column: "public_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idempotency_keys");

            migrationBuilder.DropTable(
                name: "inventories");

            migrationBuilder.DropTable(
                name: "share_links");

            migrationBuilder.DropTable(
                name: "user_events");

            migrationBuilder.DropTable(
                name: "user_missions");

            migrationBuilder.DropTable(
                name: "user_rewards");

            migrationBuilder.DropTable(
                name: "executions");

            migrationBuilder.DropTable(
                name: "reward_items");

            migrationBuilder.DropTable(
                name: "missions");

            migrationBuilder.DropTable(
                name: "inventory_item_definitions");

            migrationBuilder.DropTable(
                name: "reward_pools");
        }
    }
}
