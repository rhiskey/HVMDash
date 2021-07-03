using HVMDash.Server.Context;
using HVMDash.Server.Controllers;
using HVMDash.Shared;
using Microsoft.EntityFrameworkCore;
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
        private Timer _timer;
        //_timer2;
        //private readonly PlaylistContext _context;
        public TimedHostedService(ILogger<TimedHostedService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

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

            //Get Playlists

            //List<Playlist> Playlists  = await _context.Playlists.OrderBy(p => p.Status).ThenBy(p => p.PlaylistName).ToListAsync();


            //for (int i = 0; i < Playlists.Count(); i++)
            //{
            //    var playlist = Playlists.ToList()[i];
            //    var uri = playlist.PlaylistId;
            //    var id = Formatters.GetIdFromSpotifyUri(uri);

            //    SpotifyController sc = new();
            //    var response = await sc.GetSpotify(id);

            //    string jsonString = JsonSerializer.Serialize(response);
            //    var playlistInfo = JsonSerializer.Deserialize<SpotifyAPIPlaylist>(jsonString);

            //    playlist.ImageUrl = playlistInfo.Images.First().Url;
            //    playlist.FollowersTotal = playlistInfo.Followers.Total;
            //    playlist.UpdateDate = DateTime.Now;

            //    //PlaylistsController pc = new();
            //    //pc.PutPlaylist(id, playlist);
            //}

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
