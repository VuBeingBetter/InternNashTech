namespace MvcBankAccountSim.Application.DTOs;

public class CreateAccountRequest
{
    public string OwnerName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}