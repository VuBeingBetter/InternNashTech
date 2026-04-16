using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Domain.Enums;

namespace MvcBankAccountSim.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _uow;

    public TransactionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Transaction>> GetHistoryAsync(string accountNumber, string filter)
    {
        var transactions = await _uow.Transactions.GetByAccountNumberAsync(accountNumber);

        if (string.IsNullOrEmpty(filter) || filter.Equals("all", StringComparison.OrdinalIgnoreCase))
            return transactions;

        return transactions.Where(t => t.Type.ToString().Equals(filter, StringComparison.OrdinalIgnoreCase));
    }
}