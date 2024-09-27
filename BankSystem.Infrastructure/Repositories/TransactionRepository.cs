using BankSystem.Application.Interface;
using BankSystem.Application.Interface.StrategyInterfaces;
using BankSystem.Domain.Entities;
using BankSystem.Infrastructure.Data;
using BankSystem.SharedLibrarySolution.Logs;
using BankSystem.SharedLibrarySolution.Responses;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Repositories
{
    public class TransactionRepository : ITransaction
    {
        private readonly BankAccountDbContext _context;
        private readonly ITransactionMessageStrategy _messageStrategy;

        public TransactionRepository(BankAccountDbContext context,ITransactionMessageStrategy messageStrategy)
        {
            _context = context;
            _messageStrategy = messageStrategy;
        }

        public async Task<Response> CreateAsync(Transaction entity)
        {
            if (entity == null) return new Response(false, _messageStrategy.GetCreateErrorMessage());

            try
            {
                await _context.Transactions.AddAsync(entity);
                await _context.SaveChangesAsync();
                return new Response(true, _messageStrategy.GetCreateSuccessMessage());
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, _messageStrategy.GetCreateErrorMessage());
            }
        }

        public async Task<Response> DeleteAsync(Transaction entity)
        {
            var existingTransaction = await _context.Transactions.FindAsync(entity.Id);
            if (existingTransaction == null) return new Response(false, _messageStrategy.GetNotFoundMessage(entity.Id));

            try
            {
                _context.Transactions.Remove(existingTransaction);
                await _context.SaveChangesAsync();
                return new Response(true, _messageStrategy.GetDeleteSuccessMessage(entity.Id));
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, _messageStrategy.GetCreateErrorMessage());
            }
        }

        public async Task<Transaction> FindByIdAsync(int id) =>
            await _context.Transactions.FindAsync(id) ?? throw new Exception(_messageStrategy.GetNotFoundMessage(id));

        public async Task<Response> DepositAsync(Transaction entity)
        {
            var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == entity.BankAccountNumber);
            if (account == null) return new Response(false, _messageStrategy.GetAccountNotFoundMessage(entity.BankAccountNumber));

            account.Balance += entity.Amount;
            entity.TransactionType = "Deposit";

            await CreateAsync(entity);
            return new Response(true, _messageStrategy.GetDepositSuccessMessage(account.FullName, entity.Amount));
        }

        public async Task<Response> WithdrawAsync(Transaction entity)
        {
            var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == entity.BankAccountNumber);
            if (account == null) return new Response(false, _messageStrategy.GetAccountNotFoundMessage(entity.BankAccountNumber));

            account.Balance -= entity.Amount;
            entity.TransactionType = "Withdraw";
            entity.TransactionDate = DateTime.Now;

            await CreateAsync(entity);
            return new Response(true, _messageStrategy.GetWithdrawSuccessMessage(account.FullName, entity.Amount));
        }

        public async Task<Response> TransferAsync(Transaction entity)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var sender = await _context.BankAccounts.FirstOrDefaultAsync(b => b.AccountNumber == entity.BankAccountNumber);
                    var receiver = await _context.BankAccounts.FirstOrDefaultAsync(b => b.AccountNumber == entity.ReceiverAccountNumber);

                    if (sender == null || receiver == null)
                        return new Response(false, _messageStrategy.GetAccountNotFoundMessage(entity.BankAccountNumber));

                    if (sender.Balance < entity.Amount)
                        return new Response(false, _messageStrategy.GetInsufficientBalanceMessage());

                    if (sender.AccountNumber == receiver.AccountNumber)
                        return new Response(false, _messageStrategy.GetSelfTransferMessage());

                    sender.Balance -= entity.Amount;
                    receiver.Balance += entity.Amount;
                    entity.TransactionType = "Transfer";

                    await CreateAsync(entity);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new Response(true, _messageStrategy.GetTransferSuccessMessage(sender.FullName, receiver.FullName, entity.Amount));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    LogException.LogExceptions(ex);
                    return new Response(false, _messageStrategy.GetCreateErrorMessage());
                }
            });
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync() =>
            await _context.Transactions.AsNoTracking().ToListAsync();

        public async Task<IEnumerable<Transaction>> GetTransactionsByBankAccountNumberAsync(string bankAccountNumber) =>
            await _context.Transactions.Where(t => t.BankAccountNumber == bankAccountNumber).ToListAsync();
    }
}
