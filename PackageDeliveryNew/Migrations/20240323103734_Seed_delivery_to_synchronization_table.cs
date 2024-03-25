using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Seed_delivery_to_synchronization_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Synchronization",
                columns: new[] { "Name", "IsSyncRequired" },
                values: new object[] { "Delivery", false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Synchronization",
                keyColumn: "Name",
                keyValue: "Delivery");
        }
    }
}
