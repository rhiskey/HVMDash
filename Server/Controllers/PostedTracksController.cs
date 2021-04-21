using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WAAuth.Server.Context;
using vkaudioposter_ef.parser;
using Microsoft.AspNetCore.Authorization;

namespace WAAuth.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PostedTracksController : ControllerBase
    {
        private readonly PostedTracksContext _context;

        public PostedTracksController(PostedTracksContext context)
        {
            _context = context;
        }

        // GET: api/PostedTracks
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<PostedTrack>>> GetPostedTracks()
        {
            //var postedTracksContext = _context.PostedTracks.Include(p => p.Playlist);
            var last10Posted = _context.PostedTracks.OrderByDescending(t => t.Date);
            return last10Posted.ToList();
            //return Json(await last10Posted.ToListAsync()); //return View(...);
        }

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
