using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletRemoveLiquidityTransactionCommand : IRequest<string>
    {
        public CreateWalletRemoveLiquidityTransactionCommand(string pool, string liquidity,
            string amountCrsMin, string amountSrcMin, string walletAddress, string market)
        {
            Pool = pool;
            Liquidity = liquidity;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            WalletAddress = walletAddress;
            Market = market;
        }
        
        public string Liquidity { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string WalletAddress { get; }
        public string Pool { get; }
        public string Market { get; }
    }
}