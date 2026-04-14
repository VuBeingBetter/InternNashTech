namespace MvcBankAccountSim.Domain.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber);
}