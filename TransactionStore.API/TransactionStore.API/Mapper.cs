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
        public TransactionDto ConvertTransactionInputModelDepositToTransactionDto(TransactionInputModel deposit)
        {
            return new TransactionDto()
            {
                LeadId = deposit.LeadId,
                TypeId = Convert.ToByte(Enums.TransactionType.Deposit),
                CurrencyId = deposit.CurrencyId,
                Amount = deposit.Amount
            };
        }

        public TransactionDto ConvertTransactionInputModelWithdrawToTransactionDto(TransactionInputModel withdraw)
        {
            return new TransactionDto()
            {
                LeadId = withdraw.LeadId,
                TypeId = Convert.ToByte(Enums.TransactionType.Withdraw),
                CurrencyId = withdraw.CurrencyId,
                Amount = -withdraw.Amount
            };
        }

        public TransferTransactionDto ConvertTransferInputModelToTransferTransactionDto(TransferInputModel transfer)
        {
            return new TransferTransactionDto()
            {
                LeadId = transfer.LeadId,
                TypeId = Convert.ToByte(Enums.TransactionType.Transfer),
                CurrencyId = transfer.CurrencyId,
                Amount = transfer.Amount,
                DestinationLeadId = transfer.DestinationLeadId
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
