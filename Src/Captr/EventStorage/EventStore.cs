using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.EventStorage
{
	public class EventStore : IEventStore
	{
		private readonly IEventRepository _repository;

		public EventStore(IEventRepository repository)
		{
			_repository = repository;
		}

		/// <summary>
		/// Capture events and save to the event store
		/// </summary>
		/// <param name="streamId">Stream Id</param>
		/// <param name="expectedVersion">The version the stream is expected to be at</param>
		/// <param name="events">List of events to capture</param>
		/// <param name="cancellationToken">Cancellation token to optionally cancel the operation</param>
		/// <returns>Returns TRUE if the event(s) were captured successfully</returns>
		public async Task<bool> AppendToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventInfo> events, CancellationToken cancellationToken = default)
		{
			if (events == null || events.Count == 0)
				return false;

			List<EventDescriptor> eventList = SerialiseEvents(streamId, expectedVersion, events);

			var newVersion = await _repository.AppendEventsToStreamAsync(streamId, expectedVersion, eventList, cancellationToken);

			return newVersion > expectedVersion;
		}

		/// <summary>
		/// Load all the events for a given Stream
		/// </summary>
		/// <param name="streamId">Stream Id</param>
		/// <param name="cancellationToken">Cancellation token to optionally cancel the operation</param>
		/// <returns>Returns a list of events for the given Stream</returns>
		public Task<IReadOnlyCollection<EventInfo>> LoadStreamAsync(string streamId, CancellationToken cancellationToken = default)
		{
			return LoadStreamAsync(streamId, 0, cancellationToken);
		}

		/// <summary>
		/// Load all the events for a given Stream from a specified event version
		/// </summary>
		/// <param name="streamId">Stream Id</param>
		/// <param name="fromVersion">Version to start retrieving from</param>
		/// <param name="cancellationToken">Cancellation token to optionally cancel the operation</param>
		/// <returns>Returns a list of events from the given Stream</returns>
		public async Task<IReadOnlyCollection<EventInfo>> LoadStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default)
		{
			IReadOnlyCollection<EventDescriptor> eventDescriptors = await _repository.GetEventsFromStreamAsync(streamId, fromVersion, cancellationToken);

			return DeserialiseEvents(eventDescriptors);
		}


		/// <summary>
		/// Convert a list of IEvent to a list of Event Descriptors
		/// </summary>
		/// <param name="streamId">Id of the stream the list of events belongs to</param>
		/// <param name="expectedVersion">Version that the event stream is expected to be at</param>
		/// <param name="events">List of events from client</param>
		/// <returns>Returns a list of event descriptors; Empty list otherwise</returns>
		private static List<EventDescriptor> SerialiseEvents(string streamId, int expectedVersion, IReadOnlyCollection<EventInfo> events)
		{
			List<EventDescriptor> eventDescriptors = new(0);

			foreach (var @event in events)
			{
				// Form the event descriptor and add to the resulting list
				EventDescriptor eventDescriptor = new()
				{
					Id = streamId,
					Timestamp = DateTime.UtcNow,
					Stream = new StreamInfo(streamId, ++expectedVersion),
					Event = @event,
					Metadata = null
				};

				eventDescriptors.Add(eventDescriptor);
			}

			return eventDescriptors;
		}

		/// <summary>
		/// Convert a list of Event Descriptors to a list of Events
		/// </summary>
		/// <param name="eventDescriptors">List of event descriptors retrieved from the Event Store</param>
		/// <returns>Returns a list of Events successfully deserialised from their Event Descriptors; Empty list otherwise</returns>
		private static IReadOnlyCollection<EventInfo> DeserialiseEvents(IReadOnlyCollection<EventDescriptor> eventDescriptors)
		{
			return eventDescriptors
				.OrderBy(ed => ed.Stream.Version)
				.Select(ed => ed.Event)
				.ToList();
		}
	}
}
