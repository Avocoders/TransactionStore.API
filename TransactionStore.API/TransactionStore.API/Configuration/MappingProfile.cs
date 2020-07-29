using AutoMapper;
using System;
using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
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
                .ForPath(dest => dest.Currency.Id, o => o.MapFrom(src => src.CurrencyId));                

            CreateMap<TransferInputModel, TransferTransaction>()
                .ForPath(dest => dest.Currency.Id, o => o.MapFrom(src => src.CurrencyId));

            CreateMap<SearchParametersInputModel, TransactionSearchParameters>();

            CreateMap<TransactionDto, TransactionOutputModel>()
                .ForPath(dest => dest.Type, o => o.MapFrom(src => Enum.GetName(typeof(TransactionType), src.Type.Id)))
                .ForPath(dest => dest.Currency, o => o.MapFrom(src => Enum.GetName(typeof(TransactionCurrency), src.Currency.Id)));

            CreateMap<TransferTransaction, TransactionOutputModel>()
                .ForPath(dest => dest.Type, o => o.MapFrom(src => Enum.GetName(typeof(TransactionType), src.Type.Id)))
                .ForPath(dest => dest.Currency, o => o.MapFrom(src => Enum.GetName(typeof(TransactionCurrency), src.Currency.Id)));
        }
    }
}
