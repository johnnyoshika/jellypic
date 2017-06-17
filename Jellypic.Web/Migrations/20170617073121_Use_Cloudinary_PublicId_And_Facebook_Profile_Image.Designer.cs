using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Jellypic.Web.Models;

namespace Jellypic.Web.Migrations
{
    [DbContext(typeof(JellypicContext))]
    [Migration("20170617073121_Use_Cloudinary_PublicId_And_Facebook_Profile_Image")]
    partial class Use_Cloudinary_PublicId_And_Facebook_Profile_Image
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Jellypic.Web.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("PostId");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Jellypic.Web.Models.Like", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("PostId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId", "PostId")
                        .IsUnique();

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("Jellypic.Web.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CloudinaryPublicId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Jellypic.Web.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityCount");

                    b.Property<string>("AuthType")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("AuthUserId")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("FirstName")
                        .HasMaxLength(200);

                    b.Property<DateTime>("LastActivityAt");

                    b.Property<DateTime>("LastLoggedInAt");

                    b.Property<string>("LastName")
                        .HasMaxLength(200);

                    b.Property<int>("LoginCount");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.HasIndex("AuthType", "AuthUserId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Jellypic.Web.Models.Comment", b =>
                {
                    b.HasOne("Jellypic.Web.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jellypic.Web.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Jellypic.Web.Models.Like", b =>
                {
                    b.HasOne("Jellypic.Web.Models.Post", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Jellypic.Web.Models.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Jellypic.Web.Models.Post", b =>
                {
                    b.HasOne("Jellypic.Web.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
