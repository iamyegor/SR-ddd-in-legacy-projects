using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    /// <inheritdoc />
    public partial class Reconfigure_product_line_to_be_value_object : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_lines_deliveries_delivery_id",
                table: "product_lines"
            );

            migrationBuilder.AlterColumn<int>(
                name: "delivery_id",
                table: "product_lines",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true
            );

            migrationBuilder.Sql(@"ALTER TABLE product_lines drop column id;");
            migrationBuilder.Sql(@"alter table product_lines add column id serial primary key;");

            migrationBuilder.AddForeignKey(
                name: "FK_product_lines_deliveries_delivery_id",
                table: "product_lines",
                column: "delivery_id",
                principalTable: "deliveries",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_product_lines_deliveries_delivery_id",
                table: "product_lines"
            );

            migrationBuilder.AlterColumn<int>(
                name: "delivery_id",
                table: "product_lines",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer"
            );

            migrationBuilder.Sql(@"alter table product_lines drop columnn id;");
            migrationBuilder.Sql(
                @"alter table product_lines add column id uuid not null default 00000000-0000-0000-0000-000000000000"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_product_lines_deliveries_delivery_id",
                table: "product_lines",
                column: "delivery_id",
                principalTable: "deliveries",
                principalColumn: "id"
            );
        }
    }
}
