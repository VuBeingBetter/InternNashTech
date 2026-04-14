namespace MvcBankAccountSim.Domain.Entities;

public class BankAccount
{
    public string AccountNumber { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; private set; } = 0;
    public AccountStatus Status { get; set; } = AccountStatus.ACTIVE;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    private BankAccount() { }

    public BankAccount(string accountNumber, string ownerName, decimal balance, AccountStatus status, DateTime createdAt)
    {
        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = balance;
        Status = status;
        CreatedAt = createdAt;
    }
    public BankAccount(string accountNumber, string ownerName, decimal balance)
    {
        AccountNumber = accountNumber;
        OwnerName = ownerName;
        Balance = balance;
    }

    public override string ToString()
        => $"[{AccountNumber}] Owner: {OwnerName} | Created at: {CreatedAt} | Status: {Status} | Balance: {Balance}";

    public void ChangeStatus (AccountStatus newStatus)=> Status = newStatus;

    public void Deposit(decimal amount) => Balance += amount;

    public void Withdraw(decimal amount) => Balance -= amount;

}