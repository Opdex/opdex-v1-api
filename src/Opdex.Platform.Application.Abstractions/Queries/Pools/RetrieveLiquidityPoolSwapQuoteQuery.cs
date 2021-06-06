using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveLiquidityPoolSwapQuoteQuery : IRequest<string>
    {
        public RetrieveLiquidityPoolSwapQuoteQuery(Token tokenIn, Token tokenOut, string tokenInAmount, string tokenOutAmount, string market, string router)
        {
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            Market = market;
            Router = router;
        }
        
        public Token TokenIn { get; }
        public Token TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public string Market { get; }
        public string Router { get; }
    }
}