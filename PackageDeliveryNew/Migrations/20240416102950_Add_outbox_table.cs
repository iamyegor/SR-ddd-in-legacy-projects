using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Add_outbox_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
create table outbox (
    id serial primary key,
    content jsonb not null,
    type text not null
)"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop table if exists outbox");
        }
    }
}
