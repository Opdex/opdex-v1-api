using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.WebApi.Models.Requests.Tokens;

namespace Opdex.Platform.WebApi.Validation.Tokens
{
    public class TokenFilterParametersValidator : AbstractCursorValidator<TokenFilterParameters, TokensCursor>
    {
        public TokenFilterParametersValidator()
        {
            RuleFor(filter => filter.OrderBy).MustBeValidEnumValue();
            RuleForEach(filter => filter.Tokens).MustBeNetworkAddress();
            RuleForEach(filter => filter.Attributes).MustBeValidEnumValue();
            RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
        }
    }
}
