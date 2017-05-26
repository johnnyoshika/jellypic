using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Jellypic.Web.Models
{
    public class JellypicContext : DbContext
    {
        public JellypicContext(DbContextOptions<JellypicContext> options) : base(options)
        {


        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity
                    .Property(u => u.Username)
                    .HasMaxLength(200)
                    .IsRequired();

                entity
                    .HasIndex(u => u.Username)
                    .IsUnique();

                entity
                    .Property(u => u.PictureUrl)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity
                    .Property(p => p.ImageUrl)
                    .HasMaxLength(500)
                    .IsRequired();
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity
                    .HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasIndex(l => new { l.UserId, l.PostId })
                    .IsUnique();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity
                    .HasOne(c => c.User)
                    .WithMany(u => u.Comments)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .Property(c => c.Text)
                    .IsRequired();
            });
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public int ActivityCount { get; set; }
        public DateTime LastLoggedInAt { get; set; }
        public int LoginCount { get; set; }

        public List<Post> Posts { get; set; }
        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }
    }

    public class Like
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
