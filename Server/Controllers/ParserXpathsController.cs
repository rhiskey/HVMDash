using HVMDash.Server.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using vkaudioposter_ef.Model;


namespace HVMDash.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParserXpathsController : ControllerBase
    {
        private readonly ParserXpathContext _context;

        public ParserXpathsController(ParserXpathContext context)
        {
            _context = context;
        }

        // GET: ParserXpaths
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParserXpath>>> GetXpaths()
        {
            return await _context.ParserXpaths.ToListAsync();
        }

        // GET: ParserXpaths/Xpath/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParserXpath>> GetXpath(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var parserXpath = await _context.ParserXpaths
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parserXpath == null)
            {
                return NotFound();
            }

            return parserXpath;
        }

        // GET: api/ConsolePhotostocks/export
        [HttpGet("export")]
        public async Task<JsonResult> ExportPhotostocks()
        {
            List<ParserXpath> data = new();

            data = await _context.ParserXpaths.OrderBy(p => p.Id).ToListAsync();
            var jsonString = JsonSerializer.Serialize(data);

            return new JsonResult(jsonString);
        }

        //// GET: ParserXpaths/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: ParserXpaths/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult<ParserXpath>> PostXpath(ParserXpath parserXpath)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(parserXpath);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            //}
            return CreatedAtAction("GetXpath", new { id = parserXpath.Id }, parserXpath);
        }

        // GET: ParserXpaths/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ParserXpath>> PutXpath(int id, ParserXpath parserXpath)
        {
            if (id != parserXpath.Id)
            {
                return BadRequest();
            }

            _context.Entry(parserXpath).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParserXpathExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var res = await _context.ParserXpaths.FindAsync(id);
            return res;

        }

        //// POST: ParserXpaths/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Xpath,XpathInner")] ParserXpath parserXpath)
        //{
        //    if (id != parserXpath.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(parserXpath);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ParserXpathExists(parserXpath.Id))
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
        //    return View(parserXpath);
        //}

        // GET: ParserXpaths/Delete/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ParserXpath>> DeleteXpath(int? id)
        {
            var xpath = await _context.ParserXpaths.FindAsync(id);
            if (xpath == null)
            {
                return NotFound();
            }

            _context.ParserXpaths.Remove(xpath);
            await _context.SaveChangesAsync();

            return xpath;
        }

        //// POST: ParserXpaths/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var parserXpath = await _context.ParserXpaths.FindAsync(id);
        //    _context.ParserXpaths.Remove(parserXpath);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ParserXpathExists(int id)
        {
            return _context.ParserXpaths.Any(e => e.Id == id);
        }
    }
}
