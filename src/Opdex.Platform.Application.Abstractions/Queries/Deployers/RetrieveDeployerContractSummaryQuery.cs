using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Deployers;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Deployers
{
    /// <summary>
    /// Retrieve select properties from a deployer smart contract based on the provided block height.
    /// </summary>
    public class RetrieveDeployerContractSummaryQuery : IRequest<DeployerContractSummary>
    {
        /// <summary>
        /// Creates a query to retrieve a deployer contract summary by address query.
        /// </summary>
        /// <param name="deployer">The address of the deployer contract.</param>
        /// <param name="blockHeight">The block height to query the contract's state at.</param>
        /// <param name="includePendingOwner">Flag to include the deployer pending owner property, default is false.</param>
        /// <param name="includeOwner">Flag to include the deployer owner property, default is false.</param>
        public RetrieveDeployerContractSummaryQuery(Address deployer, ulong blockHeight, bool includePendingOwner = false, bool includeOwner = false)
        {
            if (deployer == Address.Empty)
            {
                throw new ArgumentNullException(nameof(deployer), "Deployer address must be provided.");
            }

            if (blockHeight == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
            }

            Deployer = deployer;
            BlockHeight = blockHeight;
            IncludePendingOwner = includePendingOwner;
            IncludeOwner = includeOwner;
        }

        public Address Deployer { get; }
        public ulong BlockHeight { get; }
        public bool IncludePendingOwner { get; }
        public bool IncludeOwner { get; }
    }
}
