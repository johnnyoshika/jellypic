using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public class BatchLoader : IBatchLoader
    {
        public BatchLoader(Func<JellypicContext> context)
        {
            Context = context;
        }

        Func<JellypicContext> Context { get; }

        public async Task<IDictionary<int, User>> GetUsersByIdsAsync(IEnumerable<int> ids)
        {
            using (var context = Context())
            {
                return (await context
                    .Users
                    .Where(u => ids.Contains(u.Id))
                    .ToListAsync())
                    .ToDictionary(u => u.Id);
            }
        }

        public async Task<ILookup<int, Like>> GetLikesByPostIdsAsync(IEnumerable<int> postIds)
        {
            using (var context = Context())
            {
                return (await context
                    .Likes
                    .Where(l => postIds.Contains(l.PostId))
                    .OrderBy(l => l.Id)
                    .ToListAsync())
                    .ToLookup(l => l.PostId);
            }
        }

        public async Task<ILookup<int, Comment>> GetCommentsByPostIdsAsync(IEnumerable<int> postIds)
        {
            using (var context = Context())
            {
                return (await context
                    .Comments
                    .Where(c => postIds.Contains(c.PostId))
                    .OrderBy(c => c.Id)
                    .ToListAsync())
                    .ToLookup(c => c.PostId);
            }
        }
    }
}
