namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class QuoteReplayRequest
    {
        /// <summary>
        /// Encoded request string for a quoted transaction.
        /// </summary>
        /// <example>eyJzZW5kZXIiOiJ0UTlSdWtac0I2YkJzZW5IbkdTbzFxNjlDSnpXR254b2htIiwidG8iOiJ0THJNY1UxY3NiTjdSeEdqQk1FbkplTG9hZTNQeG1ROWNyIiwiYW1vdW50IjoiMCIsIm1ldGhvZCI6IlN5bmMiLCJwYXJhbWV0ZXJzIjpbXSwiY2FsbGJhY2siOiJodHRwczovL3Rlc3QtYXBpLm9wZGV4LmNvbS90cmFuc2FjdGlvbnMifQ==</example>
        public string Quote { get; set; }
    }
}
