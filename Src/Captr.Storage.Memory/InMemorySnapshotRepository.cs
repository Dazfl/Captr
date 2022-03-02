using Captr.SnapshotStorage;
using System.Collections.Immutable;

namespace Captr.Storage.Memory
{
    public class InMemorySnapshotRepository : ISnapshotRepository
    {
        private readonly ImmutableList<SnapshotDescriptor> _snapshotStorage;

        public InMemorySnapshotRepository()
        {
            _snapshotStorage = ImmutableList<SnapshotDescriptor>.Empty;
        }

        public async Task<SnapshotDescriptor> GetSnapshotAsync(string streamId, CancellationToken cancellationToken = default)
        {
            SnapshotDescriptor snapshot = await Task.Run(() => _snapshotStorage.FirstOrDefault(s => s.StreamId == streamId));
            return snapshot;
        }

        public Task<bool> SaveSnapshotAsync(SnapshotDescriptor snapshotDescriptor, CancellationToken cancellationToken = default)
        {
            var snapshot = _snapshotStorage.FirstOrDefault(s => s.StreamId == snapshotDescriptor.StreamId);

            if (snapshot is null)
                _snapshotStorage.Add(snapshotDescriptor);
            else
                _snapshotStorage.Replace(snapshot, snapshotDescriptor);

            return Task.FromResult(true);
        }
    }
}
