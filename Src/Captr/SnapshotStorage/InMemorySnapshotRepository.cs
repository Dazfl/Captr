using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.SnapshotStorage
{
	public class InMemorySnapshotRepository : ISnapshotRepository
	{
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
