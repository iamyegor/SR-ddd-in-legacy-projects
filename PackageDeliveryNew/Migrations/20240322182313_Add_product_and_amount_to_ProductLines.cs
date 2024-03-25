using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Add_product_and_amount_to_ProductLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "Delivery_ProductLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Delivery_ProductLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ProductLines_ProductId",
                table: "Delivery_ProductLines",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_ProductLines_Products_ProductId",
                table: "Delivery_ProductLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_ProductLines_Products_ProductId",
                table: "Delivery_ProductLines");

            migrationBuilder.DropIndex(
                name: "IX_Delivery_ProductLines_ProductId",
                table: "Delivery_ProductLines");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Delivery_ProductLines");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Delivery_ProductLines");
        }
    }
}
