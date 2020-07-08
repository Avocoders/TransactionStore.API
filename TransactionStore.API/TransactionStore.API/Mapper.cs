using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using System.Collections.Generic;

namespace TransactionStore.API
{
    public class Mapper
    {
        public TransactionDto ConvertTransactionInputModelToTransactionDto(TransactionInputModel transaction)
        {
            return new TransactionDto()
            {
                LeadId = transaction.LeadId,
                TypeId = transaction.TypeId,
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp
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
