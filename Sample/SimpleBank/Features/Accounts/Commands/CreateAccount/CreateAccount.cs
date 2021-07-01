using Captr;
using MediatR;
using SimpleBank.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Features.Accounts.Commands.CreateAccount
{
	/// <summary>
	/// Class to handle the command that creates a new Bank Account
	/// </summary>
	public static class CreateAccount
	{
		/// <summary>
		/// Command request
		/// </summary>
		public record Command(string AccountNumber, long OpeningBalance, bool IsActive) : IRequest<Response>;

		/// <summary>
		/// Command handler
		/// </summary>
		public class CommandHandler : IRequestHandler<Command, Response>
		{
			private readonly CaptrClientServices<Account>.LoadEntity _loadAccount;
			private readonly CaptrClientServices<Account>.SaveEntityChanges _saveAccount;

			public CommandHandler(CaptrClientServices<Account>.LoadEntity loadAccount, CaptrClientServices<Account>.SaveEntityChanges saveAccount)
			{
				_loadAccount = loadAccount;
				_saveAccount = saveAccount;
			}

			public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
			{
				// Does the Account already exist?
				//Account account = await _captrClient.LoadEntity<Account>(request.AccountNumber, cancellationToken);
				Account account = await _loadAccount(request.AccountNumber, cancellationToken);
				if (account != null)
					return Response.Fail("Account already exists.");

				account = new(request.AccountNumber, request.OpeningBalance, request.IsActive);

				//if (!await _captrClient.SaveEntityChanges(account, cancellationToken))
				if (!await _saveAccount(account, cancellationToken))
					return Response.Fail("Unable to create the new account at this time.");

				return Response.Success;
			}
		}

		/// <summary>
		/// Command response
		/// </summary>
		public record Response
		{
			public bool IsSuccessful { get; init; } = true;
			public string Message { get; init; }

			public static Response Success => new();
			public static Response Fail(string message) => new() { IsSuccessful = false, Message = message };

		};
	}
}
