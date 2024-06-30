﻿// <auto-generated />
using System;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240614202726_Charts")]
    partial class Charts
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.5");

            modelBuilder.Entity("Infrastructure.Database.Entities.EspClientSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("FanOn")
                        .HasColumnType("INTEGER");

                    b.Property<double>("MaxHumidity")
                        .HasColumnType("REAL");

                    b.Property<int>("MaxIllumination")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxSoilHumidity")
                        .HasColumnType("INTEGER");

                    b.Property<double>("MaxTemperature")
                        .HasColumnType("REAL");

                    b.Property<double>("MinHumidity")
                        .HasColumnType("REAL");

                    b.Property<int>("MinIllumination")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinSoilHumidity")
                        .HasColumnType("INTEGER");

                    b.Property<double>("MinTemperature")
                        .HasColumnType("REAL");

                    b.Property<int>("RgbBrightness")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RgbColor")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("RgbMode")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("RgbPower")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("WindowOpen")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("EspClientSettings");
                });

            modelBuilder.Entity("Infrastructure.Database.Entities.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Infrastructure.Database.HumidityChartsData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("DateTime");

                    b.ToTable("HumidityChartsData");
                });

            modelBuilder.Entity("Infrastructure.Database.IlluminationChartsData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("DateTime");

                    b.ToTable("IlluminationChartsData");
                });

            modelBuilder.Entity("Infrastructure.Database.SoilHumidityChartsData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("DateTime");

                    b.ToTable("SoilHumidityChartsData");
                });

            modelBuilder.Entity("Infrastructure.Database.TemperatureChartsData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("DateTime");

                    b.ToTable("TemperatureChartsData");
                });
#pragma warning restore 612, 618
        }
    }
}