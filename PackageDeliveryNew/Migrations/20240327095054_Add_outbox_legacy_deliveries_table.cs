using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Add_outbox_legacy_deliveries_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
                CREATE TABLE outbox_legacy_deliveries (
                    id SERIAL PRIMARY KEY, -- This is a unique identifier for each record
                    content JSON NOT NULL -- This is the column where you'll store serialized JSON
                )"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS outbox_legacy_deliveries;");
        }
    }
}
