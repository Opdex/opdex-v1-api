namespace Opdex.Platform.Common.Enums
{
    public enum TransactionEligibilityType : short
    {
        /// <summary>
        /// The default status of an unknown and unvalidated transaction.
        /// </summary>
        PendingInitialValidation = 1,

        /// <summary>
        /// The status when transaction logs are of Opdex known types/schema
        /// but the contracts themselves have not been validated to be known to Opdex.
        /// </summary>
        PendingContractValidation = 2,

        /// <summary>
        /// The status given to a transaction that shouldn't be persisted but may have
        /// attributes that Opdex can act on such as general token approvals and transfers.
        /// </summary>
        PartiallyEligible = 3,

        /// <summary>
        /// The transaction is not an Opdex eligible transaction to be persisted.
        /// </summary>
        Ineligible = 4,

        /// <summary>
        /// The transaction is an eligible Opdex transaction to be persisted.
        /// </summary>
        Eligible = 5
    }
}
