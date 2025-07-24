using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace game_x.persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangePrimaryKey_Counter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_counter_token_counter_id",
                table: "counter_token");

            // Drop related foreign keys before altering the counter_id columns
            DropRelatedForeignKeyUp(migrationBuilder);

            // Execute method to change the counter table
            ChangeCounterTableUp(migrationBuilder);

            // Update existing foreign key references from string to int
            migrationBuilder.Sql(@"
                UPDATE staff_counter sc
                SET    counter_id = c.counter_id
                FROM   counter c
                WHERE  sc.counter_id = c.counter_number;
            
                UPDATE staff_user su
                SET    counter_id = c.counter_id
                FROM   counter c
                WHERE  su.counter_id = c.counter_number;
            
                UPDATE ""order"" o
                SET    counter_id = c.counter_id
                FROM   counter c
                WHERE  o.counter_id = c.counter_number;
            
                UPDATE counter_token ct
                SET    counter_id = c.counter_id
                FROM   counter c
                WHERE  ct.counter_id = c.counter_number;
            ");

            // Alter the counter_id columns in related tables (staff_user) to integer type
            migrationBuilder.Sql(@"
                ALTER TABLE staff_user
                ALTER COLUMN counter_id TYPE integer USING counter_id::integer;
            ");

            // Alter the counter_id columns in related tables (staff_counter) to integer type
            migrationBuilder.Sql(@"
                ALTER TABLE staff_counter
                ALTER COLUMN counter_id TYPE integer USING counter_id::integer;
            ");

            // Alter the counter_id columns in related tables (staff_counter) to integer type
            migrationBuilder.Sql(@"
                ALTER TABLE ""order""
                ALTER COLUMN counter_id TYPE integer USING counter_id::integer;
            ");

            // Alter the counter_id columns in related tables (staff_counter) to integer type
            migrationBuilder.Sql(@"
                ALTER TABLE counter_token
                ALTER COLUMN counter_id TYPE integer USING counter_id::integer;
            ");

            // Re create foreign keys after altering the counter_id columns
            ReAddRelatedForeignKeyUp(migrationBuilder);
        }

        private void ChangeCounterTableUp(MigrationBuilder migrationBuilder)
        {
            // 1. Change column name `counter_id` → `counter_number`
            migrationBuilder.RenameColumn(
                name: "counter_id",
                table: "counter",
                newName: "counter_number");

            // 2. Add new column `counter_id` as an identity column
            migrationBuilder.AddColumn<int>(
                name: "counter_id",
                table: "counter",
                nullable: true)
            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // 3️. Assign an incremental ID to every existing record
            migrationBuilder.Sql(@"
                WITH numbered AS (
                  SELECT ctid, ROW_NUMBER() OVER (ORDER BY created_at) AS new_counter_id
                  FROM counter
                )
                UPDATE counter
                SET counter_id = numbered.new_counter_id
                FROM numbered
                WHERE counter.ctid = numbered.ctid;
            ");

            // 4️. Block NULL from now on
            migrationBuilder.AlterColumn<int>(
                name: "counter_id",
                table: "counter",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            // 5️. Synchronize sequence = MAX(counter_id) so auto-increment continues on correct number
            migrationBuilder.Sql(@"
                SELECT setval(
                    pg_get_serial_sequence('counter', 'counter_id'),
                    (SELECT MAX(counter_id) FROM counter)
                );
            ");

            // 6. Drop old primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_counter",
                table: "counter");

            // 7. Set `counter_id` is the new primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_counter",
                table: "counter",
                column: "counter_id");

            // 8. Add new column `counter_code` with default value
            migrationBuilder.AddColumn<Guid>(
                name: "counter_code",
                table: "counter",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()");

            // 9. Add foreign key constraint to `counter_token` table
            migrationBuilder.AddForeignKey(
                name: "FK_counter_counter_token_counter_id",
                table: "counter",
                column: "counter_id",
                principalTable: "counter_token",
                principalColumn: "counter_token_id",
                onDelete: ReferentialAction.Cascade);
        }

        private void DropRelatedForeignKeyUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_counter_counter_id",
                table: "order");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_counter_counter_counter_id",
                table: "staff_counter");

            migrationBuilder.DropForeignKey(
                name: "FK_staff_user_counter_counter_id",
                table: "staff_user");

            migrationBuilder.DropForeignKey(
                name: "FK_counter_token_counter_counter_id",
                table: "counter_token");
        }

        private void ReAddRelatedForeignKeyUp(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_order_counter_counter_id",
                table: "order",
                column: "counter_id",
                principalTable: "counter",
                principalColumn: "counter_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_staff_counter_counter_counter_id",
                table: "staff_counter",
                column: "counter_id",
                principalTable: "counter",
                principalColumn: "counter_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_staff_user_counter_counter_id",
                table: "staff_user",
                column: "counter_id",
                principalTable: "counter",
                principalColumn: "counter_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_counter_token_counter_counter_id",
                table: "counter_token",
                column: "counter_id",
                principalTable: "counter",
                principalColumn: "counter_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Remove additional constraints on Up
            migrationBuilder.DropForeignKey(
                name: "FK_counter_counter_token_counter_id",
                table: "counter");

            // 2. Remove current PK (set on counter_id INT)
            migrationBuilder.DropPrimaryKey(
                name: "PK_counter",
                table: "counter");

            // 3. Remove new column in Up
            migrationBuilder.DropColumn(
                name: "counter_code",
                table: "counter");

            migrationBuilder.DropColumn(
                name: "counter_id",
                table: "counter");

            // 4. Return the original column name to counter_id
            migrationBuilder.RenameColumn(
                name: "counter_number",
                table: "counter",
                newName: "counter_id");

            // 5. Restore type & constraint for counter.counter_id
            migrationBuilder.AlterColumn<string>(
                name: "counter_id",
                table: "counter",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string));          // (kiểu sau khi rename vẫn là string, dòng này chỉ bảo đảm đúng cấu trúc)

            // 6. Reset PK on string counter_id column
            migrationBuilder.AddPrimaryKey(
                name: "PK_counter",
                table: "counter",
            column: "counter_id");

            // 7.Return related tables to old string +FK

            // staff_user
            migrationBuilder.AlterColumn<string>(
                name: "counter_id",
                table: "staff_user",
                type: "character varying(4)",
                nullable: false,
                oldClrType: typeof(int));

            // staff_counter
            migrationBuilder.AlterColumn<string>(
                name: "counter_id",
                table: "staff_counter",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(int));

            // order
            migrationBuilder.AlterColumn<string>(
                name: "counter_id",
                table: "order",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(int));

            // counter_token
            migrationBuilder.AlterColumn<string>(
                name: "counter_id",
                table: "counter_token",
                type: "character varying(4)",
                nullable: false,
                oldClrType: typeof(int));

            // 8. Recreate index + original FK counter_token → counter
            migrationBuilder.CreateIndex(
                name: "IX_counter_token_counter_id",
                table: "counter_token",
                column: "counter_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_counter_token_counter_counter_id",
                table: "counter_token",
                column: "counter_id",
                principalTable: "counter",
                principalColumn: "counter_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
