using System;

namespace Opdex.Platform.Domain.Models
{
    public abstract class BlockAudit
    {
        protected BlockAudit(ulong createdBlock, ulong modifiedBlock = 0)
        {
            if (createdBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(createdBlock));
            }

            if (modifiedBlock != 0 && modifiedBlock < createdBlock)
            {
                throw new ArgumentNullException(nameof(modifiedBlock));
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
                throw new ArgumentOutOfRangeException(nameof(block));
            }
            
            ModifiedBlock = block;
        }
    }
}