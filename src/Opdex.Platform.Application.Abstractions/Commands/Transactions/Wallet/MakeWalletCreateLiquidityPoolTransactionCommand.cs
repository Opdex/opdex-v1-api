using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCreateLiquidityPoolTransactionCommand : IRequest<string>
    {
        public MakeWalletCreateLiquidityPoolTransactionCommand(string token, string sender, string market)
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