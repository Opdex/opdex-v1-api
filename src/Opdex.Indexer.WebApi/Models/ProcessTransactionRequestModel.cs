namespace Opdex.Indexer.WebApi.Models
{
    public class ProcessTransactionRequestModel
    {
        public string TxHash { get; set; }
        public int SortOrder { get; set; }
    }
}