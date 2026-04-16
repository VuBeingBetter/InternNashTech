using MvcBankAccountSim.Domain.Entities;

namespace MvcBankAccountSim.Application.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber);
}