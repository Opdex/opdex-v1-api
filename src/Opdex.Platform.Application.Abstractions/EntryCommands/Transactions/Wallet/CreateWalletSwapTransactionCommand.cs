using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletSwapTransactionCommand : IRequest<string>
    {
        public CreateWalletSwapTransactionCommand(string tokenIn, string tokenOut, string tokenInAmount, string tokenOutAmount, 
            bool tokenInExactAmount, decimal tolerance, string to)
        {
            TokenIn = tokenIn;
            TokenOut = tokenOut;
            TokenInAmount = tokenInAmount;
            TokenOutAmount = tokenOutAmount;
            TokenInExactAmount = tokenInExactAmount;
            Tolerance = tolerance;
            To = to;
        }
        
        public string TokenIn { get; }
        public string TokenOut { get; }
        public string TokenInAmount { get; }
        public string TokenOutAmount { get; }
        public bool TokenInExactAmount { get; }
        public decimal Tolerance { get; }
        public string To { get; }
    }
}