using AutoMapper.QueryableExtensions;
using Chama.Assignment.Service.Data;
using Chama.Assignment.Service.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.Assignment.Service.SignalR
{
    public class StopwatchHub : Hub
    {
        private readonly TimerEntryService _timerEntryService;

        /// <summary>
        /// Stopwatchtime is injected here to ensure the update timer is running
        /// </summary>
        public StopwatchHub(
            StopwatchTimer _stopwatchTimer,
            TimerEntryService timerEntryService)
        {
            _timerEntryService = timerEntryService;
        }

        public async Task Start(string stopwatchName)
        {
            await _timerEntryService.InsertOrUpdate(
                Context.User.Identity.Name,
                stopwatchName);
        }

        public async Task<IEnumerable<Stopwatch>> GetStopwatches()
        {
            var timerEntries = await _timerEntryService.GetByOwnerName(Context.User.Identity.Name);

            return timerEntries
                .AsQueryable()
                .ProjectTo<Stopwatch>()
                .ToList();
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            Groups.Add(Context.ConnectionId, name.ToLowerInvariant());

            return base.OnConnected();
        }
    }
}
