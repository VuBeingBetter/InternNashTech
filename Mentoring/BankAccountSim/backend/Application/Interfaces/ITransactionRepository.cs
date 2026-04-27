using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber, TransactionType? type, int pageNumber, int pageSize);
}