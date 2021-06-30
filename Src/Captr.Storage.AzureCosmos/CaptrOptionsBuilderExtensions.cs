using Captr.EventStorage;
using Captr.Extensions;
using Captr.SnapshotStorage;
using Captr.Storage.AzureCosmos;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Captr.Extensions
{
	public static class CaptrOptionsBuilderExtensions
	{
		public static CaptrOptionsBuilder UseAzureCosmosAsEventStore(this CaptrOptionsBuilder optionsBuilder, string accountEndpoint, string accountKey, string database, string container)
		{
			if (string.IsNullOrEmpty(accountEndpoint)) throw new ArgumentNullException(nameof(accountEndpoint));
			if (string.IsNullOrEmpty(accountKey)) throw new ArgumentNullException(nameof(accountKey));
			if (string.IsNullOrEmpty(database)) throw new ArgumentNullException(nameof(database));
			if (string.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

			CosmosClient client = new(accountEndpoint, accountKey);
			var eventContainer = client.GetContainer(database, container);

			optionsBuilder.AddService(new ServiceDescriptor(typeof(Container), eventContainer));
			optionsBuilder.AddService(new ServiceDescriptor(typeof(IEventRepository), typeof(AzureCosmosEventRepository), ServiceLifetime.Singleton));

			return optionsBuilder;
		}

		public static CaptrOptionsBuilder UseAzureCosmosAsSnapshotStore(this CaptrOptionsBuilder optionsBuilder, string accountEndpoint, string accountKey, string database, string container)
		{
			if (string.IsNullOrEmpty(accountEndpoint)) throw new ArgumentNullException(nameof(accountEndpoint));
			if (string.IsNullOrEmpty(accountKey)) throw new ArgumentNullException(nameof(accountKey));
			if (string.IsNullOrEmpty(database)) throw new ArgumentNullException(nameof(database));
			if (string.IsNullOrEmpty(container)) throw new ArgumentNullException(nameof(container));

			CosmosClient client = new(accountEndpoint, accountKey);
			Container snapshotContainer = client.GetContainer(database, container);

			optionsBuilder.AddService(new ServiceDescriptor(typeof(Container), snapshotContainer));
			optionsBuilder.AddService(new ServiceDescriptor(typeof(ISnapshotRepository), typeof(AzureCosmosSnapshotRepository), ServiceLifetime.Singleton));


			return optionsBuilder;
		}
	}
}
