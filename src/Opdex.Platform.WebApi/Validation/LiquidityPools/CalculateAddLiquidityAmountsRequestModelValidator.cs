using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class CalculateAddLiquidityAmountsRequestModelValidator : AbstractValidator<CalculateAddLiquidityAmountsRequestModel>
{
    public CalculateAddLiquidityAmountsRequestModelValidator()
    {
        RuleFor(request => request.AmountIn).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.TokenIn).MustBeNetworkAddressOrCrs();
    }
}
