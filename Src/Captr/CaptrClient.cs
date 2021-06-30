using Captr.Aggregates;
using Captr.EventStorage;
using Captr.SnapshotStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr
{
	public class CaptrClient
	{
		private readonly IEventStore _eventStore;
		private readonly ISnapshotStore _snapshotStore;
		private readonly CaptrOptions _options;

		public CaptrClient(IEventStore eventStore, ISnapshotStore snapshotStore, CaptrOptions options)
		{
			_eventStore = eventStore;
			_snapshotStore = snapshotStore;
			_options = options;
		}

		/// <summary>
		/// Load an entity by reconstructing state from stored snapshots and events
		/// </summary>
		/// <typeparam name="TEntity">Type of entity</typeparam>
		/// <param name="entityId">Entity Id</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Returns a <typeparamref name="TEntity"/> in its current state; <code>NULL</code> otherwise</returns>
		public async Task<TEntity> LoadEntity<TEntity>(string entityId, CancellationToken cancellationToken = default) where TEntity : AggregateRoot<TEntity>, new()
		{
			int startVersion = 0;
			string streamId = GetStreamId<TEntity>(entityId);

			var snapshot = await _snapshotStore.LoadSnapshotAsync<TEntity>(streamId, cancellationToken);

			if (snapshot != null)
				startVersion = snapshot.Version;

			var eventStream = await _eventStore.LoadStreamAsync(streamId, startVersion, cancellationToken);

			if (snapshot == null && eventStream.Count == 0)
				return null;

			TEntity entity = new();
			await entity.InitialiseState(snapshot, eventStream, cancellationToken);

			return entity;
		}

		/// <summary>
		/// Save any <typeparamref name="TEntity"/> events.  If the number of events exceeds the next snapshot point, a snapshot is recorded. 
		/// </summary>
		/// <typeparam name="TEntity">Type of <see cref="AggregateRoot{TEntity}"/></typeparam>
		/// <param name="entity"><typeparamref name="TEntity"/></param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>Returns <code>TRUE</code> if events successfully saved; <code>FALSE</code> otherwise</returns>
		public async Task<bool> SaveEntityChanges<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : AggregateRoot<TEntity>
		{
			bool isSuccessful = false;

			if (entity == null)
				return isSuccessful;

			var changes = entity.GetChanges();
			if (changes.Any())
			{
				string streamId = GetStreamId<TEntity>(entity.GetAggregateId());

				isSuccessful = await _eventStore.AppendToStreamAsync(streamId, entity.Version, changes, cancellationToken);
				if (isSuccessful)
				{
					// TODO - Refactor - This logic needs to be improved as this
					// will save a snapshot not always at the next snapshot point
					int snapshotInterval = int.Parse(_options.GetConfigItem("SnapshotInterval"));
					int nextSnapshotPoint = ((entity.Version / snapshotInterval) * snapshotInterval) + snapshotInterval;
					if (entity.Version < nextSnapshotPoint && entity.Version + changes.Count >= nextSnapshotPoint)
					{
						entity.Version += changes.Count;
						_ = _snapshotStore.SaveSnapshotAsync(streamId, entity, cancellationToken);
					}
				}
			}

			return isSuccessful;
		}

		/// <summary>
		/// Get the stream Id in the form of "<entity name>||<entity id>"
		/// </summary>
		/// <typeparam name="TEntity">Type of <see cref="AggregateRoot{TEntity}"/></typeparam>
		/// <param name="id">Entity Id</param>
		/// <returns>Returns the stream Id</returns>
		private static string GetStreamId<TEntity>(string id) => $"{typeof(TEntity).Name.ToLower()}||{id}";
	}
}
