using FluentValidation;

public class WithdrawRequestValidator : AbstractValidator<WithdrawRequest>
{
    public WithdrawRequestValidator()
    {
        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.");
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Withdrawal amount must be greater than zero.");
    }
}