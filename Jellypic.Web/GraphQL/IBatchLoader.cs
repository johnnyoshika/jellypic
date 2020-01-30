using Jellypic.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jellypic.Web.GraphQL
{
    public interface IBatchLoader
    {
        Task<IDictionary<int, User>> GetUsersByIdsAsync(IEnumerable<int> ids);
        Task<IDictionary<int, Post>> GetPostsByIdsAsync(IEnumerable<int> ids);
        Task<ILookup<int, Like>> GetLikesByPostIdsAsync(IEnumerable<int> postIds);
        Task<ILookup<int, Comment>> GetCommentsByPostIdsAsync(IEnumerable<int> postIds);
    }
}
