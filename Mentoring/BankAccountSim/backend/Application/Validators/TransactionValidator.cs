using FluentValidation;
using Domain.Entities;

namespace Application.Validators;

public class TransactionValidator : AbstractValidator<Transaction>
{
    public TransactionValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Transaction amount must be greater than zero.");
        RuleFor(x => x.CreatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Transaction date cannot be in the future.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid transaction type.");
        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description cannot exceed 200 characters.");
    }
}