using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _uow;

    public TransactionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Transaction>> GetHistoryAsync(string accountNumber, string filter, int pageNumber = 1, int pageSize = 10)
    {
        TransactionType? type = Enum.TryParse<TransactionType>(filter, true, out var parsedType) ? parsedType : null;
        var transactions = await _uow.Transactions.GetByAccountNumberAsync(accountNumber, type, pageNumber, pageSize);

        if (string.IsNullOrEmpty(filter) || filter.Equals("all", StringComparison.OrdinalIgnoreCase))
            return transactions;

        return transactions.Where(t => t.Type.ToString().Equals(filter, StringComparison.OrdinalIgnoreCase));
    }
}