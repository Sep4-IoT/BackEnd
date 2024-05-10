﻿// <auto-generated />
using System;
using EfcDataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EfcDataAccess.Migrations
{
    [DbContext(typeof(GreenHouseContext))]
    partial class GreenHouseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.18");

            modelBuilder.Entity("Domain.Model.GreenHouse", b =>
                {
                    b.Property<int>("GreenHouseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double?>("Co2Levels")
                        .HasColumnType("REAL");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("GreenHouseName")
                        .HasColumnType("TEXT");

                    b.Property<double?>("Humidity")
                        .HasColumnType("REAL");

                    b.Property<bool?>("IsWindowOpen")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("LightIntensity")
                        .HasColumnType("REAL");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("Temperature")
                        .HasColumnType("REAL");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("GreenHouseId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("UserId");

                    b.ToTable("GreenHouses");
                });

            modelBuilder.Entity("Domain.Model.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Model.GreenHouse", b =>
                {
                    b.HasOne("Domain.Model.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Model.User", null)
                        .WithMany("GreenHouses")
                        .HasForeignKey("UserId");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Domain.Model.User", b =>
                {
                    b.Navigation("GreenHouses");
                });
#pragma warning restore 612, 618
        }
    }
}
