using Microsoft.Extensions.DependencyInjection;

namespace Captr.Storage.Memory
{
    public static class CaptrOptionsBuilderExtensions
    {
        public static CaptrOptionsBuilder UseInMemoryAsEventStore(this CaptrOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddService(new ServiceDescriptor(typeof(InMemoryEventRepository), new InMemoryEventRepository()));
            return optionsBuilder;
        }


        public static CaptrOptionsBuilder UseInMemoryAsSnapshotStore(this CaptrOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddService(new ServiceDescriptor(typeof(InMemorySnapshotRepository), new InMemorySnapshotRepository()));
            return optionsBuilder;
        }
    }
}