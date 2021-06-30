using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.SnapshotStorage
{
	public class SnapshotDescriptor
	{
		public string StreamId { get; init; }
		public int Version { get; init; }
		public string SnapshotType { get; init; }
		public string SnapshotPayload { get; init; }
	}
}
