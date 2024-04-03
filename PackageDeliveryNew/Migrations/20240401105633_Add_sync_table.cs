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
            migrationBuilder.AddColumn<bool>(
                name: "IsSyncNeeded",
                table: "deliveries",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "sync",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    is_sync_required = table.Column<bool>(type: "boolean", nullable: false),
                    row_version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sync", x => x.name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sync");

            migrationBuilder.DropColumn(
                name: "IsSyncNeeded",
                table: "deliveries");
        }
    }
}
