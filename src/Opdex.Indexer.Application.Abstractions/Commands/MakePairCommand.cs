using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Indexer.Application.Abstractions.Commands
{
    public class MakePairCommand : IRequest<bool>
    {
        public MakePairCommand(string address)
        {
            Address = address.HasValue() ? address : throw new ArgumentNullException(nameof(address));
        }
        
        public string Address { get; }
    }
}