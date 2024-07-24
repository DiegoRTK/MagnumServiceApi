﻿// <auto-generated />
using MagnumServiceApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MagnumServiceApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240724153829_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("MagnumServiceApi.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentRoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Player1Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Player1Wins")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Player2Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Player2Wins")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinningScore")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CurrentRoundId")
                        .IsUnique();

                    b.HasIndex("Player1Id");

                    b.HasIndex("Player2Id");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("MagnumServiceApi.Models.Move", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MoveType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("playerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("RoundId");

                    b.ToTable("Moves");
                });

            modelBuilder.Entity("MagnumServiceApi.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("MagnumServiceApi.Models.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameSessionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinnerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("MagnumServiceApi.Models.Game", b =>
                {
                    b.HasOne("MagnumServiceApi.Models.Round", "CurrentRound")
                        .WithOne("Game")
                        .HasForeignKey("MagnumServiceApi.Models.Game", "CurrentRoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MagnumServiceApi.Models.Player", "Player1")
                        .WithMany()
                        .HasForeignKey("Player1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MagnumServiceApi.Models.Player", "Player2")
                        .WithMany()
                        .HasForeignKey("Player2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentRound");

                    b.Navigation("Player1");

                    b.Navigation("Player2");
                });

            modelBuilder.Entity("MagnumServiceApi.Models.Move", b =>
                {
                    b.HasOne("MagnumServiceApi.Models.Round", "Round")
                        .WithMany()
                        .HasForeignKey("RoundId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Round");
                });

            modelBuilder.Entity("MagnumServiceApi.Models.Round", b =>
                {
                    b.Navigation("Game")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
