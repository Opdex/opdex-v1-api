using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets
{
    /// <summary>
    /// Change market permissions event.
    /// </summary>
    public class ChangeMarketPermissionEventResponseModel : TransactionEventResponseModel
    {
        /// <summary>
        /// Address for which permissions were updated.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Address { get; set; }

        /// <summary>
        /// Permission type.
        /// </summary>
        /// <example>Provide</example>
        public string Permission { get; set; }

        /// <summary>
        /// If the address is authorized the permission.
        /// </summary>
        /// <example>true</example>
        public bool IsAuthorized { get; set; }
    }
}
