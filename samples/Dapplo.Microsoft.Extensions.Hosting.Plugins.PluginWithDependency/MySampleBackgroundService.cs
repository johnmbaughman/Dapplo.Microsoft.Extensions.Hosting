﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Flurl.Http;

namespace Dapplo.Microsoft.Extensions.Hosting.Plugins.PluginWithDependency
{
    /// <summary>
    /// Just some service to run in the background and use a dependency
    /// </summary>
    internal class MySampleBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly string _uri = "https://nu.nl";

        public MySampleBackgroundService(ILogger<MySampleBackgroundService> logger)
        {
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Retrieving something.");

            Task.Run(async () =>
            {
                try
                {
                    var result = await _uri.GetStringAsync();
                    _logger.LogInformation("{0} : {1}", _uri, result.Substring(0, 40));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Couldn't connect to {0}, this was expected behind a corporate firewall, as HttpClient doesn't have a default proxy!", _uri);
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}