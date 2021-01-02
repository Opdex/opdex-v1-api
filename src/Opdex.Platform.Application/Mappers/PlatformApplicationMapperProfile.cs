using AutoMapper;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Application.Mappers
{
    public class PlatformApplicationMapperProfile : Profile
    {
        public PlatformApplicationMapperProfile()
        {
            CreateMap<Token, TokenDto>();
            CreateMap<Pair, PairDto>();

        }
    }
}