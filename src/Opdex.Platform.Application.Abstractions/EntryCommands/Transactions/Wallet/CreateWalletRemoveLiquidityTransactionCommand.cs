using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRemoveLiquidityTransactionCommand : IRequest<string>
    {
        public CreateWalletRemoveLiquidityTransactionCommand(string token, string liquidity,
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