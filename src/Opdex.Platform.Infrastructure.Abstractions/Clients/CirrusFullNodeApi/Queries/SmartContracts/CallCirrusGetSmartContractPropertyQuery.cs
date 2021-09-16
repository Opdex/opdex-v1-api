using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts
{
    public class CallCirrusGetSmartContractPropertyQuery : IRequest<string>
    {
        public CallCirrusGetSmartContractPropertyQuery(Address contract, string propertyStateKey, SmartContractParameterType propertyType, ulong blockHeight)
        {
            if (contract == Address.Empty)
            {
                throw new ArgumentNullException(nameof(contract), "Contract address must be provided.");
            }

            if (!propertyStateKey.HasValue())
            {
                throw new ArgumentNullException(nameof(propertyStateKey), "Property state key value must be provided.");
            }

            if (!propertyType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(propertyType), "Property type must be a valid value.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Contract = contract;
            PropertyStateKey = propertyStateKey;
            PropertyType = propertyType;
            BlockHeight = blockHeight;
        }

        public Address Contract { get; }
        public string PropertyStateKey { get; }
        public SmartContractParameterType PropertyType { get; }
        public ulong BlockHeight { get; }
    }
}
