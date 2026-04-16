using MvcBankAccountSim.Domain.Entities;
namespace MvcBankAccountSim.Application.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<BankAccount>> GetAllAccountsAsync();
    Task<BankAccount?> GetAccountByNumberAsync(string accountNumber);
    Task CreateAccountAsync(string ownerName, decimal initialBalance);
    Task DepositAsync(string accountNumber, decimal amount);
    Task WithdrawAsync(string accountNumber, decimal amount);
    Task TransferAsync(string fromAccountNumber, string toAccountNumber, decimal amount);
    Task ToggleStatusAsync(string accountNumber);
}