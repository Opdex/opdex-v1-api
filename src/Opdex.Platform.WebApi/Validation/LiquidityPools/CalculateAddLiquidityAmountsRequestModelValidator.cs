using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class CalculateAddLiquidityAmountsRequestModelValidator : AbstractValidator<CalculateAddLiquidityAmountsRequestModel>
{
    public CalculateAddLiquidityAmountsRequestModelValidator()
    {
        RuleFor(request => request.AmountIn)
            .MustBeValidSrcValue().WithMessage("Amount in must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount in must be greater than 0.");
        RuleFor(request => request.TokenIn).MustBeNetworkAddressOrCrs().WithMessage("Token in must be valid address or CRS.");
    }
}
