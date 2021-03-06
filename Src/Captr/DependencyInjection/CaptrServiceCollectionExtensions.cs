using Captr.Aggregates;
using Captr.EventStorage;
using Captr.SnapshotStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Captr.DependencyInjection
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

		/// <summary>
		/// Register delegates for the specified Aggregate.
		/// </summary>
		/// <typeparam name="TEntity">Type of <see cref="AggregateRoot{TEntity}"/></typeparam>
		/// <param name="services">IServiceCollection</param>
		/// <returns>Returns IServiceCollection</returns>
		public static IServiceCollection AddCaptrAggregate<TEntity>(this IServiceCollection services) where TEntity : AggregateRoot<TEntity>, new()
		{
			services.AddDelegate<CaptrClient, CaptrClientServices<TEntity>.LoadEntity>(method => method.LoadEntity<TEntity>);
			return services;
		}

		private static IServiceCollection AddDelegate<TService, TDelegate>(this IServiceCollection services, Func<TService, TDelegate> DelegateFromService)
			where TDelegate : Delegate
		{
			return services.AddScoped(sp => DelegateFromService(sp.GetRequiredService<TService>()));
		}
	}
}
