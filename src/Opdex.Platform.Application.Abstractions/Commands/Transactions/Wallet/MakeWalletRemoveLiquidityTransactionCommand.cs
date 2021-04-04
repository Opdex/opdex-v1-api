using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletRemoveLiquidityTransactionCommand : IRequest<string>
    {
        public MakeWalletRemoveLiquidityTransactionCommand(string token, string liquidity,
            ulong amountCrsMin, string amountSrcMin, string to)
        {
            Token = token;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            To = to;
        }
        
        public string Token { get; }
        public string Liquidity { get; }
        public ulong AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string To { get; }
    }
}