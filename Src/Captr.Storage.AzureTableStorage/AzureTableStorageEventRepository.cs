using Captr.EventStorage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.Storage.AzureTableStorage
{
	public class AzureTableStorageEventRepository : IEventRepository
	{
		private readonly string _tableName;
		private readonly CloudStorageAccount _storageAccount;
		private readonly ILogger<AzureTableStorageEventRepository> _logger;

		public AzureTableStorageEventRepository(CloudStorageAccount storageAccount, CaptrOptions options, ILogger<AzureTableStorageEventRepository> logger)
		{
			_storageAccount = storageAccount;
			_tableName = options.GetConfigItem("EventTableName");
			_logger = logger;
		}

		/// <summary>
		/// Append a list of events to the event stream
		/// </summary>
		/// <param name="streamId"></param>
		/// <param name="expectedVersion"></param>
		/// <param name="events"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<int> AppendEventsToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventDescriptor> events, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(streamId) || expectedVersion < 0)
				return -1;

			var tableClient = _storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(_tableName);
			await table.CreateIfNotExistsAsync(cancellationToken);

			TableBatchOperation batch = new();

			IDictionary<string, EntityProperty> properties;
			int version = -1;

			foreach (var eventDescriptor in events)
			{
				properties = TableEntity.Flatten(eventDescriptor, null);
				batch.Insert(new DynamicTableEntity(streamId, $"{streamId}||{eventDescriptor.Stream.Version}")
				{
					Properties = properties
				});
				version = eventDescriptor.Stream.Version;
			}

			try
			{
				await table.ExecuteBatchAsync(batch, cancellationToken);
				return version;
			}
			catch (StorageException ex)
			{
				// Log the error
				_logger.LogWarning(ex, "Something went wrong with ExecuteBatchAsync.");
				return -1;
			}
		}

		/// <summary>
		/// Retrieve a list of events from the specified stream
		/// </summary>
		/// <param name="streamId"></param>
		/// <param name="fromVersion"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<IReadOnlyCollection<EventDescriptor>> GetEventsFromStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(streamId))
				return new List<EventDescriptor>(0);

			var tableClient = _storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(_tableName);
			await table.CreateIfNotExistsAsync(cancellationToken);

			var filter = TableQuery.CombineFilters(
				TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, streamId),
				TableOperators.And,
				TableQuery.GenerateFilterConditionForInt("Stream_Version", QueryComparisons.GreaterThan, fromVersion)
			);

			var query = new TableQuery<DynamicTableEntity>()
				.Where(filter);

			List<DynamicTableEntity> entities = new(0);
			TableContinuationToken token = null;

			do
			{
				var queryResults = await table.ExecuteQuerySegmentedAsync(query, token);
				entities.AddRange(queryResults.Results);
				token = queryResults.ContinuationToken;
			} while (token != null && !cancellationToken.IsCancellationRequested);


			return entities
				.Select(e => TableEntity.ConvertBack<EventDescriptor>(e.Properties, null))
				.ToList();
		}
	}
}
