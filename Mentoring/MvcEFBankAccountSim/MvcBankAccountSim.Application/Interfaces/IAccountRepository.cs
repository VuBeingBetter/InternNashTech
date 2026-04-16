using MvcBankAccountSim.Domain.Entities;

namespace MvcBankAccountSim.Application.Interfaces;

public interface IAccountRepository : IRepository<BankAccount>
{
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);
}