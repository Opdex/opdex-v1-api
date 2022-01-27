using FluentValidation;
using Opdex.Platform.WebApi.Models;
using System.Text.RegularExpressions;

namespace Opdex.Platform.WebApi.Validation.Transactions;

public class TransactionParameterModelValidator : AbstractValidator<TransactionParameterModel>
{
    private readonly Regex _smartContractParameterRegex = new Regex("^[1-9][0-2]?#.+$", RegexOptions.Compiled);

    public TransactionParameterModelValidator()
    {
        RuleFor(r => r.Label)
            .NotEmpty().WithMessage("Label must be provided.");
        RuleFor(r => r.Value)
            .NotEmpty().WithMessage("Value must be provided.")
            .Matches(_smartContractParameterRegex).WithMessage("Value must be serialized smart contract parameter.");
    }
}
