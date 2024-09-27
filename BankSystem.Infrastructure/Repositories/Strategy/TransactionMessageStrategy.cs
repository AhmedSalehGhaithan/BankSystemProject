using BankSystem.Application.Interface.StrategyInterfaces;

namespace BankSystem.Infrastructure.Repositories.Strategy
{
    public class TransactionMessageStrategy : ITransactionMessageStrategy
    {
        public string GetCreateSuccessMessage() => "Transaction created successfully.";

        public string GetCreateErrorMessage() => "Error occurred while executing transaction.";

        public string GetDeleteSuccessMessage(int id) => $"Transaction with ID [{id}] deleted successfully.";

        public string GetNotFoundMessage(int id) => $"Transaction with ID [{id}] is not found.";

        public string GetDepositSuccessMessage(string accountHolder, decimal amount) =>
            $"Amount ${amount} added to {accountHolder}'s account.";

        public string GetWithdrawSuccessMessage(string accountHolder, decimal amount) =>
            $"${amount} deducted from {accountHolder}'s account.";

        public string GetTransferSuccessMessage(string sender, string receiver, decimal amount) =>
            $"Amount ${amount} successfully transferred from {sender} to {receiver}.";

        public string GetInsufficientBalanceMessage() => "Insufficient balance in the sender's account.";

        public string GetSelfTransferMessage() => "Cannot transfer to the same account.";

        public string GetAccountNotFoundMessage(string accountNumber) =>
            $"Account not found with number {accountNumber}.";
    }

}
