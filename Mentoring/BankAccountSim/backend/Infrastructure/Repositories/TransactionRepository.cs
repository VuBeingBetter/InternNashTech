using Microsoft.EntityFrameworkCore;

using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;



namespace Infrastructure.Repositories;

public class TransactionRepository : Repository<Transaction>, ITransactionRepository
{
    public TransactionRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Transaction>> GetByAccountNumberAsync(string accountNumber, TransactionType? type, int pageNumber, int pageSize)
    {
        var query = _dbSet.Where(t => t.AccountNumber == accountNumber);

        if (type.HasValue)
        {
            query = query.Where(t => t.Type == type.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}