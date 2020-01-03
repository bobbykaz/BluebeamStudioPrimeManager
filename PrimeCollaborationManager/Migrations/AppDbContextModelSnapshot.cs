﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PrimeCollaborationManager.Data;

namespace PrimeCollaborationManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.1");

            modelBuilder.Entity("PrimeCollaborationManager.Data.Team", b =>
                {
                    b.Property<long>("TeamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TeamId");

                    b.HasIndex("UserId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("PrimeCollaborationManager.Data.TeamMember", b =>
                {
                    b.Property<long>("TeamMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("BBUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<long>("TeamId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TeamMemberId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamMembers");
                });

            modelBuilder.Entity("PrimeCollaborationManager.Data.TeamPermission", b =>
                {
                    b.Property<long>("TeamId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("PermissionType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PermissionValue")
                        .HasColumnType("INTEGER");

                    b.HasKey("TeamId", "PermissionType");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamPermissions");
                });

            modelBuilder.Entity("PrimeCollaborationManager.Data.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("BBUserId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<long>("TeamLimit")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TeamMemberLimit")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId");

                    b.HasAlternateKey("BBUserId");

                    b.HasIndex("BBUserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PrimeCollaborationManager.Data.TeamMember", b =>
                {
                    b.HasOne("PrimeCollaborationManager.Data.Team", "Team")
                        .WithMany("TeamMembers")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PrimeCollaborationManager.Data.TeamPermission", b =>
                {
                    b.HasOne("PrimeCollaborationManager.Data.Team", "Team")
                        .WithMany("TeamPermissions")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
