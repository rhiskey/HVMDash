using HVMDash.Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
    public class ConsolePhotostocksController : ControllerBase
    {
        private readonly ConsolePhotostockContext _context;
        private readonly IMemoryCache memoryCache;
        private readonly string cacheKey = "AllPhotostocks";

        private readonly MemoryCacheEntryOptions cacheExpiryOptions = new()
        {
            AbsoluteExpiration = DateTime.Now.AddDays(7),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromDays(3)
        };

        public ConsolePhotostocksController(IMemoryCache memoryCache, ConsolePhotostockContext context)
        {
            this.memoryCache = memoryCache;
            _context = context;
        }

        // GET: api/ConsolePhotostocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsolePhotostock>>> GetPhotostocks()
        {
            List<ConsolePhotostock> stock1;
            if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<ConsolePhotostock> _))
            {
                stock1 = await _context.Photostocks.ToListAsync();
                memoryCache.Set(cacheKey, stock1, cacheExpiryOptions);
                return stock1;
            }
            memoryCache.TryGetValue(cacheKey, out stock1);
            return stock1;
        }

        // GET: api/ConsolePhotostocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConsolePhotostock>> GetConsolePhotostock(int id)
        {
            var consolePhotostock = await _context.Photostocks.FindAsync(id);

            if (consolePhotostock == null)
            {
                return NotFound();
            }

            return consolePhotostock;
        }

        // GET: api/ConsolePhotostocks/export
        [HttpGet("export")]
        public async Task<JsonResult> ExportPhotostocks()
        {
            List<ConsolePhotostock> data = new();

            data = await _context.Photostocks.OrderBy(p => p.Status).ToListAsync();
            var jsonString = JsonSerializer.Serialize(data);

            return new JsonResult(jsonString);
        }

        // PUT: api/ConsolePhotostocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConsolePhotostock(int id, ConsolePhotostock consolePhotostock)
        {
            if (id != consolePhotostock.Id)
            {
                return BadRequest();
            }

            _context.Entry(consolePhotostock).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConsolePhotostockExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            memoryCache.Remove(cacheKey);
            return NoContent();
        }

        // POST: api/ConsolePhotostocks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ConsolePhotostock>> PostConsolePhotostock(ConsolePhotostock consolePhotostock)
        {
            memoryCache.Remove(cacheKey);
            _context.Photostocks.Add(consolePhotostock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostConsolePhotostock", new { id = consolePhotostock.Id }, consolePhotostock);
        }

        // DELETE: api/ConsolePhotostocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsolePhotostock(int id)
        {
            memoryCache.Remove(cacheKey);
            var consolePhotostock = await _context.Photostocks.FindAsync(id);
            if (consolePhotostock == null)
            {
                return NotFound();
            }

            _context.Photostocks.Remove(consolePhotostock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConsolePhotostockExists(int id)
        {
            return _context.Photostocks.Any(e => e.Id == id);
        }
    }
}
