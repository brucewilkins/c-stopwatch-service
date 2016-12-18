using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using Chama.Assignment.Service.Models;
using Microsoft.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Chama.Assignment.Service.Data;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;

namespace Chama.Assignment.Service.SignalR
{
    public class StopwatchTimer
    {
        private readonly IConnectionManager _connectionManager;
        private readonly TimerEntryService _timerEntryService;
        
        private readonly object _updateLock = new object();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(15);
        private readonly Timer _timer;

        private volatile bool _updatingStopwatches = false;

        public StopwatchTimer(
            IConnectionManager connectionManager,
            TimerEntryService timerEntryService)
        {
            _connectionManager = connectionManager;
            _timerEntryService = timerEntryService;

            _timer = new Timer(TimerElapsed, null, _updateInterval, _updateInterval);
        }

        private void TimerElapsed(object state)
        {
            lock (_updateLock)
            {
                if (!_updatingStopwatches)
                {
                    try
                    {
                        _updatingStopwatches = true;
                        BroadcastElapsedTimes().Wait();
                    }
                    finally
                    {
                        _updatingStopwatches = false;
                    }
                }
            }
        }

        private async Task BroadcastElapsedTimes()
        {
            var groupedTimerEntries = await _timerEntryService.GetAllGroupedByOwner();
            var clients = _connectionManager.GetHubContext<StopwatchHub>().Clients;

            foreach (var timerEntries in groupedTimerEntries)
            {
                var stopwatches = timerEntries.AsQueryable()
                    .ProjectTo<Stopwatch>()
                    .ToList();

                clients.Group(timerEntries.Key).updateElapsedTime(stopwatches);
            }
        }
    }
}
