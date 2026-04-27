using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IAccountRepository Accounts { get; }
    public ITransactionRepository Transactions { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Accounts = new AccountRepository(_context);
        Transactions = new TransactionRepository(_context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}