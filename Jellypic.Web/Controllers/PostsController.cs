﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        public PostsController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; set; }
        JellypicContext DataContext { get; set; }

        [HttpGet]
        public async Task<object> Get(int? after = null)
        {
            int take = 20;

            bool hasMore = DataContext
                .Posts
                .Where(p => !after.HasValue || p.Id > after)
                .OrderByDescending(p => p.Id)
                .Skip(take)
                .Any();

            var posts = await DataContext
                .Posts
                .Include(p => p.User)
                .Include("Likes.User")
                .Include("Comments.User")
                .Where(p => !after.HasValue || p.Id > after)
                .OrderByDescending(p => p.Id)
                .Take(take)
                .ToListAsync();

            return new
            {
                Data = posts.Select(p => p.ToJson()),
                Pagination = new
                {
                    NextUrl = hasMore ? $"/api/posts?after={posts.Last().Id}" : null
                }

            };
        }

        [HttpPost]
        public async Task<object> Post([FromBody]IEnumerable<PostsPostArgs> data)
        {
            var posts = new List<Post>();
            foreach (var post in data)
                posts.Add(new Post
                {
                    CreatedAt = DateTime.UtcNow,
                    CloudinaryPublicId = post.CloudinaryPublicId,
                    UserId = UserContext.UserId
                });

            DataContext.Posts.AddRange(posts);
            await DataContext.SaveChangesAsync();

            posts = await DataContext
                .Posts
                .Include(p => p.User)
                .Include("Likes.User")
                .Include("Comments.User")
                .Where(p => posts.Select(x => x.Id).Contains(p.Id))
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return new
            {
                Data = posts.Select(p => p.ToJson())

            };
        }
    }

    public class PostsPostArgs
    {
        public string CloudinaryPublicId { get; set; }
    }
}
