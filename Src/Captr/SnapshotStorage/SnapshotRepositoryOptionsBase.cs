using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.SnapshotStorage
{
	public abstract class SnapshotRepositoryOptionsBase : ISnapshotRepositoryOptions
	{
		/// <summary>
		/// How often a snapshot should be taken (default is 100)
		/// </summary>
		public virtual int SnapshotInterval { get; set; } = 100;
	}
}
