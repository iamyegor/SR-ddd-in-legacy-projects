﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PackageDeliveryNew.Infrastructure;

#nullable disable

namespace PackageDeliveryNew.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20240322181230_Add_detination_property_to_deliveries_table")]
    partial class Add_detination_property_to_deliveries_table
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PackageDeliveryNew.Deliveries.Delivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal?>("CostEstimate")
                        .HasColumnType("decimal(18,2)");

                    b.ComplexProperty<Dictionary<string, object>>("Destination", "PackageDeliveryNew.Deliveries.Delivery.Destination#Address", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");
                        });

                    b.HasKey("Id");

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("PackageDeliveryNew.Deliveries.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("PackageDeliveryNew.Deliveries.Delivery", b =>
                {
                    b.OwnsMany("PackageDeliveryNew.Deliveries.ProductLine", "ProductLines", b1 =>
                        {
                            b1.Property<int>("DeliveryId")
                                .HasColumnType("int");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int");

                            SqlServerPropertyBuilderExtensions.UseIdentityColumn(b1.Property<int>("Id"));

                            b1.HasKey("DeliveryId", "Id");

                            b1.ToTable("Delivery_ProductLines", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("DeliveryId");
                        });

                    b.Navigation("ProductLines");
                });
#pragma warning restore 612, 618
        }
    }
}
