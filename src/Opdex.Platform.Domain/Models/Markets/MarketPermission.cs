using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketPermission : BlockAudit
    {
        private Address _blame;

        public MarketPermission(long marketId,
                                Address user,
                                Permissions permission,
                                bool isAuthorized,
                                Address blame,
                                ulong createdBlock) : base(createdBlock)
        {
            if (user == Address.Empty)
            {
                throw new ArgumentNullException(nameof(user), "User address must be set.");
            }

            if (!permission.IsValid()) throw new ArgumentOutOfRangeException(nameof(permission), "Permission must be valid.");


            MarketId = marketId;
            User = user;
            Permission = permission;
            IsAuthorized = isAuthorized;
            Blame = blame;
        }

        public MarketPermission(long id,
                                long marketId,
                                Address user,
                                Permissions permission,
                                bool isAuthorized,
                                Address blame,
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
        public Address User { get; }
        public Permissions Permission { get; }
        public bool IsAuthorized { get; private set; }
        public Address Blame
        {
            get => _blame;
            private set => _blame = value != Address.Empty ? value : throw new ArgumentNullException("Blame address must be set.");
        }

        public void Authorize(Address blame, ulong block)
        {
            Blame = blame;
            IsAuthorized = true;
            SetModifiedBlock(block);
        }

        public void Revoke(Address blame, ulong block)
        {
            Blame = blame;
            IsAuthorized = false;
            SetModifiedBlock(block);
        }
    }
}
