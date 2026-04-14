namespace MvcBankAccountSim.Domain.Interfaces;

public interface IAccountRepository : IRepository<BankAccount>
{
    Task<BankAccount> GetByAccountNumberAsync(string accountNumber);
}