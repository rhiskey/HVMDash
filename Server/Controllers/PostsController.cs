using HVMDash.Server.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly PostContext _context;
        private readonly string cacheKey = "PostsList";
        private readonly MemoryCacheEntryOptions cacheExpiryOptions = new()
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(2),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromSeconds(30)
        };
        public PostsController(IMemoryCache memoryCache, PostContext context)
        {
            this.memoryCache = memoryCache;
            _context = context;
        }

        // GET: api/Posts
        //[Route("api/Posts")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts(int? page, int? pageSize)
        {
            List<Post> posts = new();
            //IEnumerable<Post> dataToPage;
            if (page != null && pageSize != null)
            {
                int pg = page.Value; int sz = pageSize.Value;

                //if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<Post> lastPosted2))
                //{
                var gmt3 = DateTime.Now;
                posts = await _context.Posts.OrderByDescending(t => t.Id)
                    .Include(a => a.PostedTracks)
                    //.ThenInclude(b=>b.)
                    .Include(c => c.PostedPhotos)
                    //.AsSplitQuery()
                    .Skip(pg * sz).Take(sz)
                    .ToListAsync();
                return posts.ToList();

                //if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<Post> lastPosted2))
                //{
                //    sz *= 3;
                //    posts = await _context.Posts.OrderByDescending(t => t.Id)
                //      .Include(a => a.PostedTracks)
                //      .Include(c => c.PostedPhotos)
                //      .Skip(pg * sz).Take(sz)
                //      .ToListAsync();
                //    memoryCache.Set(cacheKey, posts, cacheExpiryOptions);
                //    return posts.ToList();
                //}


                //memoryCache.Set(cacheKey, dataToPage, cacheExpiryOptions);
                //return dataToPage.ToList();
                //}

                //memoryCache.TryGetValue(cacheKey, out dataToPage);
                //return dataToPage.ToList();
            }
            else
            {
                if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<Post> lastPosted2))
                {
                    var gmt3 = DateTime.Now.AddHours(3);
                    posts = await _context.Posts.OrderByDescending(t => t.Id)
                    .Include(a => a.PostedTracks)
                    .Include(c => c.PostedPhotos)
                    .Where(b => b.PublishDate > gmt3)
                    .ToListAsync();

                    memoryCache.Set(cacheKey, posts, cacheExpiryOptions);
                    return posts.ToList();
                }
                memoryCache.TryGetValue(cacheKey, out posts);
                return posts.ToList();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeletePost(int entityId)
        {
            memoryCache.Remove(cacheKey);

            //var post = await _context.Posts.FindAsync(entityId);
            var p = await _context.Posts.Include(e => e.PostedPhotos).Include(e => e.PostedTracks).Where(a => a.Id == entityId).FirstAsync();
            if (p == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(p);
            await _context.SaveChangesAsync();

            return p;
        }

    }
}
