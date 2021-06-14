using HVMDash.Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly PlaylistContext _context;
        private readonly string cacheKey = "AllPlaylists";
        private readonly ILogger<PlaylistsController> _logger;

        private readonly MemoryCacheEntryOptions cacheExpiryOptions = new()
        {
            AbsoluteExpiration = DateTime.Now.AddDays(7),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromDays(3)
        };

        public PlaylistsController(IMemoryCache memoryCache, PlaylistContext context, ILogger<PlaylistsController> logger)
        {
            this.memoryCache = memoryCache;
            _context = context;
            _logger = logger;
        }

        //private readonly ILogger<PlaylistsController> _logger;

        //public PlaylistsController(ILogger<PlaylistsController> logger)
        //{
        //    _logger = logger;
        //}

        // GET: api/Playlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylists()
        {

            List<Playlist> playlists = new();
            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<Playlist> playlists2))
            {
                playlists = await _context.Playlists.OrderBy(p => p.Status).ThenBy(p => p.PlaylistName).ToListAsync();

                memoryCache.Set(cacheKey, playlists, cacheExpiryOptions);
                return playlists;
            }
            memoryCache.TryGetValue(cacheKey, out playlists);
            return playlists;
        }

        // GET: api/Playlists/export
        [HttpGet("export")]
        public async Task<JsonResult> ExportPlaylists()
        {

            List<Playlist> playlists = new();

            playlists = await _context.Playlists.OrderBy(p => p.Status).ThenBy(p => p.PlaylistName).ToListAsync();

            var jsonString = JsonSerializer.Serialize(playlists);
            //return CreatedAtAction("ExportPlaylists", jsonString); ;
            return new JsonResult(jsonString);
            //return File(jsonString, "application/force-download");

        }

        //[HttpGet]
        //public IEnumerable<Playlist> Get()
        //{
        //    return _context.Playlists.OrderBy(p => p.Status).ThenBy(p => p.PlaylistName).ToList();
        //}

        // GET: api/Playlists/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Playlist>> GetPlaylist(int id)
        {
            var playlist = await _context.Playlists.FindAsync(id);

            if (playlist == null)
            {
                return NotFound();
            }

            return playlist;
        }

        // GET: api/Playlists/uri/uri
        [HttpGet("uri/{uri}")]
        public async Task<ActionResult<Playlist>> GetPlaylistByUri(string uri)
        {
            var playlist = await _context.Playlists.Where(a => a.PlaylistId == uri).FirstOrDefaultAsync();

            if (playlist == null)
            {
                return NotFound();
            }

            return playlist;
        }

        //// GET: api/Playlists?playlistName=name
        //[HttpGet("{id}/{playlistName}")]
        //public async Task<ActionResult<Playlist>> GetPlaylist(int id, string name)
        //{
        //    var playlist = _context.Playlists.FirstOrDefault(a => a.PlaylistName == name && a.Id == id);

        //    if (playlist == null)
        //    {
        //        return NotFound();
        //    }

        //    return playlist;
        //}

        // PUT: api/Playlists/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<ActionResult<Playlist>> PutPlaylist(int id, Playlist playlist)
        //public async Task<IActionResult> PutPlaylist(int id, Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return BadRequest();
            }

            _context.Entry(playlist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            memoryCache.Remove(cacheKey);

            var res = await _context.Playlists.FindAsync(id);
            return res;

            //return NotFound();
        }

        // POST: api/Playlists
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Playlist>> PostPlaylist(Playlist playlist)
        {
            memoryCache.Remove(cacheKey);

            var created = _context.Playlists.Add(playlist).Entity;
            var res = await _context.SaveChangesAsync();

            _logger.LogInformation($"Posted Playlist {created.Id}");
            //if (n > 0)
            //{
            //    memoryCache.Set(cacheKey, playlists, cacheExpiryOptions);
            //}
            return CreatedAtAction("PostPlaylist", new { id = created.Id }, created);
        }

        // DELETE: api/Playlists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Playlist>> DeletePlaylist(int id)
        {
            memoryCache.Remove(cacheKey);

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return playlist;
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(e => e.Id == id);
        }
    }
}
