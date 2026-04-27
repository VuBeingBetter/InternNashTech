using Domain.Entities;
namespace Application.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<BankAccount>> GetAllAccountsAsync();
    Task<(IEnumerable<BankAccount> Items, int TotalCount)> GetAccountsAsync(int page, int pageSize, string search);
    Task<BankAccount?> GetAccountByNumberAsync(string accountNumber);
    Task CreateAccountAsync(string ownerName, decimal initialBalance);
    Task DepositAsync(string accountNumber, decimal amount);
    Task WithdrawAsync(string accountNumber, decimal amount);
    Task TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount);
    Task ToggleStatusAsync(string accountNumber);
}