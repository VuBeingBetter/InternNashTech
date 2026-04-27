using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : Repository<BankAccount>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context) { }

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<(IEnumerable<BankAccount> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string search)
    {
        var query = _context.Accounts.AsQueryable();

        // Filter if there are search terms
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => a.AccountNumber.Contains(search) || a.OwnerName.Contains(search));
        }

        // Đếm tổng số bản ghi TRƯỚC KHI skip/take
        var totalCount = await query.CountAsync();

        // Paginating
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}