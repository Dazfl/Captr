using System;
using System.Reflection;

namespace Captr.Aggregates
{
	/// <summary>
	/// Record to capture the details of a registered <see cref="IAggregateEvent"/>.
	/// </summary>
	public record RegisteredEventInfo(string EventName, int EventVersion, Type Parameter, MethodInfo Method);
}
