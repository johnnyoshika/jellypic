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
                    p.Id,
                    CreatedAt = p.CreatedAt.ToEpoch(),
                    p.CloudinaryPublicId,
                    User = new
                    {
                        p.User.Id,
                        p.User.Username,
                        p.User.PictureUrl,
                        p.User.ThumbUrl
                    },
                    Likes = new
                    {
                        p.Likes.Count,
                        Data = p.Likes.Select(l => new
                        {
                            l.Id,
                            CreatedAt = l.CreatedAt.ToEpoch(),
                            User = new
                            {
                                l.User.Id,
                                l.User.Username
                            } 
                        })
                    },
                    Comments = new
                    {
                        p.Comments.Count,
                        Data = p.Comments.Select(c => new
                        {
                            c.Id,
                            CreatedAt = c.CreatedAt.ToEpoch(),
                            c.Text,
                            User = new
                            {
                                c.User.Id,
                                c.User.Username
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
