using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.EventStorage
{
	public interface IEventRepository
	{
		public Task<int> AppendEventsToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventDescriptor> events, CancellationToken cancellationToken = default);

		public Task<IReadOnlyCollection<EventDescriptor>> GetEventsFromStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default);
	}
}
