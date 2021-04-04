using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCreateLiquidityPoolTransactionCommand : IRequest<string>
    {
        public CreateWalletCreateLiquidityPoolTransactionCommand(string token, string sender)
        {
            Token = token;
            Sender = sender;
        }
        
        public string Token { get; }
        public string Sender { get; }
    }
}