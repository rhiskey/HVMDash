using HVMDash.Server.Context;
using HVMDash.Server.Controllers;
using HVMDash.Server.VK;
using HVMDash.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using vkaudioposter_Console.API;
using vkaudioposter_ef.parser;

namespace HVMDash.Server.Service
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer, _timer2;
        //private readonly ConfigurationContext configurationContext;
        private readonly IServiceScopeFactory scopeFactory;

        //_timer2;
        //private readonly PlaylistContext _context;
        public TimedHostedService(ILogger<TimedHostedService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
        }

        //public TimedHostedService(IServiceScopeFactory scopeFactory)
        //{
        //    this.scopeFactory = scopeFactory;
        //}

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            PlaylistContext plContext;
            using (var scope = scopeFactory.CreateScope())
            {
                var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationContext>().Configurations.FirstOrDefault();
                Thread task = new Thread(async () => await LongPollHandler.LongPollListenerAsync(configContext));
                task.Start();

                plContext = scope.ServiceProvider.GetRequiredService<PlaylistContext>();
            }

#if DEBUG
            _logger.LogInformation("Mode=Debug");
#else

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(3));

            _timer2 = new Timer(DoUpdate, null, TimeSpan.Zero, TimeSpan.FromDays(7));
#endif
            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            StartProgram.Start();

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
        }

        private async void DoUpdate(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            var playlistContext = (PlaylistContext)state;

            try
            {
                UpdatePlaylists.UpdatePlaylistsTask(playlistContext);
            }catch (Exception ex)
            {
                _logger.LogInformation($"Error: ${ex.Message}");
            }

            _logger.LogInformation(
            "Timed Hosted Service Update Playlists is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
