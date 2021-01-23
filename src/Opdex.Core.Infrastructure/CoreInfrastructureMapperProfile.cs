using AutoMapper;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

namespace Opdex.Core.Infrastructure
{
    public class CoreInfrastructureMapperProfile : Profile
    {
        public CoreInfrastructureMapperProfile()
        {
            CreateMap<TransactionReceiptDto, TransactionReceipt>()
                .ConstructUsing(src => new TransactionReceipt(src.TransactionHash, src.BlockHash, src.GasUsed,
                    src.From, src.To, src.Success, src.Logs))
                .ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}