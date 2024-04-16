using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Add_sync_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sync",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    is_sync_required = table.Column<bool>(
                        type: "boolean",
                        nullable: false,
                        defaultValue: false
                    ),
                    row_version = table.Column<int>(
                        type: "integer",
                        nullable: false,
                        defaultValue: 1
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync", x => x.name);
                }
            );

            migrationBuilder.Sql(
                @"create or replace function increment_row_version()
returns trigger as $$
    begin
        new.row_version = old.row_version + 1;
        return new;
    end;
$$ language plpgsql;"
            );

            migrationBuilder.Sql(
                @"
create or replace trigger increment_row_version_on_update
before update on sync
for each row execute function increment_row_version();"
            );

            migrationBuilder.Sql("insert into sync (name) values ('Delivery')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from sync where name = 'Delivery'");
            migrationBuilder.Sql("drop trigger if exists increment_row_version_on_update on sync");
            migrationBuilder.Sql("drop function if exists increment_row_version()");
            
            migrationBuilder.DropTable(name: "sync");
        }
    }
}
