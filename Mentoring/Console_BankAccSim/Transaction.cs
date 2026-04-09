namespace Console_BankAccSim;

public class Transaction
{
    public required int Id { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }

}

public enum TransactionType
{
    DEPOSIT, WITHDRAW, TRANSFER
}
