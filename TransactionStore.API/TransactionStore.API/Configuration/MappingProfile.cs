using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionStore.API.Models.Input;
using TransactionStore.Core.Shared;
using TransactionStore.Data;
using TransactionStore.Data.DTO;

namespace TransactionStore.API.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TransactionInputModel, TransactionDto>()
                .ForPath(dest => dest.Currency.Id, o => o.MapFrom(src => src.CurrencyId))            
                .ForPath(destination => destination.Type.Id,
                 opt => opt.MapFrom(source => Enum.GetName(typeof(TransactionType), 1)));

            //CreateMap<TransactionInputModel, TransactionDto>()
            //    .ForPath(dest => dest.Currency.Id, o => o.MapFrom(src => src.CurrencyId))
            //    .ForPath(dest => dest.Amount, o => o.MapFrom(src => -src.Amount))
            //    .ForMember(destination => destination.Type.Id,
            //     opt => opt.MapFrom(source => Enum.GetName(typeof(TransactionType), 2)));

            CreateMap<TransferInputModel, TransferTransaction>()
                .ForPath(dest => dest.Currency.Id, o => o.MapFrom(src => src.CurrencyId));
        }
    }
}
