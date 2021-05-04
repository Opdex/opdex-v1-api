using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletSwapTransactionCommand : IRequest<string>
    {
        public MakeWalletSwapTransactionCommand(string tokenIn, string tokenOut, string tokenInAmount, string tokenOutAmount, 
            bool tokenInExactAmount, decimal tolerance, string to, string market)
        {
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            TokenInExactAmount = tokenInExactAmount;
            Tolerance = tolerance;
            To = to;
            Market = market;
        }
        
        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public bool TokenInExactAmount { get; }
        public decimal Tolerance { get; }
        public string To { get; }
        public string Market { get; }
    }
}