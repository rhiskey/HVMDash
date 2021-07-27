using HVMDash.Server.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HVMDash.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController : ControllerBase
    {
        private readonly TelegramUsersContext _context;
        public TelegramController(TelegramUsersContext context)
        {
            _context = context;
        }

        // GET: api/Telegram/uuid=aaa-fff-gg
        [HttpGet()]
        public async Task<ActionResult<string>> GetChatId(string uuid)
        {
            long chatId = 0;
            if (string.IsNullOrEmpty(uuid))
            {
                return NotFound();
            }

            //Wait longpoll read messages, where msg.text == uuid => getChatId

            //vkaudioposter_ef.Model.TelegramUser user = await _context.TelegramUsers.Where(e => e.UUID == uuid).FirstOrDefaultAsync();


            //object toReturn = new() { UUID = uuid, ChatId = chatId };

            var jsonString = JsonSerializer.Serialize(chatId);
            return CreatedAtAction("GetChatId", new { UUID = uuid, ChatId = chatId }, jsonString);
        }
    }
}
