using MediatR;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveLiquidityPoolSwapQuoteQuery : IRequest<string>
    {
        public RetrieveLiquidityPoolSwapQuoteQuery(string tokenIn, string tokenOut, string tokenInAmount, string tokenOutAmount, string market)
        {
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            Market = market;
        }
        
        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public string Market { get; }
    }
}