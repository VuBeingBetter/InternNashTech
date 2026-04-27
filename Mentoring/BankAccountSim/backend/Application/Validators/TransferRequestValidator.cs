using FluentValidation;

namespace Application.Validators;

public class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(x => x.FromAccountNumber)
            .NotEmpty().WithMessage("From account number is required.")
            .Length(10).WithMessage("From account number must be 10 characters long.");
        RuleFor(x => x.ToAccountNumber)
            .NotEmpty().WithMessage("To account number is required.")
            .Length(10).WithMessage("To account number must be 10 characters long.");
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transfer amount must be greater than zero.");
    }
}