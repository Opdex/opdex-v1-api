using AutoMapper;
using Opdex.Core.Domain.Models.Transaction;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure
{
    public class CoreInfrastructureMapperProfile : Profile
    {
        public CoreInfrastructureMapperProfile()
        {
            CreateMap<TransactionReceiptDto, Transaction>()
                .ConstructUsing(src => new Transaction(src.TransactionHash, src.BlockHeight, src.GasUsed,
                    src.From, src.To))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}