using FluentAssertions;
using SimpleBank.Domain.Aggregates;
using SimpleBank.Features.Accounts.Commands.CreateAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SimpleBank.UnitTests.FeatureTests.AccountsTests.CommandsTests.CreateAccountTests
{
	public class CreateAccountTests
	{
		[Fact]
		public async Task CreateAccount_ShouldSucceed()
		{
			// Arrange
			string accountNumber = "123456789";
			long openingBalance = 1000;
			bool isActive = true;
			var command = new CreateAccount.Command(accountNumber, openingBalance, isActive);
			Account account = null;
			CancellationToken token = CancellationToken.None;

			// Act
			var commandHandler = new CreateAccount.CommandHandler((x, token) => Task.FromResult(account), (x, token) => Task.FromResult(true));
			var result = await commandHandler.Handle(command, token);

			// Assert
			result.IsSuccessful.Should().BeTrue();
			result.Message.Should().BeNullOrEmpty();
		}
	}
}
