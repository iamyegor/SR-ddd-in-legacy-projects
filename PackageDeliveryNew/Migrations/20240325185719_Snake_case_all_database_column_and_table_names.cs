using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Snake_case_all_database_column_and_table_names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductLines_Deliveries_DeliveryId",
                table: "ProductLines");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductLines_Products_ProductId",
                table: "ProductLines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Deliveries",
                table: "Deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Synchronization",
                table: "Synchronization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductLines",
                table: "ProductLines");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "products");

            migrationBuilder.RenameTable(
                name: "Deliveries",
                newName: "deliveries");

            migrationBuilder.RenameTable(
                name: "Synchronization",
                newName: "sync");

            migrationBuilder.RenameTable(
                name: "ProductLines",
                newName: "product_lines");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "WeightInPounds",
                table: "products",
                newName: "weight_in_pounds");

            migrationBuilder.RenameColumn(
                name: "Destination_Street",
                table: "deliveries",
                newName: "destination_street");

            migrationBuilder.RenameColumn(
                name: "Destination_State",
                table: "deliveries",
                newName: "destination_state");

            migrationBuilder.RenameColumn(
                name: "Destination_City",
                table: "deliveries",
                newName: "destination_city");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "deliveries",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsSyncNeeded",
                table: "deliveries",
                newName: "is_sync_needed");

            migrationBuilder.RenameColumn(
                name: "Destination_ZipCode",
                table: "deliveries",
                newName: "destination_zip_code");

            migrationBuilder.RenameColumn(
                name: "CostEstimate",
                table: "deliveries",
                newName: "cost_estimate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "sync",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "IsSyncRequired",
                table: "sync",
                newName: "is_sync_required");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "product_lines",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "product_lines",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "product_lines",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "product_lines",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "product_lines",
                newName: "delivery_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLines_ProductId",
                table: "product_lines",
                newName: "IX_product_lines_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_ProductLines_DeliveryId",
                table: "product_lines",
                newName: "IX_product_lines_delivery_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_deliveries",
                table: "deliveries",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sync",
                table: "sync",
                column: "name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_lines",
                table: "product_lines",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_lines_deliveries_delivery_id",
                table: "product_lines",
                column: "delivery_id",
                principalTable: "deliveries",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_lines_products_product_id",
                table: "product_lines",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_lines_deliveries_delivery_id",
                table: "product_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_product_lines_products_product_id",
                table: "product_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_deliveries",
                table: "deliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sync",
                table: "sync");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_lines",
                table: "product_lines");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "deliveries",
                newName: "Deliveries");

            migrationBuilder.RenameTable(
                name: "sync",
                newName: "Synchronization");

            migrationBuilder.RenameTable(
                name: "product_lines",
                newName: "ProductLines");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "weight_in_pounds",
                table: "Products",
                newName: "WeightInPounds");

            migrationBuilder.RenameColumn(
                name: "destination_street",
                table: "Deliveries",
                newName: "Destination_Street");

            migrationBuilder.RenameColumn(
                name: "destination_state",
                table: "Deliveries",
                newName: "Destination_State");

            migrationBuilder.RenameColumn(
                name: "destination_city",
                table: "Deliveries",
                newName: "Destination_City");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Deliveries",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_sync_needed",
                table: "Deliveries",
                newName: "IsSyncNeeded");

            migrationBuilder.RenameColumn(
                name: "destination_zip_code",
                table: "Deliveries",
                newName: "Destination_ZipCode");

            migrationBuilder.RenameColumn(
                name: "cost_estimate",
                table: "Deliveries",
                newName: "CostEstimate");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Synchronization",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "is_sync_required",
                table: "Synchronization",
                newName: "IsSyncRequired");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "ProductLines",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ProductLines",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "ProductLines",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "ProductLines",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "delivery_id",
                table: "ProductLines",
                newName: "DeliveryId");

            migrationBuilder.RenameIndex(
                name: "IX_product_lines_product_id",
                table: "ProductLines",
                newName: "IX_ProductLines_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_product_lines_delivery_id",
                table: "ProductLines",
                newName: "IX_ProductLines_DeliveryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Deliveries",
                table: "Deliveries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Synchronization",
                table: "Synchronization",
                column: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductLines",
                table: "ProductLines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLines_Deliveries_DeliveryId",
                table: "ProductLines",
                column: "DeliveryId",
                principalTable: "Deliveries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductLines_Products_ProductId",
                table: "ProductLines",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
