using MvcBankAccountSim.Application.Interfaces;
using MvcBankAccountSim.Domain.Entities;
using MvcBankAccountSim.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MvcBankAccountSim.Infrastructure.Repositories;

public class AccountRepository : Repository<BankAccount>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context) { }

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }
}