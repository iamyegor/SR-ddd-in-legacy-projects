using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Make_ProductLine_an_entity_rather_than_a_value_object : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Delivery_ProductLines");

            migrationBuilder.CreateTable(
                name: "ProductLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLines_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_DeliveryId",
                table: "ProductLines",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_ProductId",
                table: "ProductLines",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductLines");

            migrationBuilder.CreateTable(
                name: "Delivery_ProductLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery_ProductLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Delivery_ProductLines_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Delivery_ProductLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ProductLines_DeliveryId",
                table: "Delivery_ProductLines",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_ProductLines_ProductId",
                table: "Delivery_ProductLines",
                column: "ProductId");
        }
    }
}
