using Captr.SnapshotStorage;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.Storage.AzureCosmos
{
	public class AzureCosmosSnapshotRepository : ISnapshotRepository
	{
		private readonly Container _snapshotContainer;

		public AzureCosmosSnapshotRepository(Container snapshotContainer)
		{
			_snapshotContainer = snapshotContainer;
		}

		public Task<SnapshotDescriptor> GetSnapshotAsync(string streamId, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<bool> SaveSnapshotAsync(SnapshotDescriptor snapshotDescriptor, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
