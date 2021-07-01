using Captr;
using MediatR;
using SimpleBank.Domain.Aggregates;
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
			private readonly CaptrClientServices<Account>.LoadEntity _loadAccount;

			public QueryHandler(CaptrClientServices<Account>.LoadEntity loadAccount)
			{
				_loadAccount = loadAccount;
			}

			public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
			{
				Account account = await _loadAccount(request.AccountNumber, cancellationToken);

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
