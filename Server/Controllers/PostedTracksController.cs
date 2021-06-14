using HVMDash.Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PostedTracksController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly PostedTracksContext _context;
        private readonly string cacheKey = "LastPostedTracksList";
        private readonly MemoryCacheEntryOptions cacheExpiryOptions = new()
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(1),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromSeconds(30)
        };
        public PostedTracksController(IMemoryCache memoryCache, PostedTracksContext context)
        {
            this.memoryCache = memoryCache;
            _context = context;
        }


        // GET: api/PostedTracks
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PostedTrack>>> GetPostedTracks(int? page, int? pageSize)
        {
            List<PostedTrack> lastPosted = new();
            IEnumerable<PostedTrack> dataToPage;

            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<PostedTrack> lastPosted2))
            {
                lastPosted = await _context.PostedTracks.OrderByDescending(t => t.Date).ToListAsync();

                if (page != null && pageSize != null)
                {
                    int pg = page.Value; int sz = pageSize.Value;
                    dataToPage = lastPosted.Skip(pg * sz).Take(sz);
                }
                else dataToPage = lastPosted;

                memoryCache.Set(cacheKey, dataToPage, cacheExpiryOptions);
                return dataToPage.ToList();
            }

            memoryCache.TryGetValue(cacheKey, out dataToPage);
            return dataToPage.ToList();
        }


        //// GET: api/PostedTracks
        //[HttpGet()]
        //public async Task<ActionResult<IEnumerable<PostedTrack>>> GetPostedTracks([FromQuery] RequestFeatures.RequestParameters reqParameters)
        //{
        //    List<PostedTrack> lastPosted = new List<PostedTrack>();
        //    if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<PostedTrack> lastPosted2))
        //    {
        //        lastPosted =  await _context.PostedTracks.OrderByDescending(t => t.Date).ToListAsync();

        //        memoryCache.Set(cacheKey, lastPosted, cacheExpiryOptions);
        //        return lastPosted.ToList();
        //    }

        //    memoryCache.TryGetValue(cacheKey, out lastPosted);
        //    return lastPosted;
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<PostedTrack>> GetPostedTracks(int postId)
        //{
        //    var postedTracks = await _context.PostedTracks.Where(a=> a.Post.Id == postId);

        //    if (postedTracks == null)
        //    {
        //        return NotFound();
        //    }

        //    return;
        //}

        // GET: api/PostedTracks/stylecount
        //[HttpGet("/stylecount")]
        //public async Task<ActionResult<IEnumerable<int?, int>>> GetStyleCount()
        //{
        //    //var sc = _context.PostedTracks.Select(d => d.PlaylistId).Distinct().Count();
        //    var sc = _context.PostedTracks.GroupBy(t => new { t.PlaylistId}).Select(g => new { g.Key.PlaylistId, count = g.Select(l => l.PlaylistId).Distinct().Count() });
        //    return Json(await sc.ToListAsync());
        //}

        //// GET: PostedTracks/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var postedTrack = await _context.PostedTracks
        //        .Include(p => p.Playlist)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (postedTrack == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(postedTrack);
        //}

        //// GET: PostedTracks/Create
        //public IActionResult Create()
        //{
        //    ViewData["PlaylistId"] = new SelectList(_context.Set<Playlist>(), "Id", "PlaylistId");
        //    return View();
        //}

        //// POST: PostedTracks/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Trackname,Date,PlaylistId,OwnerId,MediaId")] PostedTrack postedTrack)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(postedTrack);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PlaylistId"] = new SelectList(_context.Set<Playlist>(), "Id", "PlaylistId", postedTrack.PlaylistId);
        //    return View(postedTrack);
        //}

        //// GET: PostedTracks/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var postedTrack = await _context.PostedTracks.FindAsync(id);
        //    if (postedTrack == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PlaylistId"] = new SelectList(_context.Set<Playlist>(), "Id", "PlaylistId", postedTrack.PlaylistId);
        //    return View(postedTrack);
        //}

        //// POST: PostedTracks/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Trackname,Date,PlaylistId,OwnerId,MediaId")] PostedTrack postedTrack)
        //{
        //    if (id != postedTrack.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(postedTrack);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PostedTrackExists(postedTrack.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PlaylistId"] = new SelectList(_context.Set<Playlist>(), "Id", "PlaylistId", postedTrack.PlaylistId);
        //    return View(postedTrack);
        //}

        //// GET: PostedTracks/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var postedTrack = await _context.PostedTracks
        //        .Include(p => p.Playlist)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (postedTrack == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(postedTrack);
        //}

        //// POST: PostedTracks/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var postedTrack = await _context.PostedTracks.FindAsync(id);
        //    _context.PostedTracks.Remove(postedTrack);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PostedTrackExists(int id)
        //{
        //    return _context.PostedTracks.Any(e => e.Id == id);
        //}
    }
}
