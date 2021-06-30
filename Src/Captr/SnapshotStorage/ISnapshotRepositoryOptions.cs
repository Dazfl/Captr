using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.SnapshotStorage
{
	public interface ISnapshotRepositoryOptions
	{
		public int SnapshotInterval { get; set; }
	}
}
