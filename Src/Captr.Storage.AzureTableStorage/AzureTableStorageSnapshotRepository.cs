using Captr.SnapshotStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.Storage.AzureTableStorage
{
	public class AzureTableStorageSnapshotRepository : ISnapshotRepository
	{
		private readonly string _tableName;
		private readonly CloudStorageAccount _storageAccount;
		private readonly ILogger<AzureTableStorageSnapshotRepository> _logger;

		public AzureTableStorageSnapshotRepository(CloudStorageAccount storageAccount, CaptrOptions options, ILogger<AzureTableStorageSnapshotRepository> logger)
		{
			_storageAccount = storageAccount;
			_tableName = options.GetConfigItem("SnapshotTableName");
			_logger = logger;
		}

		public async Task<SnapshotDescriptor> GetSnapshotAsync(string streamId, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(streamId))
				return null;

			var tableClient = _storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(_tableName);
			await table.CreateIfNotExistsAsync(cancellationToken);

			TableOperation operation = TableOperation.Retrieve<DynamicTableEntity>(streamId, streamId);

			try
			{
				var operationResult = await table.ExecuteAsync(operation, cancellationToken);

				if (operationResult.HttpStatusCode != StatusCodes.Status200OK)
					return null;

				var snapshotEntity = operationResult.Result as DynamicTableEntity;
				var snapshotDescriptor = TableEntity.ConvertBack<SnapshotDescriptor>(snapshotEntity.Properties, null);
				return snapshotDescriptor;
			}
			catch (StorageException ex)
			{
				_logger.LogWarning(ex, "Something went wrong with ExecuteAsync while attempting to retrieve a snapshot.");
				return null;
			}
		}

		public async Task<bool> SaveSnapshotAsync(SnapshotDescriptor snapshotDescriptor, CancellationToken cancellationToken = default)
		{
			if (snapshotDescriptor == null)
				return false;

			var tableClient = _storageAccount.CreateCloudTableClient();
			var table = tableClient.GetTableReference(_tableName);
			await table.CreateIfNotExistsAsync(cancellationToken);

			OperationContext context = new();
			var properties = TableEntity.Flatten(snapshotDescriptor, context);

			var snapshotEntity = new DynamicTableEntity(snapshotDescriptor.StreamId, snapshotDescriptor.StreamId)
			{
				Properties = properties
			};

			TableOperation operation = TableOperation.InsertOrMerge(snapshotEntity);

			try
			{
				await table.ExecuteAsync(operation, cancellationToken);
				return true;
			}
			catch (StorageException ex)
			{
				_logger.LogWarning(ex, "Something went wrong with ExecuteAsync while attempting to save a snapshot.");
				return false;
			}

		}
	}
}
