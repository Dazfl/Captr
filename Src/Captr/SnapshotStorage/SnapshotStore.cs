using Captr.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.SnapshotStorage
{
	public class SnapshotStore : ISnapshotStore
	{
		private readonly ISnapshotRepository _repository;

		public SnapshotStore(ISnapshotRepository repository)
		{
			_repository = repository;
		}

		public async Task<TEntity> LoadSnapshotAsync<TEntity>(string streamId, CancellationToken cancellationToken = default) where TEntity : AggregateRoot<TEntity>
		{
			var snapshotDescriptor = await _repository.GetSnapshotAsync(streamId, cancellationToken);

			if (snapshotDescriptor == null)
				return default;

			//Type entityType = Type.GetType(snapshotDescriptor.SnapshotType);
			TEntity entity = (TEntity)JsonSerializer.Deserialize(snapshotDescriptor.SnapshotPayload, typeof(TEntity));
			return entity;
		}

		public async Task<bool> SaveSnapshotAsync<TEntity>(string streamId, TEntity entity, CancellationToken cancellationToken = default) where TEntity : AggregateRoot<TEntity>
		{
			if (string.IsNullOrEmpty(streamId) || entity == null)
				return false;

			SnapshotDescriptor snapshotDescriptor = new()
			{
				StreamId = streamId,
				SnapshotType = typeof(TEntity).Name,
				SnapshotPayload = JsonSerializer.Serialize(entity, typeof(TEntity)),
				Version = entity.Version
			};

			return await _repository.SaveSnapshotAsync(snapshotDescriptor, cancellationToken);
		}
	}
}
