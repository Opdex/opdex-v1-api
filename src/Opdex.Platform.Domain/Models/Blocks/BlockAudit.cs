using System;

namespace Opdex.Platform.Domain.Models.Blocks;

public abstract class BlockAudit
{
    protected BlockAudit(ulong createdBlock, ulong modifiedBlock = 0)
    {
        if (createdBlock < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(createdBlock), "Created block must be greater than 0.");
        }

        if (modifiedBlock != 0 && modifiedBlock < createdBlock)
        {
            throw new ArgumentOutOfRangeException(nameof(modifiedBlock), "Modified block cannot be before created block.");
        }

        CreatedBlock = createdBlock;
        ModifiedBlock = modifiedBlock != 0 ? modifiedBlock : createdBlock;
    }

    public ulong CreatedBlock { get; }
    public ulong ModifiedBlock { get; private set; }

    protected void SetModifiedBlock(ulong block)
    {
        if (block < ModifiedBlock)
        {
            throw new ArgumentOutOfRangeException(nameof(block), "Modified block cannot be before created block.");
        }

        ModifiedBlock = block;
    }
}