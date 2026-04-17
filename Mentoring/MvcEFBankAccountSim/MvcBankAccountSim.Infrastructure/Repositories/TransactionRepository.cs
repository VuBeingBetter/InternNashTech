using Microsoft.EntityFrameworkCore;
using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Infrastructure.Data;


namespace MvcBankAccountSim.Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber)
    {
        return await _dbSet
            .Where(t => t.AccountNumber == accountNumber)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}