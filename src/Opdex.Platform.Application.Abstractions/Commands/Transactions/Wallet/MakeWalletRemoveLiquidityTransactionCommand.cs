using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRemoveLiquidityTransactionCommand : IRequest<string>
    {
        public MakeWalletRemoveLiquidityTransactionCommand(string token, string liquidity,
            string amountCrsMin, string amountSrcMin, string to, string market)
        {
            Token = token;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            To = to;
            Market = market;
        }
        
        public string Token { get; }
        public string Liquidity { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string To { get; }
        public string Market { get; }
    }
}