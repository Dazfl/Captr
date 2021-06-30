using Captr;
using MediatR;
using SimpleBank.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Features.Accounts.Queries.GetAccount
{
	public static class GetAccount
	{
		/// <summary>
		/// Query request
		/// </summary>
		public record Query(string AccountNumber) : IRequest<Response>;

		/// <summary>
		/// Query handler
		/// </summary>
		public class QueryHandler : IRequestHandler<Query, Response>
		{
			private readonly CaptrClient _captrClient;

			public QueryHandler(CaptrClient captrClient)
			{
				_captrClient = captrClient;
			}

			public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
			{
				Account account = await _captrClient.LoadEntity<Account>(request.AccountNumber, cancellationToken);

				if (account == null)
					return new Response(false, "The Account does not exist.", null);
				
				return new Response(true, string.Empty, account);
			}
		}

		/// <summary>
		/// Query response
		/// </summary>
		public record Response(bool IsSuccessful, string Message, Account Account);
	}
}
