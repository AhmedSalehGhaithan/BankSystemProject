using BankSystem.Application.Interface;
using BankSystem.Application.Interface.StrategyInterfaces;
using BankSystem.Domain.Entities;
using BankSystem.Infrastructure.Data;
using BankSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class TransactionRepositoryTests
{
    private readonly Mock<ITransactionMessageStrategy> _mockMessageStrategy;
    private readonly DbContextOptions<BankAccountDbContext> _dbContextOptions;
    private readonly BankAccountDbContext _dbContext;

    public TransactionRepositoryTests()
    {

        _mockMessageStrategy = new Mock<ITransactionMessageStrategy>();
        _dbContextOptions = new DbContextOptionsBuilder<BankAccountDbContext>()
            .UseInMemoryDatabase("BankAccountTestDb")
            .Options;
        _dbContext = new BankAccountDbContext(_dbContextOptions);

        // Setup default messages for the strategy
        _mockMessageStrategy.Setup(m => m.GetCreateSuccessMessage()).Returns("Transaction created successfully.");
        _mockMessageStrategy.Setup(m => m.GetCreateErrorMessage()).Returns("Error occurred while executing transaction.");
        _mockMessageStrategy.Setup(m => m.GetDeleteSuccessMessage(It.IsAny<int>())).Returns((int id) => $"Transaction with ID [{id}] deleted successfully.");
        _mockMessageStrategy.Setup(m => m.GetNotFoundMessage(It.IsAny<int>())).Returns((int id) => $"Transaction with ID [{id}] is not found.");
        _mockMessageStrategy.Setup(m => m.GetDepositSuccessMessage(It.IsAny<string>(), It.IsAny<decimal>())).Returns((string name, decimal amount) => $"Amount ${amount} added to {name}'s account.");
        _mockMessageStrategy.Setup(m => m.GetWithdrawSuccessMessage(It.IsAny<string>(), It.IsAny<decimal>())).Returns((string name, decimal amount) => $"${amount} deducted from {name}'s account.");
        _mockMessageStrategy.Setup(m => m.GetAccountNotFoundMessage(It.IsAny<string>())).Returns((string accountNumber) => $"Account not found with number {accountNumber}.");
    }

    private TransactionRepository GetRepository() =>
        new TransactionRepository(_dbContext, _mockMessageStrategy.Object);

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenTransactionIsValid()
    {
        // Arrange
        var repository = GetRepository();
        var transaction = new Transaction { Id = 1, Amount = 100, BankAccountNumber = "12345" };

        // Act
        var response = await repository.CreateAsync(transaction);

        // Assert
        Assert.True(response.Flag);
        Assert.Equal("Transaction created successfully.", response.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenTransactionIsNull()
    {
        // Arrange
        var repository = GetRepository();
        Transaction transaction = null;

        // Act
        var response = await repository.CreateAsync(transaction);

        // Assert
        Assert.False(response.Flag);
        Assert.Equal("Error occurred while executing transaction.", response.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenTransactionDoesNotExist()
    {
        // Arrange
        var repository = GetRepository();
        var transaction = new Transaction { Id = 999 }; // Non-existing ID

        // Act
        var response = await repository.DeleteAsync(transaction);

        // Assert
        Assert.False(response.Flag);
        Assert.Contains("Transaction with ID [999] is not found.", response.Message);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldThrowException_WhenTransactionDoesNotExist()
    {
        // Arrange
        var repository = GetRepository();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await repository.FindByIdAsync(999));
    }

    [Fact]
    public async Task FindByIdAsync_ShouldThrowException_WhenIdIsNegative()
    {
        // Arrange
        var repository = GetRepository();

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await repository.FindByIdAsync(-1));
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnSuccess_WhenAccountExists()
    {
        // Arrange
        var repository = GetRepository();
        var account = new BankAccount { AccountNumber = "12345", Balance = 0, FullName = "John Doe" };
        _dbContext.BankAccounts.Add(account);
        await _dbContext.SaveChangesAsync();

        var transaction = new Transaction { Amount = 100, BankAccountNumber = "12345" };

        // Act
        var response = await repository.DepositAsync(transaction);

        // Assert
        Assert.True(response.Flag);
        Assert.Contains("Amount $100 added to John Doe's account.", response.Message);
    }

    [Fact]
    public async Task DepositAsync_ShouldReturnError_WhenAccountDoesNotExist()
    {
        // Arrange
        var repository = GetRepository();
        var transaction = new Transaction { Amount = 100, BankAccountNumber = "non-existent-account" };

        // Act
        var response = await repository.DepositAsync(transaction);

        // Assert
        Assert.False(response.Flag);
        Assert.Contains("Account not found with number non-existent-account.", response.Message);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldReturnSuccess_WhenAccountExists()
    {
        // Arrange
        var repository = GetRepository();
        var account = new BankAccount { AccountNumber = "12345", Balance = 200, FullName = "Jane Doe" };
        _dbContext.BankAccounts.Add(account);
        await _dbContext.SaveChangesAsync();

        var transaction = new Transaction { Amount = 100, BankAccountNumber = "12345" };

        // Act
        var response = await repository.WithdrawAsync(transaction);

        // Assert
        Assert.True(response.Flag);
        Assert.Contains("$100 deducted from Jane Doe's account.", response.Message);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTransactions()
    {
        // Arrange
        var repository = GetRepository();
        _dbContext.Transactions.Add(new Transaction { Id = 1, Amount = 100 });
        _dbContext.Transactions.Add(new Transaction { Id = 2, Amount = 200 });
        await _dbContext.SaveChangesAsync();

        // Act
        var transactions = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, transactions.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var repository = GetRepository();

        // Act
        var transactions = await repository.GetAllAsync();

        // Assert
        Assert.Empty(transactions);
    }

    [Fact]
    public async Task GetTransactionsByBankAccountNumberAsync_ShouldReturnTransactions_WhenExist()
    {
        // Arrange
        var repository = GetRepository();
        var transaction = new Transaction { Amount = 100, BankAccountNumber = "12345" };
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        // Act
        var transactions = await repository.GetTransactionsByBankAccountNumberAsync("12345");

        // Assert
        Assert.Single(transactions);
    }

    [Fact]
    public async Task GetTransactionsByBankAccountNumberAsync_ShouldReturnEmpty_WhenNoTransactionsForAccount()
    {
        // Arrange
        var repository = GetRepository();

        // Act
        var transactions = await repository.GetTransactionsByBankAccountNumberAsync("non-existent-account");

        // Assert
        Assert.Empty(transactions);
    }
}
