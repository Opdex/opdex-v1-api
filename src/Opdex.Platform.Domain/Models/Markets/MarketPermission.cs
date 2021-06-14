using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketPermission : BlockAudit
    {
        private string blame;

        public MarketPermission(long marketId,
                                string user,
                                Permissions permission,
                                bool isAuthorized,
                                string blame,
                                ulong createdBlock) : base(createdBlock)
        {
            if (!user.HasValue())
            {
                throw new ArgumentNullException(nameof(user), "User address must be set.");
            }

            if (permission == Permissions.Unknown)
            {
                throw new ArgumentNullException(nameof(permission), "Permission must be set.");
            }

            MarketId = marketId;
            User = user;
            Permission = permission;
            IsAuthorized = isAuthorized;
            Blame = blame;
        }

        public MarketPermission(long id,
                                long marketId,
                                string user,
                                Permissions permission,
                                bool isAuthorized,
                                string blame,
                                ulong createdBlock,
                                ulong modifiedBlock) : base(createdBlock, modifiedBlock)
        {
            Id = id;
            MarketId = marketId;
            User = user;
            Permission = permission;
            IsAuthorized = isAuthorized;
            Blame = blame;
        }

        public long Id { get; }
        public long MarketId { get; }
        public string User { get; }
        public Permissions Permission { get; }
        public bool IsAuthorized { get; private set; }
        public string Blame
        {
            get => blame;
            private set => blame = value.HasValue() ? value : throw new ArgumentNullException("Blame address must be set.");
        }

        public void Authorize(string blame, ulong block)
        {
            Blame = blame;
            IsAuthorized = true;
            SetModifiedBlock(block);
        }

        public void Revoke(string blame, ulong block)
        {
            Blame = blame;
            IsAuthorized = false;
            SetModifiedBlock(block);
        }
    }
}