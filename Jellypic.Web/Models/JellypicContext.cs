﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Common;
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
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity
                    .Property(u => u.Nickname)
                    .HasMaxLength(200)
                    .IsRequired();

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
                    .Property(u => u.Gravatar)
                    .HasMaxLength(500);

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

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity
                    .HasOne(s => s.User)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .Property(s => s.Endpoint)
                    .HasMaxLength(2048)
                    .IsRequired();

                entity
                    .Property(s => s.P256DH)
                    .HasMaxLength(2048)
                    .IsRequired();

                entity
                    .Property(s => s.Auth)
                    .HasMaxLength(2048)
                    .IsRequired();
            });
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public int ActivityCount { get; set; }
        public DateTime LastLoggedInAt { get; set; }
        public int LoginCount { get; set; }

        public string AuthType { get; set; }
        public string AuthUserId { get; set; }

        public string Gravatar { get; set; }

        public List<Post> Posts { get; set; }
        public List<Like> Likes { get; set; }
        public List<Comment> Comments { get; set; }

        public string PictureUrl => GravatarSize(152) ?? FacebookPictureSize(152);

        public string ThumbUrl => GravatarSize(30) ?? FacebookPictureSize(30);

        // https://en.gravatar.com/site/implement/images/
        // r -> Rating, pg -> may contain rude gestures, provocatively dressed individuals, the lesser swear words, or mild violence
        // d -> Default, mp -> mystery person
        string GravatarSize(int size) => Gravatar == null ? null : $"{Gravatar}?s={size}&r=pg&d=mp";

        string FacebookPictureSize(int size)
        {
            if (AuthType == "facebook")
            {
                var cloudinary = new CloudinaryUrlBuilder();
                return cloudinary.FacebookUser(AuthUserId, size);
            }
            
            return null;
        }

        public object ToJson() =>
            new
            {
                Id,
                Nickname,
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
                Likes = Likes.OrderBy(l => l.CreatedAt).Select(l => l.ToJson()),
                Comments = Comments.OrderBy(c => c.CreatedAt).Select(c => c.ToJson())
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

        public object ToJson() =>
            new
            {
                Id,
                CreatedAt = CreatedAt.ToEpoch(),
                Actor = Actor.ToJson(),
                Post = new
                {
                    Id = Post.Id,
                    CloudinaryPublicId = Post.CloudinaryPublicId
                },
                Type
            };
    }

    public enum NotificationType
    {
        Like = 1,
        Comment = 2
    }

    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Endpoint { get; set; }
        public string P256DH { get; set; }
        public string Auth { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserAgent { get; set; }

        public object ToJson() =>
            new
            {
                Id,
                CreatedAt,
                Endpoint
            };
    }
}
