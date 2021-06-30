using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.EventStorage
{
	public interface IEventStore
	{
		public Task<bool> AppendToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventInfo> events, CancellationToken cancellationToken = default);
		public Task<IReadOnlyCollection<EventInfo>> LoadStreamAsync(string streamId, CancellationToken cancellationToken = default);
		public Task<IReadOnlyCollection<EventInfo>> LoadStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default);
	}
}
