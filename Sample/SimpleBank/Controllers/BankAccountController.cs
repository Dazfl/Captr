using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleBank.Features.Accounts.Commands.CreateAccount;
using SimpleBank.Features.Accounts.Commands.Deposit;
using SimpleBank.Features.Accounts.Commands.Withdraw;
using SimpleBank.Features.Accounts.Queries.GetAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class BankAccountController : ControllerBase
	{
		private readonly IMediator _mediator;

		public BankAccountController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[HttpGet("{accountNumber}")]
		public async Task<IActionResult> GetAccount(string accountNumber, CancellationToken cancellationToken)
		{
			var response = await _mediator.Send(new GetAccount.Query(accountNumber), cancellationToken);
			if (response.IsSuccessful)
				return Ok(response.Account);

			return BadRequest(response);
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateAccount(CreateAccountRequest request, CancellationToken cancellationToken)
		{
			CreateAccount.Command command = new(request.AccountNumber, request.OpeningBalance, request.IsActive);
			var response = await _mediator.Send(command, cancellationToken);
			if (response.IsSuccessful)
				return Ok(response);

			return BadRequest(response);
		}

		[HttpPost("deposit")]
		public async Task<IActionResult> Deposit(DepositRequest request, CancellationToken cancellationToken)
		{
			Deposit.Command command = new(request.AccountNumber, request.Amount);
			var response = await _mediator.Send(command, cancellationToken);
			if (response.IsSuccessful)
				return Ok(response);

			return BadRequest(response);
		}

		[HttpPost("withdraw")]
		public async Task<IActionResult> Withdraw(WithdrawRequest request, CancellationToken cancellationToken)
		{
			Withdraw.Command command = new(request.AccountNumber, request.Amount);
			var response = await _mediator.Send(command, cancellationToken);
			if (response.IsSuccessful)
				return Ok(response);

			return BadRequest(response);
		}


		public record CreateAccountRequest(string AccountNumber, long OpeningBalance, bool IsActive);
		public record DepositRequest(string AccountNumber, long Amount);
		public record WithdrawRequest(string AccountNumber, long Amount);
	}
}
