using BankSystem.Application.Interface.StrategyInterfaces;
using BankSystem.Domain.Entities;
using BankSystem.Infrastructure.Data;
using BankSystem.Infrastructure.Repositories;
using BankSystem.Infrastructure.Repositories.Strategy;
using Microsoft.EntityFrameworkCore;
using Moq;

public class BankAccountRepositoryTests
{
    private readonly DbContextOptions<BankAccountDbContext> _dbContextOptions;
    private readonly BankAccountDbContext _dbContext;

    public BankAccountRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<BankAccountDbContext>()
            .UseInMemoryDatabase("BankAccountTestDb")
            .Options;
        _dbContext = new BankAccountDbContext(_dbContextOptions);
    }

    private BankAccountRepository GetRepository(IResponseMessageStrategy messageStrategy) =>
        new BankAccountRepository(_dbContext, messageStrategy);

    [Fact]
    public async Task CreateAsync_ShouldReturnSuccess_WhenAccountIsValid()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { FullName = "John Doe", PhoneNumber = "1234567890" };

        // Act
        var response = await repository.CreateAsync(account);

        // Assert
        Assert.True(response.Flag);
        Assert.Equal("Account John Doe created successfully.", response.Message);
        Assert.NotNull(account.AccountNumber);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenAccountIsNull()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        BankAccount account = null;

        // Act
        var response = await repository.CreateAsync(account);

        // Assert
        Assert.False(response.Flag);
        Assert.Equal("The account entity cannot be null.", response.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenPhoneNumberAlreadyExists()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account1 = new BankAccount { FullName = "John Doe", PhoneNumber = "1234567890" };
        await repository.CreateAsync(account1);
        var account2 = new BankAccount { FullName = "Jane Doe", PhoneNumber = "1234567890" };

        // Act
        var response = await repository.CreateAsync(account2);

        // Assert
        Assert.False(response.Flag);
        Assert.Contains("This Phone number already exists!", response.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnSuccess_WhenAccountExists()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { FullName = "John Doe" };
        await repository.CreateAsync(account);

        // Act
        var response = await repository.DeleteAsync(account);

        // Assert
        Assert.True(response.Flag);
        Assert.Contains("deleted successfully", response.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { Id = 999, FullName = "John Doe" }; // Non-existing ID

        // Act
        var response = await repository.DeleteAsync(account);

        // Assert
        Assert.False(response.Flag);
        Assert.Contains("not found", response.Message);
    }

    [Fact]
    public async Task FindByIdAsync_ShouldReturnAccount_WhenExists()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { FullName = "John Doe" };
        await repository.CreateAsync(account);

        // Act
        var foundAccount = await repository.FindByIdAsync(account.Id);

        // Assert
        Assert.NotNull(foundAccount);
        Assert.Equal(account.FullName, foundAccount.FullName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoAccountsExist()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());

        // Act
        var accounts = await repository.GetAllAsync();

        // Assert
        Assert.Empty(accounts);
    }

    [Fact]
    public async Task GetByAccountNumberAsync_ShouldReturnAccount_WhenExists()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { FullName = "John Doe", PhoneNumber = "1234567890" };
        await repository.CreateAsync(account);

        // Act
        var foundAccount = await repository.GetByAccountNumberAsync(account.AccountNumber);

        // Assert
        Assert.NotNull(foundAccount);
        Assert.Equal(account.FullName, foundAccount.FullName);
    }

    [Fact]
    public async Task GetByAccountNumberAsync_ShouldReturnNull_WhenAccountDoesNotExist()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());

        // Act
        var account = await repository.GetByAccountNumberAsync("non-existent-account");

        // Assert
        Assert.Null(account);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenAccountExists()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { FullName = "John Doe" };
        await repository.CreateAsync(account);
        account.FullName = "John Smith";

        // Act
        var response = await repository.UpdateAsync(account);

        // Assert
        Assert.True(response.Flag);
        Assert.Contains("updated successfully", response.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        var repository = GetRepository(new ResponseMessageStrategy());
        var account = new BankAccount { Id = 999, FullName = "John Doe" }; // Non-existing ID

        // Act
        var response = await repository.UpdateAsync(account);

        // Assert
        Assert.False(response.Flag);
        Assert.Contains("not found", response.Message);
    }
}
