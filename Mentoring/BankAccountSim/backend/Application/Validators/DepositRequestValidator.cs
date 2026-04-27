using FluentValidation;

public class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    public DepositRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Deposit amount must be greater than zero.");
        RuleFor(x => x.AccountNumber).NotEmpty().WithMessage("Account number is required.");
    }
}