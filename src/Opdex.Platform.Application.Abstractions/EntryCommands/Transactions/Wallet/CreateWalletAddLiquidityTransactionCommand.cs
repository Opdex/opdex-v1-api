using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletAddLiquidityTransactionCommand : IRequest<string>
    {
        public CreateWalletAddLiquidityTransactionCommand(string token, ulong amountCrsDesired, string amountSrcDesired, 
            ulong amountCrsMin, string amountSrcMin, string to)
        {
            Token = token;
            AmountCrsDesired = amountCrsDesired;
            AmountSrcDesired = amountSrcDesired;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            To = to;
        }
        
        public string Token { get; }
        public ulong AmountCrsDesired { get; }
        public string AmountSrcDesired { get; }
        public ulong AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string To { get; }
    }
}