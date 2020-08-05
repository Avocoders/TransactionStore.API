using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
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
                .ForPath(dest => dest.Currency.Id, o => o.MapFrom(src => src.CurrencyId))
                .AfterMap((src, dest) => { dest.Type = new TransactionTypeDto() { Id = (byte)TransactionType.Transfer };});

            CreateMap<SearchParametersInputModel, TransactionSearchParameters>();

            CreateMap<TransactionDto, TransactionOutputModel>()
                .ForPath(dest => dest.Type, o => o.MapFrom(src => Enum.GetName(typeof(TransactionType), src.Type.Id)))
                .ForPath(dest => dest.Currency, o => o.MapFrom(src => Enum.GetName(typeof(TransactionCurrency), src.Currency.Id)))
                .AfterMap((src, dest) => { var tmp = (TransferTransaction)src; dest.AccountIdReceiver = tmp.AccountIdReceiver; }); 

            CreateMap<TransferTransaction, TransactionOutputModel>()
                .ForPath(dest => dest.Type, o => o.MapFrom(src => Enum.GetName(typeof(TransactionType), src.Type.Id)))
                .ForPath(dest => dest.Currency, o => o.MapFrom(src => Enum.GetName(typeof(TransactionCurrency), src.Currency.Id)));
        }
    }
}
 