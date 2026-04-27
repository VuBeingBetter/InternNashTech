using Domain.Entities;

namespace Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetHistoryAsync(string accountNumber, string filter, int pageNumber = 1, int pageSize = 10);
}