using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.Aggregates
{
	public interface IAggregateEvent
	{
		public DateTime Timestamp { get => DateTime.UtcNow; }
	}
}
