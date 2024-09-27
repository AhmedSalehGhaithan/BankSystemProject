using Moq;
using Microsoft.AspNetCore.Mvc;
using BankSystem.Application.Interface;
using BankSystem.Domain.Entities;
using BankSystem.SharedLibrarySolution.Responses;
using BankSystem.Presentation.Controllers;

public class TransactionControllerTests
{
    private readonly TransactionController _controller;
    private readonly Mock<ITransaction> _transactionServiceMock;
    private readonly Mock<IMyCacheService> _cacheServiceMock;

    public TransactionControllerTests()
    {
        _transactionServiceMock = new Mock<ITransaction>();
        _cacheServiceMock = new Mock<IMyCacheService>();
        _controller = new TransactionController(_transactionServiceMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task GetTransactions_ReturnsOk_WithListOfTransactions()
    {
        // Arrange
        var transactions = new List<Transaction>
        {
            new Transaction { Id = 1, Amount = 100, TransactionType = "Deposit" },
            new Transaction { Id = 2, Amount = 50, TransactionType = "Withdraw" }
        };
        _transactionServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transactions);

        // Act
        var result = await _controller.GetTransactions();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Transaction>>(okResult.Value);
        Assert.Equal(2, ((List<Transaction>)returnValue).Count);
    }

    [Fact]
    public async Task CreateTransaction_ReturnsOk_WithResponse()
    {
        // Arrange
        var transaction = new Transaction { Id = 1, Amount = 100, TransactionType = "Deposit" };
        var expectedResponse = new Response(true, "Transaction created");
        _transactionServiceMock.Setup(x => x.CreateAsync(transaction)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateTransaction(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Transaction created", returnValue.Message);
    }

    [Fact]
    public async Task GetTransaction_ReturnsOk_WithTransaction()
    {
        // Arrange
        var transactionId = 1;
        var transaction = new Transaction { Id = transactionId, Amount = 100, TransactionType = "Deposit" };
        _transactionServiceMock.Setup(x => x.FindByIdAsync(transactionId)).ReturnsAsync(transaction);

        // Act
        var result = await _controller.GetTransaction(transactionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Transaction>(okResult.Value);
        Assert.Equal(transactionId, returnValue.Id);
    }

    [Fact]
    public async Task GetByAccountNumber_ReturnsOk_WithListOfTransactions()
    {
        // Arrange
        var accountNumber = "123456";
        var transactions = new List<Transaction>
    {
        new Transaction { Id = 1, Amount = 100, TransactionType = "Deposit", BankAccountNumber = accountNumber },
        new Transaction { Id = 2, Amount = 50, TransactionType = "Withdraw",  ReceiverAccountNumber= accountNumber }
    };

        _transactionServiceMock.Setup(x => x.GetTransactionsByBankAccountNumberAsync(accountNumber))
            .ReturnsAsync(transactions);

        // Act
        var result = await _controller.GetByAccountNumber(accountNumber);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Transaction>>(okResult.Value);
        Assert.Equal(2, ((List<Transaction>)returnValue).Count);
    }


    [Fact]
    public async Task Deposit_ReturnsOk_WithResponse()
    {
        // Arrange
        var transaction = new Transaction { Id = 1, Amount = 100, TransactionType = "Deposit" };
        var expectedResponse = new Response(true, "Deposit successful");
        _transactionServiceMock.Setup(x => x.DepositAsync(transaction)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Deposit(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Deposit successful", returnValue.Message);
    }

    [Fact]
    public async Task Withdraw_ReturnsOk_WithResponse()
    {
        // Arrange
        var transaction = new Transaction { Id = 1, Amount = 50, TransactionType = "Withdraw" };
        var expectedResponse = new Response(true, "Withdrawal successful");
        _transactionServiceMock.Setup(x => x.WithdrawAsync(transaction)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Withdraw(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Withdrawal successful", returnValue.Message);
    }

    [Fact]
    public async Task Transfer_ReturnsOk_WithResponse()
    {
        // Arrange
        var transaction = new Transaction { Id = 1, Amount = 100, TransactionType = "Transfer" };
        var expectedResponse = new Response(true, "Transfer successful");
        _transactionServiceMock.Setup(x => x.TransferAsync(transaction)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Transfer(transaction);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Transfer successful", returnValue.Message);
    }

    [Fact]
    public async Task DeleteTransaction_ReturnsOk_WithResponse()
    {
        // Arrange
        var transactionId = 1;
        var transaction = new Transaction { Id = transactionId, Amount = 100, TransactionType = "Deposit" };
        var expectedResponse = new Response(true, "Transaction deleted");
        _transactionServiceMock.Setup(x => x.FindByIdAsync(transactionId)).ReturnsAsync(transaction);
        _transactionServiceMock.Setup(x => x.DeleteAsync(transaction)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.DeleteTransaction(transactionId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Transaction deleted", returnValue.Message);
    }
}
