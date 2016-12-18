using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Chama.Assignment.Service.Data;
using Chama.Assignment.Service.Models;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Chama.Assignment.Service.SignalR;

namespace Chama.Assignment.Service.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class StopwatchController : Controller
    {
        private readonly TimerEntryService _timerEntryService;

        public StopwatchController(TimerEntryService timerEntryService)
        {
            _timerEntryService = timerEntryService;
        }

        [HttpPost()]
        public async Task<IActionResult> Post([FromBody]StartStopwatchRequest request)
        {
            await _timerEntryService.InsertOrUpdate(
                User.Identity.Name, 
                request.StopwatchName);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string ownerName)
        {
            if (string.IsNullOrEmpty(ownerName)) return BadRequest();

            var timerEntries = await _timerEntryService.GetByOwnerName(ownerName);

            var stopwatches = timerEntries
                .AsQueryable()
                .ProjectTo<Stopwatch>()
                .ToList();

            return Ok(stopwatches);
        }      
    }
}
