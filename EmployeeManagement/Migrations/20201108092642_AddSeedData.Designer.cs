﻿// <auto-generated />
using EmployeeManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeManagement.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20201108092642_AddSeedData")]
    partial class AddSeedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EmployeeManagement.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Department")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("EmployeeID");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            EmployeeID = 1,
                            Department = 2,
                            Email = "Puru@201gmail.com",
                            Name = "PuruKC"
                        },
                        new
                        {
                            EmployeeID = 2,
                            Department = 2,
                            Email = "Puru202gmail.com",
                            Name = "PuruKC"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
