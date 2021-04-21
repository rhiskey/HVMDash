using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WAAuth.Server.Context;
using vkaudioposter_ef.parser;
using Microsoft.AspNetCore.Authorization;

namespace WAAuth.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ConsolePhotostocksController : ControllerBase
    {
        private readonly ConsolePhotostockContext _context;

        public ConsolePhotostocksController(ConsolePhotostockContext context)
        {
            _context = context;
        }

        // GET: api/ConsolePhotostocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConsolePhotostock>>> GetPhotostocks()
        {
            return await _context.Photostocks.ToListAsync();
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

            return NoContent();
        }

        // POST: api/ConsolePhotostocks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ConsolePhotostock>> PostConsolePhotostock(ConsolePhotostock consolePhotostock)
        {
            _context.Photostocks.Add(consolePhotostock);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConsolePhotostock", new { id = consolePhotostock.Id }, consolePhotostock);
        }

        // DELETE: api/ConsolePhotostocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConsolePhotostock(int id)
        {
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
