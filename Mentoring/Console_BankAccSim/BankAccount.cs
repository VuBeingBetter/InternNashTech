namespace Console_BankAccSim;

public class BankAccount
{
    public required string AccountNumber { get; set; }
    public required string OwnerName { get; set; }
    public decimal Balance { get; private set; } = 0;
    public AccountStatus Status { get; private set; } = AccountStatus.ACTIVE;
    public DateTime CreatedAt { get; set; }
}

public enum AccountStatus
{
    ACTIVE, FROZEN
}