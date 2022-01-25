using FluentValidation;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class QuotedTransactionModelValidator : AbstractValidator<QuotedTransactionModel>
{
    public QuotedTransactionModelValidator()
    {
        RuleFor(r => r.Sender)
            .MustBeNetworkAddress().WithMessage("Sender must be valid address.");
        RuleFor(r => r.To)
            .MustBeNetworkAddress().WithMessage("Sender must be valid address.");
        RuleFor(r => r.Amount)
            .MustBeValidCrsValue().WithMessage("Amount must contain 8 decimal places.");
        RuleFor(r => r.Method)
            .NotEmpty().WithMessage("Method name must be provided.");
        RuleForEach(r => r.Parameters)
            .SetValidator(new TransactionParameterModelValidator());
        RuleFor(r => r.Callback)
            .NotEmpty().WithMessage("Callback URL must be provided.");
    }
}
