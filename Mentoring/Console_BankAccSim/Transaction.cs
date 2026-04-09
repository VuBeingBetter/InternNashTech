using System.Diagnostics.Contracts;

namespace Console_BankAccSim;

public class Transaction
{
    public required int Id { get; set; }
    public required string AccountNumber { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Description { get; set; } = "";

    public override string ToString()
        => $"[{Id}] {Type,-8} | {AccountNumber} | {Amount} | {CreatedAt} | {Description}";
}

public enum TransactionType
{
    DEPOSIT, WITHDRAW, TRANSFER
}
