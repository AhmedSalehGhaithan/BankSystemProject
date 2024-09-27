namespace BankSystem.Application.Interface.StrategyInterfaces
{
    public interface ITransactionMessageStrategy
    {
        string GetCreateSuccessMessage();
        string GetCreateErrorMessage();
        string GetDeleteSuccessMessage(int id);
        string GetNotFoundMessage(int id);
        string GetDepositSuccessMessage(string accountHolder, decimal amount);
        string GetWithdrawSuccessMessage(string accountHolder, decimal amount);
        string GetTransferSuccessMessage(string sender, string receiver, decimal amount);
        string GetInsufficientBalanceMessage();
        string GetSelfTransferMessage();
        string GetAccountNotFoundMessage(string accountNumber);
    }

}
