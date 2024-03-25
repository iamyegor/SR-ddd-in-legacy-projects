using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Drop_all_tables_again : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Synchronization");
            
            migrationBuilder.DropTable(name: "Delivery_ProductLines");

            migrationBuilder.DropTable(name: "Deliveries");

            migrationBuilder.DropTable(name: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CostEstimate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsSyncNeeded = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),
                    Destination_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination_State = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false
                    ),
                    Destination_Street = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false
                    ),
                    Destination_ZipCode = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeightInPounds = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Synchronization",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSyncRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Synchronization", x => x.Name);
                }
            );

            migrationBuilder.CreateTable(
                name: "Delivery_ProductLines",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLines_Delivery_ProductLines_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Delivery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_ProductLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "Synchronization",
                columns: new[] { "Name", "IsSyncRequired" },
                values: new object[] { "Delivery", false }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_DeliveryId",
                table: "Delivery_ProductLines",
                column: "DeliveryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductLines_ProductId",
                table: "Delivery_ProductLines",
                column: "ProductId"
            );
        }
    }
}
