
using HVMDash.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HVMDash.Client
{
    public class DashClient
    {
        private readonly HttpClient httpClient;
        public DashClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task SubscribeToNotifications(NotificationSubscription subscription)
        {
            var response = await httpClient.PutAsJsonAsync("notifications/subscribe", subscription);
            response.EnsureSuccessStatusCode();
        }
    }
}
