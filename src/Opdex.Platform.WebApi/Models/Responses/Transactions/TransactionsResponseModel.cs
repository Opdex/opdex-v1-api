using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions
{
    public class TransactionsResponseModel
    {
        public TransactionsResponseModel()
        {
            Transactions = new List<TransactionResponseModel>();
        }

        public List<TransactionResponseModel> Transactions { get; set; }
        public CursorResponseModel Paging { get; set; }
    }
}
