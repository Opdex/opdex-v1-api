using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions
{
    public abstract class BaseQuoteCommand : IRequest<TransactionQuoteDto>
    {
        protected BaseQuoteCommand(Address contractAddress, Address walletAddress)
        {
            if (contractAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(contractAddress), "Contract address must be provided.");
            }

            if (walletAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(walletAddress), "Wallet address must be provided.");
            }

            ContractAddress = contractAddress;
            WalletAddress = walletAddress;
        }

        public Address ContractAddress { get; }
        public Address WalletAddress { get; }
    }
}
