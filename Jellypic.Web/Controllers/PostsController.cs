using System;
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
        public async Task<object> Index(int? after = null)
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
                Data = posts.Select(p => new
                {
                    Id = p.Id,
                    CreatedAt = p.CreatedAt.ToEpoch(),
                    ImageUrl = p.ImageUrl,
                    User = new
                    {
                        Id = p.User.Id,
                        Username = p.User.Username,
                        PictureUrl = p.User.PictureUrl
                    },
                    Likes = new
                    {
                        Count = p.Likes.Count,
                        Data = p.Likes.Select(l => new
                        {
                            Id = l.Id,
                            CreatedAt = l.CreatedAt.ToEpoch(),
                            User = new
                            {
                                Id = l.User.Id,
                                Username = l.User.Username
                            } 
                        })
                    },
                    Comments = new
                    {
                        Count = p.Comments.Count,
                        Data = p.Comments.Select(c => new
                        {
                            Id = c.Id,
                            CreatedAt = c.CreatedAt.ToEpoch(),
                            Text = c.Text,
                            User = new
                            {
                                Id = c.User.Id,
                                Username = c.User.Username
                            }
                        })
                    }
                }),
                Pagination = new
                {
                    NextUrl = hasMore ? $"/api/posts?after={posts.Last().Id}" : null
                }

            };
        }
    }
}
