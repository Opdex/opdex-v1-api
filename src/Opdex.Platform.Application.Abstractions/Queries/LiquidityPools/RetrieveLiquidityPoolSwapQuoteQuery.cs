using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.LiquidityPools
{
    public class RetrieveLiquidityPoolSwapQuoteQuery : IRequest<UInt256>
    {
        public RetrieveLiquidityPoolSwapQuoteQuery(Token tokenIn, Token tokenOut, UInt256 tokenInAmount, UInt256 tokenOutAmount, Address market, Address router)
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
        public UInt256 TokenInAmount { get; }
        public UInt256 TokenOutAmount { get; }
        public Address Market { get; }
        public Address Router { get; }
    }
}
