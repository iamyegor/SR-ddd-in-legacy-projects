using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Rename_outbox_legacy_deliveries_table_to_outbox_and_add_column_named_type_to_it
        : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                ALTER TABLE outbox_legacy_deliveries 
                RENAME TO outbox;

                ALTER TABLE outbox
                ADD COLUMN type VARCHAR(100);"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                ALTER TABLE outbox
                RENAME TO outbox_legacy_deliveries;

                ALTER TABLE outbox_legacy_deliveries 
                DROP COLUMN type;"
            );
        }
    }
}
