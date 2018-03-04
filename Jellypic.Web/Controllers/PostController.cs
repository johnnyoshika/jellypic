﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Jellypic.Web.Infrastructure;
using Jellypic.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jellypic.Web.Controllers
{
    [Authorize]
    [Route("api/posts")]
    public class PostController : Controller
    {
        public PostController(IUserContext userContext, JellypicContext dataContext)
        {
            UserContext = userContext;
            DataContext = dataContext;
        }

        IUserContext UserContext { get; set; }
        JellypicContext DataContext { get; set; }

        [HttpGet]
        public async Task<object> Get(int? after = null)
        {
            int take = 10;

            var posts = await DataContext.ReadPosts(p => !after.HasValue || p.Id < after)
                .OrderByDescending(p => p.Id)
                .Take(take + 1)
                .ToListAsync();

            bool hasMore = posts.Count() > take;

            return new
            {
                Data = posts.Take(take).Select(p => p.ToJson()),
                Pagination = new
                {
                    NextUrl = hasMore ? $"/api/posts?after={posts.Take(take).Last().Id}" : null
                }

            };
        }

        [HttpGet("{id}")]
        public async Task<object> Get(int id)
        {
            var post = await DataContext.ReadPosts(p => p.Id == id).FirstOrDefaultAsync();
            if (post == null)
                throw new NotFoundException($"'{HttpContext.Request.Path.Value}' not found.");

            return post.ToJson();
        }

        [HttpPost]
        public async Task<object> Post([FromBody]IEnumerable<PostPostArgs> data)
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

            return new
            {
                Data = (await DataContext.ReadPosts(p => posts.Select(x => x.Id).Contains(p.Id))
                        .ToListAsync())
                    .Select(p => p.ToJson())

            };
        }
    }

    public class PostPostArgs
    {
        public string CloudinaryPublicId { get; set; }
    }
}
