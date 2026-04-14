namespace MvcBankAccountSim.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public required string AccountNumber { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Description { get; set; } = "";

    private Transaction() { }
    public override string ToString()
        => $"[{Id}] {Type,-8} | {AccountNumber} | {Amount} | {CreatedAt} | {Description}";
}