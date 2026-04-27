using Domain.Entities;

namespace Application.Interfaces;

public interface IAccountRepository : IRepository<BankAccount>
{
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);
    Task<(IEnumerable<BankAccount> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string search);
}