﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PairedEmojiBot.Db;

#nullable disable

namespace PairedEmojiBot.Migrations
{
    [DbContext(typeof(PairedEmojiBotContext))]
    [Migration("20230424175519_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("PairedEmojiBot.Models.EmojiStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Count")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<long>("MessageId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("EmojiStatistics");
                });
#pragma warning restore 612, 618
        }
    }
}
