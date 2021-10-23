using System;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    /// <summary>
    /// Retrieve a token by it's internal Id.
    /// </summary>
    public class RetrieveTokenByIdQuery : FindQuery<Token>
    {
        /// <summary>
        /// Constructor to build a retrieve token by Id query.
        /// </summary>
        /// <param name="tokenId">The internal Id of the token.</param>
        /// <param name="findOrThrow">Flag indicating to throw if the requested token is not found, default is true.</param>
        public RetrieveTokenByIdQuery(ulong tokenId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
            }

            TokenId = tokenId;
        }

        public ulong TokenId { get; }
    }
}
