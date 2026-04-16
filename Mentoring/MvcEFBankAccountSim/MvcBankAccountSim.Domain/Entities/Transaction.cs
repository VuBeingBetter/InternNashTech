using MvcBankAccountSim.Domain.Enums;

namespace MvcBankAccountSim.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public string? AccountNumber { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Description { get; set; } = string.Empty;

    private Transaction() { }

    public Transaction(string accountNumber, TransactionType type, decimal amount, string description)
    {
        AccountNumber = accountNumber;
        Type = type;
        Amount = amount;
        Description = description;
    }
    public override string ToString()
        => $"[{Id}] {Type,-8} | {AccountNumber} | {Amount} | {CreatedAt} | {Description}";
}