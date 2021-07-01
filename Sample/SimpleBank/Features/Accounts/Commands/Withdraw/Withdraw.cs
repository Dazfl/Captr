using Captr;
using MediatR;
using SimpleBank.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Features.Accounts.Commands.Withdraw
{
	public static class Withdraw
	{
		public record Command(string AccountNumber, long Amount) : IRequest<Response>;

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
				// Does the account exist?
				Account account = await _loadAccount(request.AccountNumber, cancellationToken);
				if (account == null)
					return Response.Fail("Account does not exist.");

				account.Withdraw(request.Amount);

				if (!await _saveAccount(account, cancellationToken))
					return Response.Fail("Unable to withdraw from this account at this time.");

				return Response.Success;
			}
		}

		public record Response
		{
			public bool IsSuccessful { get; init; } = true;
			public string Message { get; init; }

			public static Response Success => new();
			public static Response Fail(string message) => new() { IsSuccessful = false, Message = message };
		}
	}
}
