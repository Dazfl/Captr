using Captr.EventStorage;
using Captr.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.Aggregates
{
	/// <summary>
	/// Base class that an aggregate MUST inherit in order to capture events
	/// </summary>
	/// <typeparam name="TEntity">Type of <see cref="AggregateRoot{TEntity}"/></typeparam>
	public abstract class AggregateRoot<TEntity> : IAggregateRoot where TEntity : AggregateRoot<TEntity>
	{
		private static readonly IReadOnlyCollection<RegisteredEventInfo> _registeredEvents;

		private readonly List<EventInfo> _changes;

		private protected string AggregateId { get; private set; }

		public int Version { get; set; }

		/// <summary>
		/// Static constructor - used to register aggregate events
		/// </summary>
		static AggregateRoot()
		{
			_registeredEvents = typeof(TEntity).RegisterApplyMethods<TEntity>();
		}

		public AggregateRoot()
		{
			_changes = new(0);
			Version = 0;
		}

		protected AggregateRoot(IEnumerable<EventInfo> events) 
		{
			_changes = new(0);
			Version = 0;
			ApplyEvents(events);
		}

		/// <summary>
		/// Retrieve a list of changes made to <typeparamref name="TEntity"/>
		/// </summary>
		/// <returns>Returns a list of IEvents</returns>
		public IReadOnlyCollection<EventInfo> GetChanges() => _changes;

		public string GetAggregateId() => AggregateId;

		protected void SetAggregateId(string id) => AggregateId = id;

		/// <summary>
		/// Method to initialise the state of <typeparamref name="TEntity"/>.
		/// </summary>
		/// <param name="entitySnapshot">Recent snapshot of <typeparamref name="TEntity"/>, if applicable</param>
		/// <param name="events">Event stream to bring <typeparamref name="TEntity"/> to current state</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Returns a Task once completed</returns>
		public abstract Task InitialiseState(TEntity entitySnapshot, IEnumerable<EventInfo> events, CancellationToken cancellationToken = default);

		/// <summary>
		/// Add an event to the change list
		/// </summary>
		/// <param name="event">Event of type <see cref="IAggregateEvent"/></param>
		private protected void AddChange(IAggregateEvent @event)
		{
			var (EventName, EventVersion) = @event.GetEventDetails();
			EventInfo eventInfo = new()
			{
				Name = EventName,
				Version = EventVersion,
				Payload = JsonSerializer.Serialize(@event, @event.GetType())
			};
			_changes.Add(eventInfo);
		}

		/// <summary>
		/// Method to capture and apply a state change event.
		/// </summary>
		/// <typeparam name="TEvent">Event of type IEvent</typeparam>
		/// <param name="event">Event to capture and apply</param>
		protected void EmitEvent<TEvent>(TEvent @event) where TEvent : IAggregateEvent
		{
			AddChange(@event);

			ApplyEvent(@event);
		}

		/// <summary>
		/// Apply a stream of events to <typeparamref name="TEntity"/>
		/// </summary>
		/// <typeparam name="TEvent">Event of type IEvent</typeparam>
		/// <param name="events">List of events</param>
		protected void ApplyEvents(IEnumerable<EventInfo> events)
		{
			foreach (var @event in events)
			{
				ApplyEvent(@event);
				Version += 1;
			}
		}

		/// <summary>
		/// Apply an event to <typeparamref name="TEntity"/>
		/// </summary>
		/// <typeparam name="TEvent">Event of type IEvent</typeparam>
		/// <param name="event">Event to apply to <typeparamref name="TEntity"/></param>
		private void ApplyEvent<TEvent>(TEvent @event) where TEvent : IAggregateEvent
		{
			var aggregateEventInfo = _registeredEvents.GetApplyEventMethod(@event.GetType());
			var entity = this as TEntity;

			aggregateEventInfo.Method?.Invoke(entity, new object[] { @event });
		}

		/// <summary>
		/// Apply an event to <typeparamref name="TEntity"/>
		/// </summary>
		/// <typeparam name="TEvent">Event of type IEvent</typeparam>
		/// <param name="event">Event to apply to <typeparamref name="TEntity"/></param>
		private void ApplyEvent(EventInfo eventInfo)
		{
			var aggregateEventInfo = _registeredEvents.GetApplyEventMethod(eventInfo.Name, eventInfo.Version);
			var entity = this as TEntity;
			var @event = (IAggregateEvent)JsonSerializer.Deserialize(eventInfo.Payload, aggregateEventInfo.Parameter);

			aggregateEventInfo.Method?.Invoke(entity, new object[] { @event });
		}
	}
}
