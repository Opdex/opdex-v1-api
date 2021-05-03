using MediatR;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet
{
    public class CreateWalletCreateLiquidityPoolTransactionCommand : IRequest<string>
    {
        public CreateWalletCreateLiquidityPoolTransactionCommand(string token, string sender, string market)
        {
            Token = token;
            Sender = sender;
            Market = market;
        }
        
        public string Token { get; }
        public string Sender { get; }
        public string Market { get; }
    }
}