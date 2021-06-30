using Captr.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Captr.Extensions
{
	public static class AggregateExtensions
	{
		public static (string EventName, int EventVersion) GetEventDetails(this IAggregateEvent @event)
		{
			var eventType = @event.GetType();
			var attribute = eventType.GetCustomAttribute<AggregateEventAttribute>();
			string eventName = attribute?.EventName ?? eventType.Name;
			int eventVersion = attribute?.EventVersion ?? 1;

			return (eventName, eventVersion);
		}

		public static List<RegisteredEventInfo> RegisterApplyMethods<TEntity>(this Type entityType) where TEntity : IAggregateRoot
		{
			var eventType = typeof(IAggregateEvent);

			var aggregateEvents = entityType
				.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Select(m => new
				{
					Method = m,
					Param = m.GetParameters().FirstOrDefault()
				})
				.Where(m => m.Param != null && eventType.IsAssignableFrom(m.Param.ParameterType))
				.Select(m => new RegisteredEventInfo(
					GetEventName(m.Param.ParameterType),
					GetEventVersion(m.Param.ParameterType),
					m.Param.ParameterType,
					m.Method))
				.ToList();

			return aggregateEvents;

			// Local function to retrieve Event Name
			string GetEventName(Type parameterType)
			{
				var attribute = parameterType.GetCustomAttribute<AggregateEventAttribute>();
				return attribute?.EventName ?? parameterType.Name;
			}
			// Local function to retrieve Event Version
			int GetEventVersion(Type parameterType)
			{
				var attribute = parameterType.GetCustomAttribute<AggregateEventAttribute>();
				return attribute?.EventVersion ?? 1;
			}
		}

		public static RegisteredEventInfo GetApplyEventMethod(this IReadOnlyCollection<RegisteredEventInfo> aggregateEvents, Type eventType)
		{
			return aggregateEvents.FirstOrDefault(m => m.Parameter.Equals(eventType));
		}

		public static RegisteredEventInfo GetApplyEventMethod(this IReadOnlyCollection<RegisteredEventInfo> aggregateEvents, string eventName, int eventVersion)
		{
			return aggregateEvents.FirstOrDefault(m => m.EventName.Equals(eventName) && m.EventVersion.Equals(eventVersion));
		}
	}
}
