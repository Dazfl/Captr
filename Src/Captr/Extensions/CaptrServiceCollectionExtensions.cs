using Captr.EventStorage;
using Captr.SnapshotStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.Extensions
{
	public static class CaptrServiceCollectionExtensions
	{
		public static IServiceCollection AddCaptr(this IServiceCollection services, Action<CaptrOptionsBuilder> options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));

			var optionsBuilder = new CaptrOptionsBuilder();
			options(optionsBuilder);

			var servicesToAdd = optionsBuilder.GetServices();
			services.Add(servicesToAdd);
			var optionsToAdd = optionsBuilder.GetOptions();
			services.AddSingleton(optionsToAdd);

			// Add main client
			services.AddSingleton<CaptrClient>();

			return services;
		}

		public static CaptrOptionsBuilder AddEventStorage(this CaptrOptionsBuilder optionsBuilder, Action<CaptrOptionsBuilder> storageOptions)
		{
			storageOptions(optionsBuilder);

			// Add EventStore
			optionsBuilder.AddService(new ServiceDescriptor(typeof(IEventStore), typeof(EventStore), ServiceLifetime.Singleton));

			return optionsBuilder;
		}

		public static CaptrOptionsBuilder AddSnapshotStorage(this CaptrOptionsBuilder optionsBuilder, Action<CaptrOptionsBuilder> storageOptions)
		{
			storageOptions(optionsBuilder);

			// Add SnapshotStore
			optionsBuilder.AddService(new ServiceDescriptor(typeof(ISnapshotStore), typeof(SnapshotStore), ServiceLifetime.Singleton));

			return optionsBuilder;
		}
	}
}
