using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _uow;

    public AccountService(IUnitOfWork uow)
    {
        _uow = uow;
    }


    public async Task CreateAccountAsync(string ownerName, decimal initialBalance)
    {
        string newAccNumber = await GenerateUniqueAccountNumberAsync();
        var account = new BankAccount(newAccNumber, ownerName, initialBalance);
        
        await _uow.Accounts.AddAsync(account);
        await _uow.SaveChangesAsync();
    }

    private async Task<decimal> GetTotalWithdrawalTodayAsync(string accountNumber)
    {
        var today = DateTime.UtcNow.Date;
        var transactions = await _uow.Transactions.GetByAccountNumberAsync(accountNumber, TransactionType.WITHDRAW, 1, int.MaxValue);
        return transactions.Where(t => t.CreatedAt.Date == today).Sum(t => t.Amount);
    }

    public async Task<IEnumerable<BankAccount>> GetAllAccountsAsync() 
        => await _uow.Accounts.GetAllAsync();

    public async Task<(IEnumerable<BankAccount> Items, int TotalCount)> GetAccountsAsync(int page, int pageSize, string search)
    {
        // Only call repository, service does not need to know about query logic
        return await _uow.Accounts.GetPagedAsync(page, pageSize, search);
    }

    public async Task<BankAccount?> GetAccountByNumberAsync(string accountNumber)
        => await _uow.Accounts.GetByAccountNumberAsync(accountNumber);

    private async Task<string> GenerateUniqueAccountNumberAsync()
    {
        string newAccNumber;
        do
        {
            // Create a random 10-digit account number
            var bytes = new byte[10]; 
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            // Chuyển byte thành chuỗi số 0-9
            newAccNumber = string.Concat(bytes.Select(b => (b % 10).ToString()));

        } while (await _uow.Accounts.GetByAccountNumberAsync(newAccNumber) != null); // Check duplicates

        return newAccNumber;
    }

    public async Task DepositAsync(string accountNumber, decimal amount)
    {
        var account = await GetAndValidateAccount(accountNumber);

        if (account.Status == AccountStatus.FROZEN) 
        {
            throw new Exception("Account is frozen. Cannot perform transactions.");
        }
        account.Deposit(amount);
        
        await _uow.Transactions.AddAsync(new Transaction(accountNumber, TransactionType.DEPOSIT, amount, "Deposit cash"));
        await _uow.SaveChangesAsync();
    }

    public async Task WithdrawAsync(string accountNumber, decimal amount)
    {
        var account = await GetAndValidateAccount(accountNumber);

        if (account.Status == AccountStatus.FROZEN) 
        {
            throw new Exception("Account is frozen. Cannot perform transactions.");
        }

        if (account.Balance - amount < 100)
            throw new InvalidOperationException("Balance must remain >= $100.");

        var totalWithdrawToday = await GetTotalWithdrawalTodayAsync(accountNumber);
        if (totalWithdrawToday + amount > 5000)
            throw new InvalidOperationException("Daily withdrawal limit of $5000 exceeded.");

        account.Withdraw(amount);
        await _uow.Transactions.AddAsync(new Transaction(accountNumber, TransactionType.WITHDRAW, amount, "Withdrawal"));
        await _uow.SaveChangesAsync();
    }

    public async Task TransferAsync(string fromAcc, string toAcc, decimal amount)
    {
        var source = await GetAndValidateAccount(fromAcc);

        if (source.Status == AccountStatus.FROZEN) 
        {
            throw new Exception("Source account is frozen. Cannot perform transactions.");
        }
        var destination = await _uow.Accounts.GetByAccountNumberAsync(toAcc) 
            ?? throw new Exception("Destination account not found.");
        if (destination.Status == AccountStatus.FROZEN) 
        {
            throw new Exception("Destination account is frozen. Cannot perform transactions.");
        }

        if (source.Balance - amount < 100)
            throw new InvalidOperationException("Insufficient funds to maintain minimum balance.");

        source.Withdraw(amount);
        destination.Deposit(amount);

        // Tạo 2 bản ghi giao dịch cho 1 lần chuyển tiền
        await _uow.Transactions.AddAsync(new Transaction(fromAcc, TransactionType.TRANSFER, amount, $"Transfer to {toAcc}"));
        await _uow.Transactions.AddAsync(new Transaction(toAcc, TransactionType.TRANSFER, amount, $"Transfer from {fromAcc}"));

        await _uow.SaveChangesAsync();
    }

    public async Task ToggleStatusAsync(string accountNumber)
    {
        var account = await _uow.Accounts.GetByAccountNumberAsync(accountNumber) ?? throw new Exception("Not found");
        var newStatus = account.Status == AccountStatus.ACTIVE ? AccountStatus.FROZEN : AccountStatus.ACTIVE;
        account.ChangeStatus(newStatus);
        await _uow.SaveChangesAsync();
    }

    private async Task<BankAccount> GetAndValidateAccount(string accountNumber)
    {
        var account = await _uow.Accounts.GetByAccountNumberAsync(accountNumber) 
            ?? throw new Exception("Account not found.");
        
        if (account.Status == AccountStatus.FROZEN)
            throw new InvalidOperationException("Account is frozen. Transaction denied.");
            
        return account;
    }
}