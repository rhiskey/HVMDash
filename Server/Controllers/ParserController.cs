using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using vkaudioposter_Console.API;


namespace HVMDash.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParserController : ControllerBase
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        // PUT: api/Parser/Action
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Action")]
        public async Task<IActionResult> PostParser(string cmd)
        {
            //if (action != "start")
            //{
            //    return BadRequest();
            //}
            //Console.WriteLine($"Start Parser");
            StartProgram.Start();

            //return CreatedAtAction(
            //"StartParser",
            //new { result = "Command Send" });
            return Ok();
        }
    }
}
