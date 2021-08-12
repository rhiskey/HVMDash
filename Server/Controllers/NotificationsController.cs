using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using HVMDash.Server.Firebase;
using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HVMDash.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        // POST: api/notifications/send?topic=weather&title=notification&body=welcome_to_app
        [HttpPost("send")]

        public async Task<ActionResult<string>> SendNotification(MobileNotifications notification)
        {
            string jsonString;

            MobileMessagingClient mobileMessagingClient = new MobileMessagingClient();

            // See documentation on defining a message payload.
            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = notification.Title,
                    Body = notification.Body,
                },
                Topic = notification.Topic,
            };

            // Send a message to devices subscribed to the combination of topics
            // specified by the provided condition.
            string response = await mobileMessagingClient.messaging.SendAsync(message);
            // Response is a message ID string.

            jsonString = JsonSerializer.Serialize(response);

            return CreatedAtAction("SendNotification", new { response }, jsonString);
        }
    }
}
