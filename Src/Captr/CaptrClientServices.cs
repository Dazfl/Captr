using Captr.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Captr
{
	/// <summary>
	///		<para>
	///			Class containing services delegates that match the methods in CaptrClient.
	///		</para>
	///		<para>
	///			This allows the methods to be injected through dependency injection
	///			into class constructors.  This helps with Unit Testing (see the Sample for an example).
	///		</para>
	/// </summary>
	/// <typeparam name="TEntity">Type of <typeparamref name="TEntity"/></typeparam>
	public class CaptrClientServices<TEntity> where TEntity : AggregateRoot<TEntity>
	{
		public delegate Task<TEntity> LoadEntity(string entityId, CancellationToken cancellationToken);
		public delegate Task<bool> SaveEntityChanges(TEntity entity, CancellationToken cancellationToken);
	}
}
