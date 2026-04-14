namespace MvcBankAccountSim.Infrastructure.Repositories;

public class AccountRepository : Repository<BankAccount>, IAccountRepository
{
    public AccountRepository(AppDbContext context) : base(context) { }

    public async Task<BankAccount> GetByAccountNumberAsync(string accountNumber)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }
}