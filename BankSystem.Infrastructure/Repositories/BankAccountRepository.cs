using BankSystem.Application.Interface;
using BankSystem.Application.Interface.StrategyInterfaces;
using BankSystem.Domain.Entities;
using BankSystem.Infrastructure.Data;
using BankSystem.SharedLibrarySolution.Logs;
using BankSystem.SharedLibrarySolution.Responses;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccount
    {
        private readonly BankAccountDbContext _context;
        private readonly IResponseMessageStrategy _messageStrategy;

        public BankAccountRepository(BankAccountDbContext context, IResponseMessageStrategy messageStrategy)
        {
            _context = context;
            _messageStrategy = messageStrategy;
        }

        public async Task<Response> CreateAsync(BankAccount entity)
        {
            if (entity == null)
                return new Response(false, _messageStrategy.GenerateValidationErrorMessage("The account entity cannot be null."));

            try
            {
                string generatedAccountNumber = await GenerateRandomNumber();
                entity.AccountNumber = generatedAccountNumber;

                if (await IsPhoneNumberAlreadyExist(entity.PhoneNumber!))
                    return new Response(false, _messageStrategy.GenerateValidationErrorMessage("This phone number already exists!"));

                await _context.BankAccounts.AddAsync(entity);
                await _context.SaveChangesAsync();

                return new Response(true, _messageStrategy.GenerateCreateMessage(entity));
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, _messageStrategy.GenerateErrorMessage("adding new account"));
            }
        }

        public async Task<bool> IsPhoneNumberAlreadyExist(string phoneNumber) =>
            await _context.BankAccounts.AnyAsync(account => account.PhoneNumber == phoneNumber);

        public async Task<string> GenerateRandomNumber()
        {
            try
            {
                string generatedAccountNumber;
                bool isUnique;
                Random random = new Random();

                do
                {
                    generatedAccountNumber = random.Next(100000000, 999999999).ToString();
                    var existingAccount = await _context.BankAccounts.FirstOrDefaultAsync(account => account.AccountNumber == generatedAccountNumber);
                    isUnique = existingAccount == null;
                }
                while (!isUnique);

                return generatedAccountNumber;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred generating account number", ex);
            }
        }

        public async Task<Response> DeleteAsync(BankAccount entity)
        {
            try
            {
                var account = await FindByIdAsync(entity.Id);
                if (account is null)
                    return new Response(false, _messageStrategy.GenerateNotFoundMessage(entity.Id));

                _context.BankAccounts.Remove(account);
                await _context.SaveChangesAsync();

                return new Response(true, _messageStrategy.GenerateDeleteMessage(entity));
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, _messageStrategy.GenerateErrorMessage("deleting account"));
            }
        }

        public async Task<Response> UpdateAsync(BankAccount entity)
        {
            try
            {
                var account = await FindByIdAsync(entity.Id);
                if (account is null)
                    return new Response(false, _messageStrategy.GenerateNotFoundMessage(entity.Id));

                _context.Entry(account).State = EntityState.Detached;
                _context.BankAccounts.Update(entity);
                await _context.SaveChangesAsync();

                return new Response(true, _messageStrategy.GenerateUpdateMessage(entity));
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response(false, _messageStrategy.GenerateErrorMessage("updating account"));
            }
        }

        public async Task<BankAccount> FindByIdAsync(int id)
        {
            var account = await _context.BankAccounts.FindAsync(id);
            if (account == null)
                throw new Exception(_messageStrategy.GenerateNotFoundMessage(id));

            return account;
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync()
        {
            try
            {
                return await _context.BankAccounts.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred retrieving accounts", ex);
            }
        }

        public async Task<BankAccount> GetByAccountNumberAsync(string accountNumber)
        {
            var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new Exception(_messageStrategy.GenerateNotFoundMessage(accountNumber));

            return account;
        }
    }
}
