using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet
{
    public class AddressBalancesResponseModel
    {
        public AddressBalancesResponseModel()
        {
            Balances = new List<AddressBalanceResponseModel>();
        }

        public List<AddressBalanceResponseModel> Balances { get; set; }
        public CursorResponseModel Paging { get; set; }
    }
}
