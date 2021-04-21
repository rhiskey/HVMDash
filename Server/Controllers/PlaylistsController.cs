using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAAuth.Server.Context;
using vkaudioposter_ef.parser;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace WAAuth.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly PlaylistContext _context;

        public PlaylistsController(PlaylistContext context)
        {
            _context = context;
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
            return await _context.Playlists.OrderBy(p => p.Status).ThenBy(p=>p.PlaylistName).ToListAsync();
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
            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlaylist", new { id = playlist.Id }, playlist);
        }

        // DELETE: api/Playlists/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Playlist>> DeletePlaylist(int id)
        {
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
