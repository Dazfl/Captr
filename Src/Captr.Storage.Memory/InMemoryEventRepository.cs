using Captr.EventStorage;
using System.Collections.Immutable;

namespace Captr.Storage.Memory
{
    public class InMemoryEventRepository : IEventRepository
    {
        private readonly ImmutableList<EventDescriptor> _eventStorage;

        public InMemoryEventRepository()
        {
            _eventStorage = ImmutableList<EventDescriptor>.Empty;
        }

        /// <inheritdoc cref="IEventRepository.AppendEventsToStreamAsync(string, int, IReadOnlyCollection{EventDescriptor}, CancellationToken)" />
        public Task<int> AppendEventsToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventDescriptor> events, CancellationToken cancellationToken = default)
        {
            if (_eventStorage.Any(e => streamId.Equals(e.Stream.StreamId) && expectedVersion.Equals(e.Stream.Version)))
                return Task.FromResult(-1);

            _eventStorage.AddRange(events);
            return Task.FromResult(events.Max(e => e.Stream.Version));
        }

        /// <inheritdoc cref="IEventRepository.GetEventsFromStreamAsync(string, int, CancellationToken)" />
        public Task<IReadOnlyCollection<EventDescriptor>> GetEventsFromStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default)
        {
            var events = _eventStorage
                .Where(e => streamId.Equals(e.Stream.StreamId) && e.Stream.Version >= fromVersion)
                .ToList();

            return Task.FromResult<IReadOnlyCollection<EventDescriptor>>(events);
        }
    }
}
