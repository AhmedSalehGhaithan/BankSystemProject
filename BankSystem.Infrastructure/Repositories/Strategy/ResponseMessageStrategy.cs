using BankSystem.Application.Interface.StrategyInterfaces;
using BankSystem.Domain.Entities;

namespace BankSystem.Infrastructure.Repositories.Strategy
{
    public class ResponseMessageStrategy : IResponseMessageStrategy
    {
        public string GenerateValidationErrorMessage(string message) => message;

        public string GenerateCreateMessage(BankAccount account) =>
            $"Bank account for {account.FullName} created successfully with account number {account.AccountNumber}.";

        public string GenerateDeleteMessage(BankAccount account) =>
            $"Bank account for {account.FullName} deleted successfully.";

        public string GenerateUpdateMessage(BankAccount account) =>
            $"Bank account for {account.FullName} updated successfully.";

        public string GenerateErrorMessage(string operation) =>
            $"Error occurred while {operation} the bank account.";

        public string GenerateNotFoundMessage(int id) =>
            $"Bank account with ID [{id}] not found.";

        public string GenerateNotFoundMessage(string accountNumber) =>
            $"Bank account not found with account number {accountNumber}.";
    }

}
