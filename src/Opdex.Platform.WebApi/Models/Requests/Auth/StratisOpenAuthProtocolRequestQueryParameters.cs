using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Opdex.Platform.WebApi.Models.Binders;
using System;

namespace Opdex.Platform.WebApi.Models.Requests.Auth
{
    /// <summary>
    /// Callback query parameters for Stratis Open Auth Protocol
    /// </summary>
    public class StratisOpenAuthProtocolRequestQueryParameters
    {
        /// <summary>
        /// The unique identifier of the Stratis ID.
        /// </summary>
        [BindRequired]
        public string Uid { get; set; }

        /// <summary>
        /// Optional expiry date indicating when the signature expires.
        /// </summary>
        [ModelBinder(typeof(UnixDateTimeModelBinder))]
        public DateTime Exp { get; set; }
    }
}