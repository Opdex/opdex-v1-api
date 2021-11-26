using NJsonSchema.Annotations;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions
{
    /// <summary>
    /// Quote for submitting a smart contract transaction.
    /// </summary>
    public class TransactionQuoteResponseModel
    {
        public TransactionQuoteResponseModel()
        {
            Events = new List<TransactionEvent>().AsReadOnly();
        }

        /// <summary>
        /// Value returned as part of the quoted transaction.
        /// </summary>
        /// <example>50</example>
        public object Result { get; set; }

        /// <summary>
        /// Error that occured as part of the quoted transaction.
        /// </summary>
        /// <example>Value overflow.</example>
        public string Error { get; set; }

        /// <summary>
        /// Total amount of gas consumed.
        /// </summary>
        /// <example>50000</example>
        [NotNull]
        [Range(0, double.MaxValue)]
        public uint GasUsed { get; set; }

        /// <summary>
        /// Events that occured in the quoted transaction.
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<TransactionEvent> Events { get; set; }

        /// <summary>
        /// Encoded transaction request, which can be used to replay or broadcast the transaction.
        /// </summary>
        /// <example>eyJzZW5kZXIiOiJ0UTlSdWtac0I2YkJzZW5IbkdTbzFxNjlDSnpXR254b2htIiwidG8iOiJ0THJNY1UxY3NiTjdSeEdqQk1FbkplTG9hZTNQeG1ROWNyIiwiYW1vdW50IjoiMCIsIm1ldGhvZCI6IlN5bmMiLCJwYXJhbWV0ZXJzIjpbXSwiY2FsbGJhY2siOiJodHRwczovL3Rlc3QtYXBpLm9wZGV4LmNvbS90cmFuc2FjdGlvbnMifQ==</example>
        [NotNull]
        public string Request { get; set; }
    }
}
