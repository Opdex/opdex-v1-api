using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets
{
    public class ChangeMarketPermissionLog : TransactionLog
    {
        public ChangeMarketPermissionLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.ChangeMarketPermissionLog, address, sortOrder)
        {
            Address fromAddress = (string)log?.address;
            byte permission = log?.permission;
            bool isAuthorized = log?.isAuthorized;

            if (fromAddress == Address.Empty)
            {
                throw new ArgumentNullException(nameof(fromAddress), "Address must be set.");
            }

            var permissionCast = (MarketPermissionType)permission;
            if (!permissionCast.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(permission), "Permission must be valid.");
            }

            Address = fromAddress;
            Permission = permissionCast;
            IsAuthorized = isAuthorized;
        }

        public ChangeMarketPermissionLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
            : base(TransactionLogType.ChangeMarketPermissionLog, id, transactionId, address, sortOrder)
        {
            var logDetails = DeserializeLogDetails(details);
            Address = logDetails.Address;
            Permission = logDetails.Permission;
            IsAuthorized = logDetails.IsAuthorized;
        }

        public Address Address { get; }
        public MarketPermissionType Permission { get; }
        public bool IsAuthorized { get; }

        public struct LogDetails
        {
            public Address Address { get; set; }
            public MarketPermissionType Permission { get; set; }
            public bool IsAuthorized { get; set; }
        }

        private static LogDetails DeserializeLogDetails(string details)
        {
            return JsonConvert.DeserializeObject<LogDetails>(details);
        }

        public override string SerializeLogDetails()
        {
            return JsonConvert.SerializeObject(new LogDetails
            {
                Address = Address,
                Permission = Permission,
                IsAuthorized = IsAuthorized
            });
        }
    }
}
