using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Add_version_number_property_to_Synchronization_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER TABLE sync ADD COLUMN version_number BIGINT DEFAULT 1;");

            migrationBuilder.Sql(
                @"
                CREATE OR REPLACE FUNCTION increment_version_number()
                RETURNS TRIGGER AS $$
                BEGIN
                    NEW.version_number := OLD.version_number + 1;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER update_version_number BEFORE UPDATE ON sync
                FOR EACH ROW EXECUTE FUNCTION increment_version_number();"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS update_version_number ON sync;");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS increment_version_number;");

            migrationBuilder.Sql("ALTER TABLE sync DROP COLUMN IF EXISTS version_number;");
        }
    }
}
