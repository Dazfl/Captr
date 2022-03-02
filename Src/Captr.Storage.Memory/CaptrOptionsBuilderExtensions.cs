using Captr.EventStorage;
using Captr.SnapshotStorage;
using Microsoft.Extensions.DependencyInjection;

namespace Captr.Storage.Memory
{
    public static class CaptrOptionsBuilderExtensions
    {

        public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
        {
            return services.AddSingleton<IEventRepository, InMemoryEventRepository>();
        }

        public static IServiceCollection AddInMemorySnapshotStore(this IServiceCollection services)
        {
            return services.AddSingleton<ISnapshotRepository, InMemorySnapshotRepository>();
        }
    }
}