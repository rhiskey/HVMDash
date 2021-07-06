using HVMDash.Server.Context;
using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
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
        public async Task<ActionResult<string>> GetConfigurations()
        {
            var cfg = await _context.Configurations.FirstOrDefaultAsync();
            //var cfgToReturn = new { HoursPeriod = cfg.HoursPeriod, MinutesPeriod = cfg.MinutesPeriod };
            //return AcceptedAtAction("GetConfig", new { hours = cfg.HoursPeriod, minutes = cfg.MinutesPeriod });
            var cfgToReturned = new SimpleSettings { Id = cfg.Id, HoursPeriod = cfg.HoursPeriod, MinutesPeriod = cfg.MinutesPeriod, UseApiWs = cfg.USEApiWS };

            var jsonString = JsonSerializer.Serialize(cfgToReturned);
            return CreatedAtAction("GetConfigurations", new { Id = cfg.Id, HoursPeriod = cfg.HoursPeriod, MinutesPeriod = cfg.MinutesPeriod, UseApiWs = cfg.USEApiWS }, jsonString);
        }


        //// GET: Configuration
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Configuration>>> GetConfigurations()
        //{
        //    return await _context.Configurations.ToListAsync();
        //}

        //// GET: Configuration/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Configuration>> GetConfiguration(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var Configuration = await _context.Configurations
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (Configuration == null)
        //    {
        //        return NotFound();
        //    }

        //    return Configuration;
        //}

        //// POST: Configuration/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public async Task<ActionResult<Configuration>> PostConfig(Configuration Configuration)
        //{
        //    //if (ModelState.IsValid)
        //    //{
        //    _context.Add(Configuration);
        //    await _context.SaveChangesAsync();
        //    //return RedirectToAction(nameof(Index));
        //    //}
        //    return CreatedAtAction("GetConfig", new { id = Configuration.Id }, Configuration);
        //}

        //TODO
        // GET: Configuration/5
        [HttpPut("{id}")]
        public async Task<ActionResult<SimpleSettings>> PutConfig(int id, SimpleSettings ss)
        {
            Configuration config;
            SimpleSettings simpleSettings = new();
            if (id != ss.Id)
            {
                return BadRequest();
            }

            config = await _context.Configurations.FindAsync(id);

            _context.Entry(config).State = EntityState.Modified;
            try
            {
                config.HoursPeriod = ss.HoursPeriod;
                config.MinutesPeriod = ss.MinutesPeriod;

                config.Id = simpleSettings.Id;
                config.HoursPeriod = simpleSettings.HoursPeriod;
                config.MinutesPeriod = simpleSettings.MinutesPeriod ;
                config.USEApiWS = simpleSettings.UseApiWs;

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

            return simpleSettings;
        }


        //// GET: Configuration/Delete/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Configuration>> DeleteConfig(int? id)
        //{
        //    var xpath = await _context.Configurations.FindAsync(id);
        //    if (xpath == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Configurations.Remove(xpath);
        //    await _context.SaveChangesAsync();

        //    return xpath;
        //}

        private bool ConfigurationExists(int id)
        {
            return _context.Configurations.Any(e => e.Id == id);
        }
    }
}
