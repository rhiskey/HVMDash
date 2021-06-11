using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HVMDash.Server.Service
{
    public class HVMBotService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<HVMBotService> _logger;

        public HVMBotService(ILogger<HVMBotService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            DoWork(null);

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            var count = Interlocked.Increment(ref executionCount);

            //Start bot

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", count);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");


            return Task.CompletedTask;
        }

        public void Dispose()
        {
            //_timer?.Dispose();
        }
    }
}
