using FluentValidation;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using System.Text.RegularExpressions;

namespace Opdex.Platform.WebApi.Validation.Tokens;

public class TokenFilterParametersValidator : AbstractCursorValidator<TokenFilterParameters, TokensCursor>
{
    private static readonly Regex Alphanumeric = new Regex("^[0-9A-Za-z ]+$", RegexOptions.Compiled);

    public TokenFilterParametersValidator()
    {
        RuleFor(filter => filter.Keyword)
            .Must(keyword => !keyword.HasValue() || Alphanumeric.IsMatch(keyword))
            .WithMessage("Keyword must consist of letters, numbers and spaces only.");

        RuleFor(filter => filter.OrderBy).MustBeValidEnumValue();
        RuleFor(filter => filter.TokenType).MustBeValidEnumValue();
        RuleForEach(filter => filter.Tokens).MustBeNetworkAddress();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}