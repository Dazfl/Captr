using Captr.EventStorage;
using Captr.Extensions;
using Captr.SnapshotStorage;
using Captr.Storage.AzureTableStorage;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Captr.Extensions
{
	public static class CaptrOptionsBuilderExtensions
	{
		public static CaptrOptionsBuilder UseAzureTableStorageAsEventStore(this CaptrOptionsBuilder optionsBuilder, string connectionString, string tableName)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(nameof(tableName));

			if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
				throw new ArgumentException("Storage account connection string error.");

			optionsBuilder.SetOption("EventTableName", tableName);
			optionsBuilder.AddService(new ServiceDescriptor(typeof(CloudStorageAccount), storageAccount));
			optionsBuilder.AddService(new ServiceDescriptor(typeof(IEventRepository), typeof(AzureTableStorageEventRepository), ServiceLifetime.Singleton));

			return optionsBuilder;
		}

		public static CaptrOptionsBuilder UseAzureTableStorageAsSnapshotStore(this CaptrOptionsBuilder optionsBuilder, string connectionString, string tableName)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(nameof(tableName));

			if (!CloudStorageAccount.TryParse(connectionString, out CloudStorageAccount storageAccount))
				throw new ArgumentException("Storage account connection string error.");

			optionsBuilder.SetOption("SnapshotTableName", tableName);
			optionsBuilder.AddService(new ServiceDescriptor(typeof(CloudStorageAccount), storageAccount));
			optionsBuilder.AddService(new ServiceDescriptor(typeof(ISnapshotRepository), typeof(AzureTableStorageSnapshotRepository), ServiceLifetime.Singleton));

			return optionsBuilder;
		}
	}
}
