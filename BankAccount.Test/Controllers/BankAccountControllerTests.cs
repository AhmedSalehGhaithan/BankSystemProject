using Moq;
using Microsoft.AspNetCore.Mvc;
using BankSystem.Application.Interface;
using BankSystem.Domain.Entities;
using BankSystem.SharedLibrarySolution.Responses;
using BankSystem.Presentation.Controllers;

public class BankAccountControllerTests
{
    private readonly BankAccountController _controller;
    private readonly Mock<IBankAccount> _bankAccountServiceMock;

    public BankAccountControllerTests()
    {
        _bankAccountServiceMock = new Mock<IBankAccount>();
        _controller = new BankAccountController(_bankAccountServiceMock.Object);
    }

    [Fact]
    public async Task GetAccounts_ReturnsOk_WithListOfAccounts()
    {
        // Arrange
        var accounts = new List<BankAccount>
        {
            new BankAccount { Id = 1, AccountNumber = "123456" },
            new BankAccount { Id = 2, AccountNumber = "654321" }
        };
        _bankAccountServiceMock.Setup(x => x.GetAllAsync()).ReturnsAsync(accounts);

        // Act
        var result = await _controller.GetAccounts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<BankAccount>>(okResult.Value);
        Assert.Equal(2, ((List<BankAccount>)returnValue).Count);
    }

    [Fact]
    public async Task CreateAccount_ReturnsOk_WithResponse()
    {
        // Arrange
        var accountDTO = new BankAccount { Id = 1, AccountNumber = "123456" };
        var expectedResponse = new Response(true, "Account created");
        _bankAccountServiceMock.Setup(x => x.CreateAsync(accountDTO)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.CreateAccount(accountDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Account created", returnValue.Message);
    }

    [Fact]
    public async Task GetAccountById_ReturnsOk_WithAccount()
    {
        // Arrange
        var accountId = 1;
        var account = new BankAccount { Id = accountId, AccountNumber = "123456" };
        _bankAccountServiceMock.Setup(x => x.FindByIdAsync(accountId)).ReturnsAsync(account);

        // Act
        var result = await _controller.GetAccountById(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<BankAccount>(okResult.Value);
        Assert.Equal(accountId, returnValue.Id);
    }

    [Fact]
    public async Task GetByAccountNumber_ReturnsOk_WithAccount()
    {
        // Arrange
        var accountNumber = "123456";
        var account = new BankAccount { Id = 1, AccountNumber = accountNumber };
        _bankAccountServiceMock.Setup(x => x.GetByAccountNumberAsync(accountNumber)).ReturnsAsync(account);

        // Act
        var result = await _controller.GetByAccountNumber(accountNumber);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<BankAccount>(okResult.Value);
        Assert.Equal(accountNumber, returnValue.AccountNumber);
    }

    [Fact]
    public async Task UpdateBankAccount_ReturnsOk_WithResponse()
    {
        // Arrange
        var account = new BankAccount { Id = 1, AccountNumber = "123456" };
        var expectedResponse = new Response(true, "Account updated");
        _bankAccountServiceMock.Setup(x => x.UpdateAsync(account)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.UpdateBankAccount(account);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Account updated", returnValue.Message);
    }

    [Fact]
    public async Task DeleteBankAccount_ReturnsOk_WithResponse()
    {
        // Arrange
        var accountId = 1;
        var account = new BankAccount { Id = accountId, AccountNumber = "123456" };
        var expectedResponse = new Response(true, "Account deleted");
        _bankAccountServiceMock.Setup(x => x.FindByIdAsync(accountId)).ReturnsAsync(account);
        _bankAccountServiceMock.Setup(x => x.DeleteAsync(account)).ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.DeleteBankAccount(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Response>(okResult.Value);
        Assert.True(returnValue.Flag);
        Assert.Equal("Account deleted", returnValue.Message);
    }
}
