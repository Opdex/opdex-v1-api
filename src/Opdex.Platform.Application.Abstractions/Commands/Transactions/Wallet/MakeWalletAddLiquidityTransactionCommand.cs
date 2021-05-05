using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletAddLiquidityTransactionCommand : IRequest<string>
    {
        public MakeWalletAddLiquidityTransactionCommand(string token, string amountCrs, string amountSrc, 
            string amountCrsMin, string amountSrcMin, string to, string market)
        {
            Token = token;
            AmountCrs = amountCrs;
            AmountSrc = amountSrc;
            AmountCrsMin = amountCrsMin;
            AmountSrcMin = amountSrcMin;
            To = to;
            Market = market;
        }
        
        public string Token { get; }
        public string AmountCrs { get; }
        public string AmountSrc { get; }
        public string AmountCrsMin { get; }
        public string AmountSrcMin { get; }
        public string To { get; }
        public string Market { get; }
    }
}