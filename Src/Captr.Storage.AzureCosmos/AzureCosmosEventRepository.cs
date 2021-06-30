using Captr.EventStorage;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.Storage.AzureCosmos
{
	public class AzureCosmosEventRepository : IEventRepository
	{
		private readonly Container _eventContainer;

		public AzureCosmosEventRepository(Container eventContainer)
		{
			_eventContainer = eventContainer;
		}

		public Task<int> AppendEventsToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventDescriptor> events, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyCollection<EventDescriptor>> GetEventsFromStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
