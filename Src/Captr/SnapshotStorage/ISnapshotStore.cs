using Captr.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.SnapshotStorage
{
	public interface ISnapshotStore
	{
		public Task<TEntity> LoadSnapshotAsync<TEntity>(string streamId, CancellationToken cancellationToken = default) where TEntity : AggregateRoot<TEntity>;

		public Task<bool> SaveSnapshotAsync<TEntity>(string streamId, TEntity entity, CancellationToken cancellationToken = default) where TEntity : AggregateRoot<TEntity>;
	}
}
