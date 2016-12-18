using Chama.Assignment.Service.Data.DataModels;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chama.Assignment.Service.Data
{
    public class TimerEntryService
    {
        private readonly string _storageConnectionString;
        private const string TableName = "Stopwatch";

        CloudTable _table;

        public TimerEntryService(IOptions<Configuration.Options> options)
        {
            _storageConnectionString = options.Value.StorageConnectionString;

            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            _table = storageAccount.CreateCloudTableClient().GetTableReference(TableName);
            _table.CreateIfNotExistsAsync().Wait();
        }

        public async Task InsertOrUpdate(string ownerName, string stopwatchName)
        {
            var timerEntry = await GetExisting(ownerName, stopwatchName) 
                ?? new TimerEntry(ownerName, stopwatchName);

            timerEntry.StartedAt = DateTime.UtcNow;
            timerEntry.StopwatchName = stopwatchName;

            var tableOperation = TableOperation.InsertOrReplace(timerEntry);
            var tableTask = _table.ExecuteAsync(tableOperation).Result;
        }

        public async Task<IEnumerable<TimerEntry>> GetByOwnerName(string ownerName)
        {
            if (string.IsNullOrEmpty(ownerName)) throw new ArgumentNullException(nameof(ownerName));

            var tableQuery = new TableQuery<TimerEntry>()
                .Where(TableQuery.GenerateFilterCondition(nameof(TimerEntry.PartitionKey), QueryComparisons.Equal, ownerName.ToLowerInvariant()));

            return await ExecuteQueryAsync(_table, tableQuery);
        }

        public async Task<IEnumerable<IGrouping<string, TimerEntry>>> GetAllGroupedByOwner()
        {
            var tableQuery = new TableQuery<TimerEntry>();
            var timerEntries = await ExecuteQueryAsync(_table, tableQuery);

            return timerEntries.GroupBy(t => t.PartitionKey);
        }

        private async Task<TimerEntry> GetExisting(string ownerName, string stopwatchName)
        {
            var tableQuery = new TableQuery<TimerEntry>()
               .Where(TableQuery.GenerateFilterCondition(nameof(TimerEntry.PartitionKey), QueryComparisons.Equal, ownerName.ToLowerInvariant()))
               .Where(TableQuery.GenerateFilterCondition(nameof(TimerEntry.StopwatchNameInvariant), QueryComparisons.Equal, stopwatchName.ToLowerInvariant()));

            var items = await ExecuteQueryAsync(_table, tableQuery);
            return items.FirstOrDefault();
        }

        private static async Task<IList<T>> ExecuteQueryAsync<T>(CloudTable table, TableQuery<T> query) 
            where T : ITableEntity, new()
        {
            var items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync<T>(query, token);
                token = segment.ContinuationToken;
                items.AddRange(segment);

            } while (token != null);

            return items;
        }
    }
}
