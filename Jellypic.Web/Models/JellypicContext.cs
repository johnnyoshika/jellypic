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
        public DbSet<Notification> Notifications { get; set; }

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
                    .Property(u => u.FirstName)
                    .HasMaxLength(200);

                entity
                    .Property(u => u.LastName)
                    .HasMaxLength(200);

                entity
                    .Property(u => u.AuthType)
                    .HasMaxLength(50)
                    .IsRequired();

                entity
                    .Property(u => u.AuthUserId)
                    .HasMaxLength(200)
                    .IsRequired();

                entity
                    .HasIndex(u => new { u.AuthType, u.AuthUserId })
                    .IsUnique();
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity
                    .Property(p => p.CloudinaryPublicId)
                    .HasMaxLength(50)
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

            modelBuilder.Entity<Notification>(entity =>
            {
                entity
                    .HasOne(n => n.Actor)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(n => n.Recipient)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(n => n.Post)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public int ActivityCount { get; set; }
        public DateTime LastLoggedInAt { get; set; }
        public int LoginCount { get; set; }

        public string AuthType { get; set; }
        public string AuthUserId { get; set; }

        public List<Post> Posts { get; set; }
        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }

        public string PictureUrl {
            get
            {
                if (AuthType == "Facebook")
                    return $"https://res.cloudinary.com/{ConfigSettings.Current.Cloudinary.CloudName}/image/facebook/c_fill,g_auto:faces,h_152,r_max,w_152/{AuthUserId}.png";

                return null;
            }
        }

        public string ThumbUrl
        {
            get
            {
                if (AuthType == "Facebook")
                    return $"https://res.cloudinary.com/{ConfigSettings.Current.Cloudinary.CloudName}/image/facebook/c_fill,g_auto:faces,h_30,r_max,w_30/{AuthUserId}.png";

                return null;
            }
        }

        public object ToJson() =>
            new
            {
                Id,
                Username,
                FirstName,
                LastName,
                PictureUrl,
                ThumbUrl
            };
    }

    public class Post
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CloudinaryPublicId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }

        public object ToJson() =>
            new
            {
                Id,
                CreatedAt = CreatedAt.ToEpoch(),
                CloudinaryPublicId,
                User = User.ToJson(),
                Likes = Likes.Select(l => l.ToJson()),
                Comments = Comments.Select(c => c.ToJson())
            };
    }

    public class Like
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public object ToJson() =>
            new
            {
                Id,
                CreatedAt = CreatedAt.ToEpoch(),
                User = User.ToJson()
            };
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

        public object ToJson() =>
            new
            {
                Id,
                CreatedAt = CreatedAt.ToEpoch(),
                Text,
                User = User.ToJson()
            };
    }

    public class Notification
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }

        public int ActorId { get; set; }
        public User Actor { get; set; }

        public int RecipientId { get; set; }
        public User Recipient { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

        public NotificationType Type { get; set; }

        public bool Dismissed { get; set; }
    }

    public enum NotificationType
    {
        Like = 1,
        Comment = 2
    }
}
