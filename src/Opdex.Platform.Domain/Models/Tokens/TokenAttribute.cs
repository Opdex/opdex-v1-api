using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Domain.Models.Tokens;

public class TokenAttribute
{
    public TokenAttribute(ulong tokenId, TokenAttributeType attributeType)
    {
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
        AttributeType = attributeType.IsValid() ? attributeType : throw new ArgumentOutOfRangeException(nameof(attributeType), "Token attribute type must be valid.");
    }

    public TokenAttribute(uint id, ulong tokenId, TokenAttributeType attributeType)
    {
        Id = id;
        TokenId = tokenId;
        AttributeType = attributeType;
    }

    public uint Id { get; }
    public ulong TokenId { get; }
    public TokenAttributeType AttributeType { get; }
}