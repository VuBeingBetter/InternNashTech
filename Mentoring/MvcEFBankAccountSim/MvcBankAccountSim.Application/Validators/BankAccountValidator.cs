using FluentValidation;
using MvcBankAccountSim.Domain.Entities;

namespace MvcBankAccountSim.Application.Validators;

public class BankAccountValidator : AbstractValidator<BankAccount>
{
    public BankAccountValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .Length(10).WithMessage("Account number must be 10 characters long.");
        RuleFor(x => x.OwnerName)
            .NotEmpty().WithMessage("Owner name is required.")
            .MinimumLength(2).WithMessage("Owner name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Owner name cannot exceed 100 characters.");
        RuleFor(x => x.Balance)
            .GreaterThanOrEqualTo(100).WithMessage("Balance cannot be less than $100.");
    }
}