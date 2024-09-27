using BankSystem.Application.DTOs.AccountDTOs.Request.Account;
using BankSystem.Application.DTOs.AccountDTOs.Response;
using BankSystem.Application.DTOs.AccountDTOs.Response.Account;
using BankSystem.Application.Interface;
using BankSystem.Presentation.Controllers;
using BankSystem.SharedLibrarySolution.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankSystem.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccount> _accountMock;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _accountMock = new Mock<IAccount>();
            _controller = new AccountController(_accountMock.Object);
        }
        [Fact]
        public async Task GetRoles_ReturnsOk_WithEmptyList_WhenNoRolesAvailable()
        {
            // Arrange
            var roles = new List<GetRoleDTO>();
            _accountMock.Setup(x => x.GetRolesAsync()).ReturnsAsync(roles);

            // Act
            var result = await _controller.GetRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<GetRoleDTO>>(okResult.Value);
            Assert.Empty(returnValue); // Verify that the returned list is empty
        }
        [Fact]
        public async Task CreateAccount_ReturnsOk_WhenModelIsValid()
        {
            // Arrange
            var createAccountDTO = new CreateAccountDTO { EmailAddress = "user@example.com", Password = "Password123", Name = "User" };
            var response = new Response { Flag = true, Message = "Account created successfully" };
            _accountMock.Setup(x => x.CreateAccountAsync(createAccountDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateAccount(createAccountDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("EmailAddress", "Required");

            var createAccountDTO = new CreateAccountDTO();

            // Act
            var result = await _controller.CreateAccount(createAccountDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Model can not be null", badRequestResult.Value);
        }

        [Fact]
        public async Task LoginAccount_ReturnsOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDTO = new LoginDTO { EmailAddress = "user@example.com", Password = "Password123" };
            var response = new LoginResponse { Flag = true, Message = "Logged in successfully" };
            _accountMock.Setup(x => x.LoginAccountAsync(loginDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.LoginAccount(loginDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsOk_WhenUserIsDeleted()
        {
            // Arrange
            var email = "user@example.com";
            var response = new Response { Flag = true, Message = "User deleted successfully" };
            _accountMock.Setup(x => x.DeleteUserAsync(email)).ReturnsAsync(response);

            // Act
            var result = await _controller.DeleteAccount(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task RefreshToken_ReturnsOk_WhenModelIsValid()
        {
            // Arrange
            var refreshTokenDTO = new RefreshTokenDTO { Token = "someToken" };
            var response = new LoginResponse { Flag = true, Message = "Token refreshed successfully" };
            _accountMock.Setup(x => x.RefreshTokenAsync(refreshTokenDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.RefreshToken(refreshTokenDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task CreateRole_ReturnsOk_WhenModelIsValid()
        {
            // Arrange
            var createRoleDTO = new CreateRoleDTO { Name = "Admin" };
            var response = new Response { Flag = true, Message = "Role created successfully" };
            _accountMock.Setup(x => x.CreateRoleAsync(createRoleDTO)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateRole(createRoleDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task GetUsersWithRole_ReturnsOk_WhenUsersAreRetrieved()
        {
            // Arrange
            var usersWithRoles = new List<GetUsersWithRolesResponseDTO>
            {
                new GetUsersWithRolesResponseDTO { Name = "User1", Email = "user1@example.com", RoleName = "Admin" }
            };
            _accountMock.Setup(x => x.GetUsersWithRoles()).ReturnsAsync(usersWithRoles);

            // Act
            var result = await _controller.GetUsersWithRole();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(usersWithRoles, okResult.Value);
        }
    }
}
