using Captr.Aggregates;
using Captr.EventStorage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Domain.Aggregates
{
	/// <summary>
	/// Account Aggregate class
	/// </summary>
	public class Account : AggregateRoot<Account>
	{
		#region Aggregate Properties

		public string AccountNumber { get; set; }

		public long Balance { get; set; }

		public bool IsActive { get; set; }


		#endregion


		public Account() : base() { }

		/// <summary>
		/// Constructor to rehydrate the entity with events
		/// </summary>
		/// <param name="events"></param>
		public Account(IEnumerable<EventInfo> events) : base(events) { }

		/// <summary>
		/// Constructor to create a new account
		/// </summary>
		/// <param name="accountNumber">Account Number</param>
		/// <param name="balance">Opening balance in lowest currency unit (e.g. cents)</param>
		/// <param name="isActive">Activate (<code>TRUE</code>) or Deactivate (<code>FALSE</code>) the new account</param>
		public Account(string accountNumber, long balance, bool isActive)
		{
			EmitEvent(new AccountEvents.AccountCreated(accountNumber, balance, isActive));
		}

		/// <summary>
		/// Initialises the state of <see cref="Account"/> based on a supplied snapshot and additional events
		/// </summary>
		/// <param name="entitySnapshot">The last recorded snapshot</param>
		/// <param name="events">Any additional events since the last recorded snapshot</param>
		/// <param name="cancellationToken">Cancellation token</param>
		/// <returns>Returns a completed task once state has been initialised</returns>
		public override Task InitialiseState(Account entitySnapshot, IEnumerable<EventInfo> events, CancellationToken cancellationToken = default)
		{
			if (entitySnapshot != null)
			{
				SetAggregateId(entitySnapshot.AccountNumber);
				AccountNumber = entitySnapshot.AccountNumber;
				Balance = entitySnapshot.Balance;
				IsActive = entitySnapshot.IsActive;
				Version = entitySnapshot.Version;
			}

			ApplyEvents(events);

			return Task.CompletedTask;
		}


		#region Aggregate Events

		public void Deposit(long amount)
		{
			if (amount <= 0)
				return;

			EmitEvent(new AccountEvents.Deposit(amount));
		}

		public void Withdraw(long amount)
		{
			if (amount <= 0)
				return;

			if (Balance - amount < 0)
				return;

			EmitEvent(new AccountEvents.Withdraw(amount));
		}

		public void Activate()
		{
			if (IsActive)
				return;

			EmitEvent(new AccountEvents.Activated());
		}

		public void Deactivate()
		{
			if (!IsActive)
				return;

			EmitEvent(new AccountEvents.Deactivated());
		}

		#endregion


		#region Aggregate Apply Methods

		protected void Apply(AccountEvents.AccountCreated @event)
		{
			SetAggregateId(@event.AccountNumber);
			AccountNumber = @event.AccountNumber;
			Balance = @event.Balance;
			IsActive = @event.IsActive;
		}

		protected void Apply(AccountEvents.Deposit @event)
		{
			Balance += @event.Amount;
		}

		protected void Apply(AccountEvents.Withdraw @event)
		{
			Balance -= @event.Amount;
		}

		protected void Apply(AccountEvents.Activated @event)
		{
			IsActive = true;
		}

		protected void Apply(AccountEvents.Deactivated @event)
		{
			IsActive = false;
		}

		#endregion
	}


	public static class AccountEvents
	{
		[AggregateEvent("Created", 1)]
		public record AccountCreated(string AccountNumber, long Balance, bool IsActive) : BaseAggregateEvent;
		public record Deposit(long Amount) : BaseAggregateEvent;
		public record Withdraw(long Amount) : BaseAggregateEvent;
		public record Activated() : BaseAggregateEvent;
		public record Deactivated() : BaseAggregateEvent;
	}

}
