using TransactionStore.API.Models.Input;
using TransactionStore.API.Models.Output;
using TransactionStore.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionStore.API
{
    public class Mapper
    {
        public TransactionDTO ConvertTransactionInputModelToTransactionDTO(TransactionInputModel transaction)
        {
            return new TransactionDTO()
            {
                LeadId = transaction.LeadId,
                TypeId = transaction.TypeId,
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp
            };
        }

        public TransactionOutputModel ConvertTransactionDTOToTransactionOutputModel(TransactionDTO transaction)
        {
            return new TransactionOutputModel()
            {
                LeadId = transaction.LeadId,
                TypeId = transaction.TypeId,
                CurrencyId = transaction.CurrencyId,
                Amount = transaction.Amount,
                Timestamp = transaction.Timestamp
            };
        }
    }
}
