using HVMDash.Server.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vkaudioposter_ef.Model;

namespace HVMDash.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ConfigurationContext _context;
        public ConfigurationController(ConfigurationContext context)
        {
            _context = context;
        }

        // GET: Configuration
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Configuration>>> GetConfigurations()
        {
            return await _context.Configurations.ToListAsync();
        }

        // GET: Configuration/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Configuration>> GetConfiguration(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Configuration = await _context.Configurations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Configuration == null)
            {
                return NotFound();
            }

            return Configuration;
        }

        // POST: Configuration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<Configuration>> PostConfig(Configuration Configuration)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(Configuration);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            //}
            return CreatedAtAction("GetConfig", new { id = Configuration.Id }, Configuration);
        }

        // GET: Configuration/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Configuration>> PutXpath(int id, Configuration Configuration)
        {
            if (id != Configuration.Id)
            {
                return BadRequest();
            }

            _context.Entry(Configuration).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfigurationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var res = await _context.Configurations.FindAsync(id);
            return res;

        }


        // GET: Configuration/Delete/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Configuration>> DeleteConfig(int? id)
        {
            var xpath = await _context.Configurations.FindAsync(id);
            if (xpath == null)
            {
                return NotFound();
            }

            _context.Configurations.Remove(xpath);
            await _context.SaveChangesAsync();

            return xpath;
        }

        private bool ConfigurationExists(int id)
        {
            return _context.Configurations.Any(e => e.Id == id);
        }
    }
}
