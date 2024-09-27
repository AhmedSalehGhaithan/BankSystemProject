using BankSystem.Application.DTOs.AccountDTOs.Request.Account;
namespace BankSystem.Application.Interface
{
    public interface IAdminConstants
    {
        string Name { get; }
        string Email { get; }
        string Password { get; }
        CreateAccountDTO CreateFirstAdmin();
    }
}
