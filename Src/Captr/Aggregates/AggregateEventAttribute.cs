using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.Aggregates
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class AggregateEventAttribute : Attribute
	{
		/// <summary>
		/// Name of the event to be recorded
		/// </summary>
		public string EventName { get; set; }

		/// <summary>
		/// Version of the event to be recorded
		/// </summary>
		public int EventVersion { get; set; }

		public AggregateEventAttribute(string name, int version)
		{
			EventName = name;
			EventVersion = version;
		}
	}
}
