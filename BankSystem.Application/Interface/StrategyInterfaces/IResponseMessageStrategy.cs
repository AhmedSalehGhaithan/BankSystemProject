using BankSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Application.Interface.StrategyInterfaces
{
    public interface IResponseMessageStrategy
    {
        string GenerateValidationErrorMessage(string message);
        string GenerateCreateMessage(BankAccount account);
        string GenerateDeleteMessage(BankAccount account);
        string GenerateUpdateMessage(BankAccount account);
        string GenerateErrorMessage(string operation);
        string GenerateNotFoundMessage(int id);
        string GenerateNotFoundMessage(string accountNumber);
    }
}
