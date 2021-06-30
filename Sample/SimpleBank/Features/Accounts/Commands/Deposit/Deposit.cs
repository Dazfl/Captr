using Captr;
using MediatR;
using SimpleBank.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Features.Accounts.Commands.Deposit
{
	public static class Deposit
	{
		/// <summary>
		/// Command request
		/// </summary>
		public record Command(string AccountNumber, long Amount) : IRequest<Response>;

		/// <summary>
		/// Command handler
		/// </summary>
		public class CommandHandler : IRequestHandler<Command, Response>
		{
			private readonly CaptrClient _captrClient;

			public CommandHandler(CaptrClient captrClient)
			{
				_captrClient = captrClient;
			}

			public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
			{
				// Does the account exist?
				Account account = await _captrClient.LoadEntity<Account>(request.AccountNumber, cancellationToken);
				if (account == null)
					return Response.Fail("Account does not exist.");

				account.Deposit(request.Amount);

				if (!await _captrClient.SaveEntityChanges(account, cancellationToken))
					return Response.Fail("Unable to deposit to this account at this time.");

				return Response.Success;
			}
		}

		/// <summary>
		/// Command response
		/// </summary>
		public record Response
		{
			public bool IsSuccessful { get; init; } = true;
			public string Message { get; set; }

			public static Response Success => new();
			public static Response Fail(string message) => new() { IsSuccessful = false, Message = message };
		}
	}
}
