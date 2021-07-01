using FluentAssertions;
using SimpleBank.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleBank.UnitTests.DomainTests.AggregateTests
{
	public class AccountTests
	{
		[Fact]
		public void Account_Create_ShouldSucceed()
		{
			// Arrange
			string accountNumber = "123456789";
			long openingBalance = 1000;
			bool isActive = true;

			// Act
			Account account = new(accountNumber, openingBalance, isActive);

			// Assert
			account.Should().NotBeNull();
			account.AccountNumber.Should().Be(accountNumber);
			account.Balance.Should().Be(openingBalance);
			account.IsActive.Should().Be(isActive);
			account.GetAggregateId().Should().Be(accountNumber);
			account.GetChanges().Count.Should().Be(1);
		}

		[Fact]
		public void Account_Deposit_ShouldSucceed()
		{
			// Arrange
			string accountNumber = "123456789";
			long openingBalance = 1000;
			long deposit = 500;
			bool isActive = true;

			// Act
			Account account = new(accountNumber, openingBalance, isActive);
			account.Deposit(deposit);

			// Assert
			account.Balance.Should().Be(openingBalance + deposit);
			account.GetChanges().Count.Should().Be(2);
		}

		[Fact]
		public void Account_Withdraw_ShouldSucceed()
		{
			// Arrange
			string accountNumber = "123456789";
			long openingBalance = 1000;
			long withdraw = 450;
			bool isActive = true;

			// Act
			Account account = new(accountNumber, openingBalance, isActive);
			account.Withdraw(withdraw);

			// Assert
			account.Balance.Should().Be(openingBalance - withdraw);
			account.GetChanges().Count.Should().Be(2);
		}
	}
}
