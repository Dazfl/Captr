using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr.EventStorage
{
	public record EventDescriptor
	{
		public string Id { get; init; }
		public DateTime Timestamp { get; init; }
		public StreamInfo Stream { get; init; }
		public EventInfo Event { get; init; }
		public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
	}

	public record StreamInfo
	{
		public string StreamId { get; init; }
		public int Version { get; init; } = 1;

		public StreamInfo() { }

		public StreamInfo(string streamId, int version)
		{
			StreamId = streamId;
			Version = version;
		}
	}

	public record EventInfo
	{
		public string Name { get; init; }
		public int Version { get; init; } = 1;
		public string Payload { get; init; }
		public EventInfo() { }

		public EventInfo(string name, int version, string payload)
		{
			Name = name;
			Version = version;
			Payload = payload;
		}
	}
}
