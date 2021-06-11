using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HVMDash.Shared;
using Microsoft.AspNetCore.Mvc;

namespace HVMDash.Server.Controllers
{
    //[Route("notifications")]
    //[ApiController]
    //public class NotificationsController : Controller
    //{
    //    private readonly PizzaStoreContext _db;

    //    public NotificationsController(PizzaStoreContext db)
    //    {
    //        _db = db;
    //    }

    //    [HttpPut("subscribe")]
    //    public async Task<NotificationSubscription> Subscribe(NotificationSubscription subscription)
    //    {
    //        // We're storing at most one subscription per user, so delete old ones.
    //        // Alternatively, you could let the user register multiple subscriptions from different browsers/devices.
    //        var userId = GetUserId();
    //        var oldSubscriptions = _db.NotificationSubscriptions.Where(e => e.UserId == userId);
    //        _db.NotificationSubscriptions.RemoveRange(oldSubscriptions);

    //        // Store new subscription
    //        subscription.UserId = userId;
    //        _db.NotificationSubscriptions.Attach(subscription);

    //        await _db.SaveChangesAsync();
    //        return subscription;
    //    }

    //    private string GetUserId()
    //    {
    //        return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    //    }
    //}
}
