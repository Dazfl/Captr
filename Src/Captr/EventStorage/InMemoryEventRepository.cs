using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.EventStorage
{
	public class InMemoryEventRepository : IEventRepository
	{
		private readonly List<EventDescriptor> _eventStorage = new();

		public InMemoryEventRepository()
		{

		}

		public Task<int> AppendEventsToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventDescriptor> events, CancellationToken cancellationToken = default)
		{
			if (_eventStorage.Any(e => streamId.Equals(e.Stream.StreamId) && expectedVersion.Equals(e.Stream.Version)))
				return Task.FromResult(-1);

			_eventStorage.AddRange(events);
			return Task.FromResult(events.Max(e => e.Stream.Version));
		}

		public Task<IReadOnlyCollection<EventDescriptor>> GetEventsFromStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default)
		{
			var events = _eventStorage
				.Where(e => streamId.Equals(e.Stream.StreamId) && e.Stream.Version >= fromVersion)
				.ToList();

			return Task.FromResult<IReadOnlyCollection<EventDescriptor>>(events);
		}
	}
}
