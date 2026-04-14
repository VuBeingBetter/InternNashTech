using System.ComponentModel.DataAnnotations;

public class CreateAccountViewModel
{
    [Required]
    public string AccountNumber { get; set; }

    [Required]
    public string OwnerName { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InitialBalance { get; set; }
}