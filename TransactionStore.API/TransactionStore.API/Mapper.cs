using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using System.Collections.Generic;
using TransactionStore.API.Shared;
using System;

namespace TransactionStore.API
{
    public class Mapper
    {
        public TransactionDto ConvertTransactionInputModelDepositToTransactionDto(TransactionInputModel transaction)
        {
            return new TransactionDto()
            {
                LeadId = transaction.LeadId,
                TypeId = Convert.ToByte(Enums.TransactionType.Deposit),
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount
            };
        }

        public TransactionDto ConvertTransactionInputModelWithdrawToTransactionDto(TransactionInputModel transaction)
        {
            return new TransactionDto()
            {
                LeadId = transaction.LeadId,
                TypeId = Convert.ToByte(Enums.TransactionType.Withdraw),
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount
            };
        }

        public TransactionDto ConvertTransactionInputModelTransferToTransactionDto(TransactionInputModel transaction)
        {
            return new TransactionDto()
            {
                LeadId = transaction.LeadId,
                TypeId = Convert.ToByte(Enums.TransactionType.Transfer),
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount
            };
        }

        public TransactionOutputModel ConvertTransactionDtoToTransactionOutputModel(TransactionDto transaction)
        {
            return new TransactionOutputModel()
            {
                Id = transaction.Id ?? -1,
                TypeId = transaction.TypeId,
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp
            };
        }

        public List<TransactionOutputModel> ConvertTransactionDtoToTransactionOutputModels(List<TransactionDto> transactions)
        {
            List<TransactionOutputModel> models = new List<TransactionOutputModel>();
            foreach(var dto in transactions)
            {
                models.Add(ConvertTransactionDtoToTransactionOutputModel(dto));
            }
            return models;
        }
    }
}
