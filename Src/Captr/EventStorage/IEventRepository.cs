using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Captr.EventStorage
{
    public interface IEventRepository
    {
        /// <summary>
        /// Add events to storage
        /// </summary>
        /// <param name="streamId">Event stream Id</param>
        /// <param name="expectedVersion">Expected version of stream</param>
        /// <param name="events">Collection of events</param>
        /// <param name="cancellationToken">(Optional) Cancellation token</param>
        /// <returns>Returns the new stream version</returns>
        public Task<int> AppendEventsToStreamAsync(string streamId, int expectedVersion, IReadOnlyCollection<EventDescriptor> events, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve events from storage
        /// </summary>
        /// <param name="streamId">Event stream Id</param>
        /// <param name="fromVersion">Retrieve stream of events starting from version</param>
        /// <param name="cancellationToken">(Optional) Cancellation token</param>
        /// <returns>Returns a collection of events</returns>
        public Task<IReadOnlyCollection<EventDescriptor>> GetEventsFromStreamAsync(string streamId, int fromVersion, CancellationToken cancellationToken = default);
    }
}
