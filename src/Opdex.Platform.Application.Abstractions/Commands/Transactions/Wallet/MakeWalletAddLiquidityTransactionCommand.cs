using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletAddLiquidityTransactionCommand : IRequest<string>
    {
        public MakeWalletAddLiquidityTransactionCommand(string token, ulong amountCrsDesired, string amountSrcDesired, 
            ulong amountCrsMin, string amountSrcMin, string to, string market)
        {
            Token = token;
            AmountCrsDesired = amountCrsDesired;
            AmountSrcDesired = amountSrcDesired;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            To = to;
            Market = market;
        }
        
        public string Token { get; }
        public ulong AmountCrsDesired { get; }
        public string AmountSrcDesired { get; }
        public ulong AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string To { get; }
        public string Market { get; }
    }
}