﻿using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public static class DataContextQueries
    {
        public static async Task<PostConnection> PostConnectionAsync(this Func<JellypicContext> dataContext, Expression<Func<Post, bool>> filter)
        {
            int take = 12;

            using (var dc = dataContext())
            {
                var posts = await dc
                    .Posts
                    .Where(filter)
                    .OrderByDescending(p => p.Id)
                    .Take(take + 1)
                    .ToListAsync();

                return new PostConnection
                {
                    Nodes = posts.Take(take).ToList(),
                    PageInfo = new PageInfo
                    {
                        EndCursor = posts.Take(take).LastOrDefault()?.Id,
                        HasNextPage = posts.Count() > take
                    }
                };
            }
        }

        public static async Task<NotificationConnection> NotificationConnectionAsync(this Func<JellypicContext> dataContext, Expression<Func<Notification, bool>> filter)
        {
            int take = 12;

            using (var dc = dataContext())
            {
                var notifications = await dc
                    .Notifications
                    .Where(filter)
                    .OrderByDescending(p => p.Id)
                    .Take(take + 1)
                    .ToListAsync();

                return new NotificationConnection
                {
                    Nodes = notifications.Take(take).ToList(),
                    PageInfo = new PageInfo
                    {
                        EndCursor = notifications.Take(take).LastOrDefault()?.Id,
                        HasNextPage = notifications.Count() > take
                    }
                };
            }
        }
    }
}
