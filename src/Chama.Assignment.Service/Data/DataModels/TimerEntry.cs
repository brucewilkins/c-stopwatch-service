using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Chama.Assignment.Service.Data.DataModels
{
    public class TimerEntry : TableEntity
    {
        public TimerEntry()
        { }

        public TimerEntry(string ownerName, string stopwatchName)
        {
            if (string.IsNullOrEmpty(ownerName)) throw new ArgumentNullException(nameof(ownerName));
            if (string.IsNullOrEmpty(stopwatchName)) throw new ArgumentNullException(nameof(stopwatchName));

            PartitionKey = ownerName.ToLowerInvariant();
            RowKey = Guid.NewGuid().ToString();
            OwnerName = ownerName;
            StopwatchName = stopwatchName;
            StopwatchNameInvariant = stopwatchName.ToLowerInvariant();
        }

        public string OwnerName { get; set; }
        public string StopwatchName { get; set; }
        public string StopwatchNameInvariant { get; set; }
        public DateTime StartedAt { get; set; }
    }
}
