using Newtonsoft.Json;
using System;

namespace Chama.Assignment.Service.Models
{
    public class Stopwatch
    {
        public string StopwatchName { get; set; }

        [JsonIgnore]
        public DateTime StartedAt { get; set; }

        public long Elapsed => Convert.ToInt64(DateTime.UtcNow.Subtract(StartedAt).TotalMilliseconds);
    }
}
