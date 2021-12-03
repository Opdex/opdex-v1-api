using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;

/// <summary>
/// Command to refresh an address balance, by pulling from the full node and updating the indexed value.
/// </summary>
public class CreateRefreshAddressBalanceCommand : IRequest<AddressBalanceDto>
{
    /// <summary>
    /// Creates a command to refresh an address balance, by pulling from the full node and updating the indexed value.
    /// </summary>
    /// <param name="wallet">Address of the wallet.</param>
    /// <param name="token">Address of the token.</param>
    public CreateRefreshAddressBalanceCommand(Address wallet, Address token)
    {
        if (wallet == Address.Empty) throw new ArgumentNullException(nameof(wallet));
        if (token == Address.Empty) throw new ArgumentNullException(nameof(token));
        Wallet = wallet;
        Token = token;
    }

    public Address Wallet { get; }
    public Address Token { get; }
}