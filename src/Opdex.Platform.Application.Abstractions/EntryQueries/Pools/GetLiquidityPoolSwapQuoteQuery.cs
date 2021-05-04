using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetLiquidityPoolSwapQuoteQuery : IRequest<string>
    {
        public GetLiquidityPoolSwapQuoteQuery(string tokenIn, string tokenOut, string tokenInAmount, string tokenOutAmount, 
            string tokenInPool, string tokenOutPool, string market)
        {
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            TokenInPool = tokenInPool;
            TokenOutPool = tokenOutPool;
            Market = market;
        }
        
        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public string TokenInPool { get; }
        public string TokenOutPool { get; }
        public string Market { get; }
    }
}