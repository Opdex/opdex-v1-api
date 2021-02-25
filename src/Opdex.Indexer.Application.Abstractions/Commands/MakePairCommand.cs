using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Indexer.Application.Abstractions.Commands
{
    public class MakePairCommand : IRequest<bool>
    {
        public MakePairCommand(string address, long tokenId)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }
            
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            Address = address;
            TokenId = tokenId;
        }
        
        public string Address { get; }
        public long TokenId { get; }
    }
}