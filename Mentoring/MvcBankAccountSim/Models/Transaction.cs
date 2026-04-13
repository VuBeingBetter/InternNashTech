using System.ComponentModel.DataAnnotations;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string AccountNumber { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? Description { get; set; } = "";

    public Transaction() { }
    public override string ToString()
        => $"[{Id}] {Type,-8} | {AccountNumber} | {Amount} | {CreatedAt} | {Description}";
}

public enum TransactionType
{
    DEPOSIT, WITHDRAW, TRANSFER
}
